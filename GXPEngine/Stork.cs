using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using MathfExtensions;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class Stork : AnimationSprite, IHasSpeed
    {
        //private AnimationSprite _body;

        //private AnimationSprite _leftWing;
        //private AnimationSprite _rightWing;

        private const int PRESSURE_TIME_SENSITIVITY = 200;

        private int _leftWingFrame;
        private int _rightWingFrame;

        private int _leftWingTime;
        private int _rightWingTime;
        private int _wingAnimationSpeed = 600;

        private IStorkInput _storkInput;
        private bool _hasStorkInput;

        private int _leftWingPressure;
        private int _rightWingPressure;

        private float _maxSpeed = 200f;
        private float _minSpeed = 40f;
        private float _currentSpeed;
        private float _dampSpeed = -20f;
        private float _dampTurn = -100f;

        private float _leftPush;
        private float _rightPush;

        private float _angularPush;
        private float _angularPushDelta;

        private Vector2 _forward;
        private Vector2 _lastPos;

        private IGridDataUpdater _iUpdater;
        private bool _hasIGridDataUpdater;

        private EasyDraw easy;
        private Rectangle _customColliderBounds;
        private bool _hasIColliderListener;
        private IColliderListener _iColliderListener;

        private bool _inputEnabled;

        private bool _isPushing;

        private const int PUSH_FRAMECOUNT = 9;

        public Stork(float pMaxSpeed = 200) : base("data/Stork 2.png", 5, 3, 12, false, true)
        {
            _maxSpeed = pMaxSpeed;

            _inputEnabled = true;

            SetOrigin(85f / 2, 85f / 2);

            _customColliderBounds = new Rectangle(13 - 85f / 2, 5 - 85f / 2, 43, 76);

            IsMoveEnabled = true;

            // _body = new AnimationSprite("data/Stork00.png", 3, 1, -1, false, false);
            // _body.SetOrigin(85f / 2, 85f / 2);
            // _body.visible = false;
            //AddChild(_body);

            // _leftWing = new AnimationSprite("data/Stork_Left_Wing.png", 3, 1, -1, false, false);
            // _rightWing = new AnimationSprite("data/Stork_Right_Wing.png", 3, 1, -1, false, false);
            //
            // _leftWing.SetOrigin(85f / 2, 85f / 2);
            // _rightWing.SetOrigin(85f / 2, 85f / 2);
            //
            // AddChild(_leftWing);
            // AddChild(_rightWing);
        }

        private IEnumerator ResizeDebug()
        {
            int d = 400;
            int time = 0;
            while (true)
            {
                var s = Easing.CubicEaseOut(time, 0.8f, 1, d);

                SetScaleXY(s, s);

                time = time > d ? 0 : time + Time.deltaTime;
                yield return null;
            }
        }

        void Update()
        {
            var rotationRad = _rotation.DegToRad();
            _forward.x = Mathf.Cos(rotationRad);
            _forward.y = Mathf.Sin(rotationRad);

            _lastPos.x = x;
            _lastPos.y = y;

            if (_inputEnabled && _hasStorkInput)
            {
                _leftWingPressure = _storkInput.LeftWingPressure;
                _rightWingPressure = _storkInput.RightWingPressure;
            }

            if (_leftWingPressure < PRESSURE_TIME_SENSITIVITY)
            {
                //SetFrame(0);
                _leftWingTime = 0;
                _leftPush = 0;
            }
            else
            {
                _leftWingTime += _leftWingPressure > 0 ? Time.deltaTime : -Time.deltaTime;

                _leftPush = Easing.Ease(Easing.Equation.CubicEaseInOut, _leftWingTime, 0, PUSH_FRAMECOUNT,
                    _wingAnimationSpeed);

                _leftPush = Mathf.Clamp(_leftPush, 0, PUSH_FRAMECOUNT);


                int leftFrame = Mathf.Round(_leftPush);

                _leftPush = Mathf.Clamp(_leftPush, 0, 3);

                if (_rightWingPressure > PRESSURE_TIME_SENSITIVITY)
                    SetFrame(leftFrame);
            }

            if (_rightWingPressure < PRESSURE_TIME_SENSITIVITY)
            {
                //SetFrame(0);
                _rightWingTime = 0;
                _rightPush = 0;
            }
            else
            {
                _rightWingTime += _rightWingPressure > 0 ? Time.deltaTime : -Time.deltaTime;

                _rightPush = Easing.Ease(Easing.Equation.CubicEaseInOut, _rightWingTime, 0, PUSH_FRAMECOUNT,
                    _wingAnimationSpeed);
                _rightPush = Mathf.Clamp(_rightPush, 0, PUSH_FRAMECOUNT);
                int rightFrame = Mathf.Round(_rightPush);

                _rightPush = Mathf.Clamp(_rightPush, 0, 3);

                if (_leftWingPressure >= PRESSURE_TIME_SENSITIVITY)
                    SetFrame(rightFrame);
            }

            bool hasPressure = _leftPush > 0 || _rightPush > 0;

            if (!hasPressure && _currentSpeed > _minSpeed)
            {
                _currentSpeed += _dampSpeed * Time.delta;
            }
            else if (hasPressure &&
                     (_leftWingTime > _wingAnimationSpeed * 2 || _rightWingTime > _wingAnimationSpeed * 2))
            {
                _currentSpeed += _dampSpeed * Time.delta;
            }
            else
            {
                float pushForce = 20;

                if (_leftPush > 0 && _rightPush > 0)
                {
                    //Each wing add a "push" to the speed
                    float left = _leftPush * pushForce;
                    _currentSpeed += left * Time.delta;

                    float right = _rightPush * pushForce;
                    _currentSpeed += right * Time.delta;

                    if (!_isPushing)
                    {
                        var channel = SoundManager.Instance.SetFxVolume(2, 1f);
                    }

                    _isPushing = true;
                }
                else
                {
                    _currentSpeed += _dampSpeed * Time.delta;

                    _isPushing = false;
                }
            }

            if (IsMoveEnabled)
                TurnStork(Time.delta);

            _currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);

            if (_hasIGridDataUpdater && IsMoveEnabled)
            {
                _iUpdater.NextPosition(_pos, _pos + _forward * _currentSpeed * Time.delta);
            }

            if (_currentSpeed > 0 && IsMovingInGrid && IsMoveEnabled) //AllowMove will be calculated by a _iUpdater
            {
                //var nextPos = _pos + _forward * _currentSpeed * Time.deltaTime * 0.001f;
                Move(_currentSpeed * Time.delta, 0);
            }

            //
            if (_hasIGridDataUpdater)
            {
                _iUpdater.OnMove(_pos, _lastPos);
            }

            GL.glfwSetWindowTitle(
                $"speed: {_currentSpeed:0.00} | left: {_leftPush:0.00} | right: {_rightPush:0.00} | angularPushDelta: {_angularPushDelta:0.00} | angularPush: {_angularPush:0.00} | leftPress: {_leftWingPressure:0.00} | pos: {_pos} | frame: {_currentFrame}");

            DrawBoundBox();
        }

        private void TurnStork(float delta)
        {
            _angularPushDelta = _leftPush - _rightPush;
            float angularPushDeltaAbs = Mathf.Abs(_angularPushDelta);

            //onsole.WriteLine($"{this}: angularpushDelta: {_angularPushDelta:0.00}");

            bool hasPressure = (_leftPush > 0 || _rightPush > 0);

            if (true) //angularPushDeltaAbs >= 0)
            {
                _angularPush = _angularPushDelta * 15;
                Turn(-1 * _angularPush * delta);

                if (_angularPushDelta > 0.050f)
                {
                    //_body.visible = true;
                    //_leftWing.visible = false;
                    //_rightWing.visible = false;
                    SetFrame(10);
                }
                else if (_angularPushDelta < -0.050f)
                {
                    //_body.visible = true;
                    //_leftWing.visible = false;
                    //_rightWing.visible = false;
                    SetFrame(11);
                }
                else if (!hasPressure)
                {
                    //_body.visible = false;
                    // _leftWing.visible = true;
                    //_rightWing.visible = true; 
                    SetFrame(0);
                }
            }
            else
            {
                _angularPushDelta = 0;
                _angularPush = 0;
            }
        }

        void OnCollision(GameObject other)
        {
            //Console.WriteLine($"{this.name} ==> {other.name}");
            if (_hasIColliderListener)
            {
                _iColliderListener.OnCollisionWith(this, other);
            }
        }
        
        public void SetSpeed(int i)
        {
            _currentSpeed = 0;
        }

        public override Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            ret[1] = TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            ret[2] = TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            ret[3] = TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);
            return ret;
        }

        void DrawBoundBox()
        {
            var p0 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            var p1 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            var p2 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            var p3 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);

            CanvasDebugger2.Instance.DrawLine(p0.x, p0.y, p1.x, p1.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p1.x, p1.y, p2.x, p2.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p2.x, p2.y, p3.x, p3.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p3.x, p3.y, p0.x, p0.y, Color.Blue);
        }

        public IStorkInput StorkInput
        {
            get => _storkInput;
            set
            {
                _storkInput = value;
                _hasStorkInput = value != null;
            }
        }

        float IHasSpeed.Speed => _currentSpeed;

        float IHasSpeed.MaxSpeed => _maxSpeed;

        GameObject IHasSpeed.gameObject => this;

        public bool IsMovingInGrid { get; set; }

        public bool IsMoveEnabled { get; set; }
        
        public bool IsCollisionDisabled { get; set; }

        public IGridDataUpdater IUpdater
        {
            get => _iUpdater;
            set
            {
                _iUpdater = value;
                _hasIGridDataUpdater = value != null;
            }
        }

        public IColliderListener ColliderListener
        {
            get => _iColliderListener;
            set
            {
                _iColliderListener = value;
                _hasIColliderListener = value != null;
            }
        }

        public bool InputEnabled
        {
            get => _inputEnabled;
            set => _inputEnabled = value;
        }

        public Vector2 Forward => _forward;

        public float CurrentSpeed => _currentSpeed;
    }

    public interface IColliderListener
    {
        void OnCollisionWith(Stork stork, GameObject other);
    }
}