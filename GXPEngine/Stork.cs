using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using MathfExtensions;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class Stork : Sprite, IHasSpeed
    {
        private AnimationSprite _body;

        private AnimationSprite _leftWing;
        private AnimationSprite _rightWing;

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
        private Vector2 _pos;
        private Vector2 _lastPos;

        private IGridDataUpdater _iUpdater;
        private bool _hasIGridDataUpdater;

        private EasyDraw easy;
        private Rectangle _customColliderBounds;
        private bool _hasIColliderListener;
        private IColliderListener _iColliderListener;

        public Stork() : base("data/Stork Background.png")
        {
            // easy = new EasyDraw(85, 85, false);
            // easy.SetOrigin(85f / 2, 85f / 2);
            // easy.Clear(Color.Black);
            // AddChild(easy);

            SetOrigin(85f / 2, 85f / 2);

            _customColliderBounds = new Rectangle(13 - 85f / 2, 5 - 85f / 2, 43, 76);

            _body = new AnimationSprite("data/Stork00.png", 3, 1, -1, false, false);
            _body.SetOrigin(85f / 2, 85f / 2);
            _body.visible = false;
            AddChild(_body);

            _leftWing = new AnimationSprite("data/Stork_Left_Wing.png", 3, 1, -1, false, false);
            _rightWing = new AnimationSprite("data/Stork_Right_Wing.png", 3, 1, -1, false, false);

            //_leftWing.x = _leftWing.y = 85 * 0.5f;
            //_rightWing.x = _rightWing.y = 85 * 0.5f;

            _leftWing.SetOrigin(85f / 2, 85f / 2);
            _rightWing.SetOrigin(85f / 2, 85f / 2);

            // _leftWing.Turn(90);
            // _rightWing.Turn(90);

            AddChild(_leftWing);
            AddChild(_rightWing);
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
            float delta = Time.deltaTime * 0.001f;

            var rotationRad = _rotation.DegToRad();
            _forward.x = Mathf.Cos(rotationRad);
            _forward.y = Mathf.Sin(rotationRad);

            _lastPos.x = x;
            _lastPos.y = y;

            if (_hasStorkInput)
            {
                _leftWingPressure = _storkInput.LeftWingPressure;
                _rightWingPressure = _storkInput.RightWingPressure;
            }

            if (_leftWingPressure < PRESSURE_TIME_SENSITIVITY)
            {
                _leftWing.SetFrame(1);
                _leftWingTime = 0;
                _leftPush = 0;
            }
            else
            {
                _leftWingTime += _leftWingPressure > 0 ? Time.deltaTime : -Time.deltaTime;

                _leftPush = Easing.Ease(Easing.Equation.CubicEaseInOut, _leftWingTime, 0, _leftWing.frameCount,
                    _wingAnimationSpeed);
                _leftPush = Mathf.Clamp(_leftPush, 0, _leftWing.frameCount);
                int leftFrame = Mathf.Round(_leftPush);

                if (_rightWingPressure > PRESSURE_TIME_SENSITIVITY)
                    _leftWing.SetFrame(leftFrame);
            }

            if (_rightWingPressure < PRESSURE_TIME_SENSITIVITY)
            {
                _rightWing.SetFrame(1);
                _rightWingTime = 0;
                _rightPush = 0;
            }
            else
            {
                _rightWingTime += _rightWingPressure > 0 ? Time.deltaTime : -Time.deltaTime;

                _rightPush = Easing.Ease(Easing.Equation.CubicEaseInOut, _rightWingTime, 0, _rightWing.frameCount,
                    _wingAnimationSpeed);
                _rightPush = Mathf.Clamp(_rightPush, 0, _rightWing.frameCount);
                int rightFrame = Mathf.Round(_rightPush);

                if (_leftWingPressure >= PRESSURE_TIME_SENSITIVITY)
                    _rightWing.SetFrame(rightFrame);
            }

            bool hasPressure = _leftPush > 0 || _rightPush > 0;

            if (!hasPressure && _currentSpeed > _minSpeed)
            {
                _currentSpeed += _dampSpeed * delta;
            }
            else if (hasPressure &&
                     (_leftWingTime > _wingAnimationSpeed * 2 || _rightWingTime > _wingAnimationSpeed * 2))
            {
                _currentSpeed += _dampSpeed * delta;
            }
            else
            {
                float pushForce = 20;

                if (_leftPush > 0 && _rightPush > 0)
                {
                    //Each wing add a "push" to the speed
                    float left = _leftPush * pushForce;
                    _currentSpeed += left * delta;

                    float right = _rightPush * pushForce;
                    _currentSpeed += right * delta;
                }
                else
                {
                    _currentSpeed += _dampSpeed * delta;
                }
            }
            
            TurnStork(delta);

            _currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);

            if (_currentSpeed > 0)
            {
                //var nextPos = _pos + _forward * _currentSpeed * Time.deltaTime * 0.001f;
                Move(_currentSpeed * delta, 0);
            }

            _pos.x = x;
            _pos.y = y;

            //
            if (_hasIGridDataUpdater)
            {
                _iUpdater.OnMove(_pos, _lastPos);
            }

            GL.glfwSetWindowTitle(
                $"speed: {_currentSpeed:0.00} | left: {_leftPush:0.00} | right: {_rightPush:0.00} | angularPushDelta: {_angularPushDelta:0.00} | angularPush: {_angularPush:0.00} | leftPress: {_leftWingPressure:0.00}");

            DrawBoundBox();
        }

        private void TurnStork(float delta)
        {
            _angularPushDelta = _leftPush - _rightPush;
            float angularPushDeltaAbs = Mathf.Abs(_angularPushDelta);
            
            //onsole.WriteLine($"{this}: angularpushDelta: {_angularPushDelta:0.00}");
            
            if (angularPushDeltaAbs >= 0)
            {
                _angularPush = _angularPushDelta * 15;
                Turn(-1 * _angularPush * delta);
                
                if (_angularPushDelta > 0.5f)
                {
                    _body.visible = true;
                    _leftWing.visible = false;
                    _rightWing.visible = false;
                    _body.SetFrame(2);
                }
                else if (_angularPushDelta < -0.5f)
                {
                    _body.visible = true;
                    _leftWing.visible = false;
                    _rightWing.visible = false;
                    _body.SetFrame(1);
                }
                else
                {
                    _body.visible = false;
                    _leftWing.visible = true;
                    _rightWing.visible = true;
                    _body.SetFrame(0);
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
                ColliderListener.OnCollisionWith(this, other);
            }
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
    }

    public interface IColliderListener
    {
        void OnCollisionWith(GameObject go, GameObject other);
    }
}