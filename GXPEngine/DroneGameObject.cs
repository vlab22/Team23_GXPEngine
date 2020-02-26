using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;
using MathfExtensions;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class DroneGameObject : AnimationSprite, IHasDistanceToTarget
    {
        public enum DroneState
        {
            SEARCHING_ENEMY,
            ENEMY_DETECTED,
            CHASING_ENEMY,
            STOP_CHASING_ENEMY,
            RETURN_TO_START_POINT_AFTER_HIT,
            RETURN_TO_START_POINT_AFTER_CHASING_AND_SEARCHING_ENEMY,
            HIT_ENEMY,
            END_LEVEL,
        }

        private DroneState _state;

        private float _maxSpeed;
        private float _currentMaxSpeed;
        private float _currentSpeed;
        private float _waitingSpeed = 32;
        private Rectangle _customColliderBounds;

        private Vector2 _startPosition;

        private IEnumerator _searchingRoutine;

        private Vector2 _forward;

        private IEnumerator _chasingRoutine;
        private float _detectEnemyRange = 335;
        private float _stopChasingRange = 435;

        private IEnumerator _enemyDetectedRoutine;
        private IEnumerator _returnToStartPointRoutine;

        private GameObject _enemy;

        private IDroneBehaviorListener _droneBehaviorListener;
        private bool _hasDroneBehaviourListener;

        private static uint IdCounter;
        private uint _id;

        private List<IEnumerator> _iesDebug = new List<IEnumerator>();

        private EasyDraw _easyDrawDebug;
        private IEnumerator _hitEnemyRoutine;
        private IEnumerator _goToPointRoutine;

        private AnimationSprite _ledSprite;
        private AnimationSprite _ledOffSprite;

        private DroneFollowRangeCone _droneFollowRangeCone;

        private IEnumerator _blinkLedRoutine;
        
        private int _returnToStartAfterChasing = 0;

        private IEnumerator _endLevelRoutine;

        private Vector2 _distanceToTarget;
        private IOnUpdateListener[] _onUpdateListeners = new IOnUpdateListener[0];
        
        private float[] pitchInterval = new float[]
        {
            1, 1.2f
        };

        private int _frameNumber;
        private static int _FrameSpeed = 20;
        private int _frameTimer;
        
        public DroneGameObject(float pX, float pY, float pWidth, float pHeight, float pSpeed = 200,
            float pRotation = 0) : base(
            "data/Drone spritesheet small.png", 2, 2, 4, false, true)
        {
            _id = ++IdCounter;
            name = $"{this}_{_id}";

            _customColliderBounds = new Rectangle(-27, -24, 53, 50);

            _maxSpeed = pSpeed;

            float originalWidth = width;
            float originalHeight = height;

            SetOrigin(0, height);

            float lScaleX = pWidth / width;
            float lScaleY = pHeight / height;

            SetScaleXY(lScaleX, lScaleY);

            x = pX; // + width / 2;
            y = pY; // - height + height / 2f;

            SetOrigin(originalWidth / 2f, originalHeight / 2f);

            Turn(pRotation);

            x = pX + Mathf.Cos(rotation.DegToRad()) * width * 0.5f;
            y = pY + Mathf.Sin(rotation.DegToRad()) * width * 0.5f;

            var pos = new Vector2(x - pX, y - pY);
            var perp = new Vector2(pos.y, -pos.x).Normalized;

            pos = perp * height * 0.5f;

            SetScaleXY(1, 1);

            x += pos.x;
            y += pos.y;

            _startPosition = new Vector2(x, y);

            _ledSprite = new AnimationSprite("data/Drone White Led.png", 1, 1, -1, false, false);
            _ledSprite.SetOrigin(width * 0.5f, height * 0.5f);

            _ledOffSprite = new AnimationSprite("data/Drone Gray Led.png", 1, 1, -1, false, false);
            _ledOffSprite.SetOrigin(width * 0.5f, height * 0.5f);

            _ledSprite.SetColor(0, 1f, 0);

            AddChild(_ledOffSprite);
            AddChild(_ledSprite);

            _droneFollowRangeCone = new DroneFollowRangeCone(this);
            _droneFollowRangeCone.SetColor(0.9f, 0.9f, 0);
            _droneFollowRangeCone.alpha = 0;
            AddChild(_droneFollowRangeCone);

            _easyDrawDebug = new EasyDraw(200, 80, false);
            _easyDrawDebug.SetOrigin(0, _easyDrawDebug.height * 0.5f);
            _easyDrawDebug.Clear(Color.Black);
            AddChild(_easyDrawDebug);
            _easyDrawDebug.TextFont("data/Gaiatype.ttf", 8);
            _easyDrawDebug.x = 0;
            _easyDrawDebug.y = -40;

            CoroutineManager.StartCoroutine(WaitForEnemyLoad(), this);
        }

        IEnumerator WaitForEnemyLoad()
        {
            while (_enemy == null)
            {
                yield return null;
            }

            _searchingRoutine = CoroutineManager.StartCoroutine(SearchingRoutine(DroneState.SEARCHING_ENEMY), this);
            _iesDebug.Add(_searchingRoutine);
        }

        private IEnumerator SearchingRoutine(DroneState pState)
        {
            _state = pState;

            var waitingMovementRoutine = WaitingMovementRoutine();

            if (_state == DroneState.SEARCHING_ENEMY)
            {
                Console.WriteLine($"{this.name} SearchingRoutine | {Time.time}");

                _ledSprite.SetColor(0, 1, 0);
                _blinkLedRoutine = CoroutineManager.StartCoroutine(BlinkLed(), this);
                
                CoroutineManager.StartCoroutine(waitingMovementRoutine, this);
                _iesDebug.Add(waitingMovementRoutine);
            }

            while (_state == DroneState.SEARCHING_ENEMY ||
                   _state == DroneState.RETURN_TO_START_POINT_AFTER_CHASING_AND_SEARCHING_ENEMY)
            {
                _distanceToTarget = _enemy.Pos - this._pos;
                
                float dist = _distanceToTarget.Magnitude;
                
                if (dist < _detectEnemyRange)
                {
                    //Enemy Detected
                    CoroutineManager.StopCoroutine(waitingMovementRoutine);
                    CoroutineManager.StopCoroutine(_returnToStartPointRoutine);
                    CoroutineManager.StopCoroutine(_goToPointRoutine);

                    _iesDebug.Remove(_returnToStartPointRoutine);
                    _iesDebug.Remove(_goToPointRoutine);

                    _enemyDetectedRoutine = CoroutineManager.StartCoroutine(EnemyDetectedRoutine(), this);
                    _iesDebug.Add(_enemyDetectedRoutine);
                }

                yield return null;
            }

            _iesDebug.Remove(_searchingRoutine);
            _iesDebug.Remove(waitingMovementRoutine);
        }

        private IEnumerator EnemyDetectedRoutine()
        {
            _state = DroneState.ENEMY_DETECTED;

            yield return null;

            CoroutineManager.StopCoroutine(_blinkLedRoutine);
            _ledSprite.alpha = 1;
            _ledSprite.SetColor(1, 0, 0);

            Console.WriteLine($"{this.name} EnemyDetectedRoutine | {Time.time}");

            if (_hasDroneBehaviourListener)
            {
                _droneBehaviorListener.OnEnemyDetected(this, _enemy);
            }

            yield return new WaitForMilliSeconds(500);

            _ledSprite.alpha = 1;

            _iesDebug.Remove(_enemyDetectedRoutine);

            _chasingRoutine = CoroutineManager.StartCoroutine(ChasingRoutine(_enemy), this);
            _iesDebug.Add(_chasingRoutine);
        }

        private IEnumerator WaitingMovementRoutine()
        {
            //Random small movement

            while (_state == DroneState.SEARCHING_ENEMY)
            {
                var point = _startPosition + MRandom.OnUnitCircle() * width * 0.5f;

                var distance = point - _pos;
                float distanceMag = distance.Magnitude;

                while (distanceMag > 20)
                {
                    var distanceNorm = distance.Normalized;

                    _currentSpeed = (_state == DroneState.SEARCHING_ENEMY) ? _waitingSpeed : _maxSpeed;

                    var nextPos = distanceNorm * _currentSpeed;

                    //Console.WriteLine($"{this}: {x:0.00} | {y:0.00} | dist: {distanceMag:0.00} | startpos: {_startPosition} | point: {point} | speed: {speed} | nextPos: {nextPos}");

                    Translate(nextPos.x * Time.delta, nextPos.y * Time.delta);

                    yield return null;

                    distance = point - _pos;
                    distanceMag = distance.Magnitude;
                }
            }
        }

        private IEnumerator ChasingRoutine(GameObject target)
        {
            _state = DroneState.CHASING_ENEMY;

            var chasingMoveRoutine = ChasingMovementRoutine(target);
            _iesDebug.Add(chasingMoveRoutine);

            SpriteTweener.TweenAlpha(_droneFollowRangeCone, 0, 0.4f, 500);

            yield return chasingMoveRoutine;

            SpriteTweener.TweenAlpha(_droneFollowRangeCone, 0.4f, 0, 500);

            _iesDebug.Remove(chasingMoveRoutine);
            _iesDebug.Remove(_chasingRoutine);
        }

        IEnumerator ChasingMovementRoutine(GameObject target)
        {
            var distanceNorm = new Vector2();
            float distanceMag = 0;

            while (_state == DroneState.CHASING_ENEMY && distanceMag < _stopChasingRange)
            {
                _distanceToTarget = target.Pos - _pos;
                distanceNorm = _distanceToTarget.Normalized;
                distanceMag = _distanceToTarget.Magnitude;

                _currentSpeed = (_state == DroneState.SEARCHING_ENEMY) ? _waitingSpeed : _maxSpeed;

                var nextPos = distanceNorm * _currentSpeed;

                Translate(nextPos.x * Time.deltaTime * 0.001f, nextPos.y * Time.deltaTime * 0.001f);

                yield return null;
            }

            if (_state == DroneState.CHASING_ENEMY && distanceMag >= _stopChasingRange)
            {
                //Return to start point
                ReturnToStartPointAfterChasing();
            }
        }

        void OnCollision(GameObject other)
        {
            if (_hasDroneBehaviourListener)
            {
                _droneBehaviorListener.OnEnemyCollision(this, other);
            }
        }

        public void DroneHitEnemy()
        {
            _state = DroneState.HIT_ENEMY;
            _hitEnemyRoutine = CoroutineManager.StartCoroutine(HitEnemyRoutine(), this);

            _iesDebug.Add(_hitEnemyRoutine);
        }

        private IEnumerator HitEnemyRoutine()
        {
            _state = DroneState.HIT_ENEMY;

            yield return new WaitForMilliSeconds(500);

            ReturnToStartPoint();

            _iesDebug.Remove(_hitEnemyRoutine);
        }

        private void ReturnToStartPointAfterChasing()
        {
            _state = DroneState.RETURN_TO_START_POINT_AFTER_CHASING_AND_SEARCHING_ENEMY;
            _returnToStartPointRoutine = CoroutineManager.StartCoroutine(ReturnToStartPointRoutine(), this);

            _returnToStartAfterChasing++;

            if (_returnToStartAfterChasing == 1)
            {
                LocalEvents.Instance.Raise(new LevelLocalEvent(_enemy, (GameObject) this, null,
                    LevelLocalEvent.EventType.STORK_GET_POINTS_EVADE_DRONE));
            }

            _searchingRoutine = CoroutineManager.StartCoroutine(SearchingRoutine(_state), this);

            _iesDebug.Add(_returnToStartPointRoutine);
            _iesDebug.Add(_searchingRoutine);
        }

        private void ReturnToStartPoint()
        {
            _state = DroneState.RETURN_TO_START_POINT_AFTER_HIT;
            _returnToStartPointRoutine = CoroutineManager.StartCoroutine(ReturnToStartPointRoutine(), this);

            _iesDebug.Add(_returnToStartPointRoutine);
        }

        private IEnumerator ReturnToStartPointRoutine()
        {
            _goToPointRoutine = GoToPoint(_startPosition);

            _iesDebug.Add(_goToPointRoutine);

            yield return _goToPointRoutine;

            _iesDebug.Remove(_goToPointRoutine);
            _iesDebug.Remove(_returnToStartPointRoutine);

            yield return new WaitForMilliSeconds(1000);

            CoroutineManager.StopCoroutine(_searchingRoutine);
            _iesDebug.Remove(_searchingRoutine);

            _searchingRoutine = CoroutineManager.StartCoroutine(SearchingRoutine(DroneState.SEARCHING_ENEMY), this);
            _iesDebug.Add(_searchingRoutine);
        }

        public void EndLevel()
        {
            _state = DroneState.END_LEVEL;

            DroneBehaviorListener = null;

            CoroutineManager.StopAllCoroutines(this);

            CoroutineManager.StartCoroutine(GoToPoint(_startPosition), this);
        }
        
        void Update()
        {
            if (!this.Enabled) return;

            if (_frameTimer >= _FrameSpeed)
            {
                SetFrame(++_frameNumber % frameCount);
                _frameTimer = 0;
            }

            _frameTimer += Time.deltaTime;

            for (int i = 0; i < _onUpdateListeners.Length; i++)
            {
                _onUpdateListeners[i].OnUpdate(this, 1);
            }

            if (_state == DroneState.END_LEVEL)
            {
                _distanceToTarget = _enemy.Pos - _pos;
            }
            
            _easyDrawDebug.visible = MyGame.Debug;

            if (MyGame.Debug)
            {
                // if (_id == 1)
                // {
                //     Console.WriteLine($"========================");
                //     Console.WriteLine($"========================");
                //     Console.WriteLine($"\t{string.Join(Environment.NewLine + "\t", _iesDebug)}");
                // }

                _easyDrawDebug.Clear(Color.FromArgb(200, 1, 1, 1));
                _easyDrawDebug.Fill(Color.White);
                _easyDrawDebug.Stroke(Color.Aquamarine);

                string str = $"state: {_state.ToString()}";

                _easyDrawDebug.Text(str, 4, 30);

                CanvasDebugger2.Instance.DrawEllipse(x, y, _detectEnemyRange * 2, _detectEnemyRange * 2, Color.Brown);
                CanvasDebugger2.Instance.DrawEllipse(x, y, _stopChasingRange * 2, _stopChasingRange * 2,
                    Color.DarkGreen);
                DrawBoundBox();
            }
        }

        public IEnumerator GoToPoint(Vector2 point)
        {
            var distance = new Vector2();
            var distanceDirection = new Vector2();
            float distanceMag = float.MaxValue;

            while (distanceMag > 30)
            {
                distance = point - _pos;
                distanceDirection = distance.Normalized;

                _currentSpeed = (_state == DroneState.SEARCHING_ENEMY) ? _waitingSpeed : _maxSpeed;
                var nextPos = distanceDirection * _currentSpeed;

                Translate(nextPos.x * Time.delta, nextPos.y * Time.delta);

                yield return null;

                distanceMag = distance.Magnitude;
            }
        }

        public void SetState(DroneState pState)
        {
            _state = pState;
        }

        IEnumerator BlinkLed()
        {
            int time = 0;
            int duration = 400;

            _ledSprite.alpha = 0;

            while (true)
            {
                if (time < duration)
                {
                    _ledSprite.alpha = Easing.Ease(Easing.Equation.QuadEaseOut, time, 0, 1, duration);
                }
                else
                {
                    _ledSprite.alpha = Easing.Ease(Easing.Equation.QuadEaseOut, time - duration, 1, 0 - 1, duration);
                }

                time += Time.deltaTime;

                time = time % (2 * duration);
                yield return null;
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

        public DroneState State => _state;

        public float WaitingSpeed
        {
            get => _waitingSpeed;
            set => _waitingSpeed = value;
        }

        public float DetectEnemyRange
        {
            get => _detectEnemyRange;
            set => _detectEnemyRange = value;
        }

        public GameObject Enemy
        {
            get => _enemy;
            set => _enemy = value;
        }

        public IDroneBehaviorListener DroneBehaviorListener
        {
            get => _droneBehaviorListener;
            set
            {
                _droneBehaviorListener = value;
                _hasDroneBehaviourListener = value != null;
            }
        }

        public uint Id => _id;

        Vector2 IHasDistanceToTarget.Distance => _distanceToTarget;

        GameObject IHasDistanceToTarget.gameObject => this;
        
        public IOnUpdateListener[] OnUpdateListeners
        {
            get => _onUpdateListeners;
            set => _onUpdateListeners = value;
        }

        public static int FrameSpeed
        {
            get => _FrameSpeed;
            set => _FrameSpeed = value;
        }
    }

    internal class DroneFollowRangeCone : Sprite
    {
        private DroneGameObject _drone;

        public DroneFollowRangeCone(DroneGameObject pDrone) : base("data/Drone Follow Range Cone.png", false, false)
        {
            _drone = pDrone;
            SetOrigin(0, height * 0.5f);
        }

        void Update()
        {
            if (_drone.State != DroneGameObject.DroneState.CHASING_ENEMY)
            {
                return;
            }

            var targetPos = new Vector2(_drone.Enemy.x, _drone.Enemy.y);
            var direction = targetPos - _drone.Pos;
            var directionNorm = direction.Normalized;

            float angle = Mathf.Atan2(directionNorm.y, directionNorm.x);

            this.rotation = angle.RadToDegree();
        }
    }

    public interface IDroneBehaviorListener
    {
        void OnEnemyDetected(DroneGameObject drone, GameObject enemy);
        void OnEnemyCollision(DroneGameObject drone, GameObject enemy);
    }
}