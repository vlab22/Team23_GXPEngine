using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;
using MathfExtensions;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class DroneGameObject : AnimationSprite
    {
        public enum DroneState
        {
            WAITING,
            ENEMY_DETECTED,
            CHASING_ENEMY,
            STOP_CHASING_ENEMY,
            RETURN_TO_START_POINT,
        }

        private DroneState _state;

        private float _maxSpeed;
        private float _currentMaxSpeed;
        private float _currentSpeed;
        private float _waitingSpeed = 32;
        private Rectangle _customColliderBounds;

        private Vector2 _startPosition;

        private IEnumerator _waitingRoutine;

        private Vector2 _forward;
        private Vector2 _pos;

        private IEnumerator _chasingRoutine;
        private float _detectEnemyRange = 335;
        private float _stopChasingRange = 435;

        private IEnumerator _enemyDetectedRoutine;
        
        private GameObject _enemy;

        private IDroneBehaviorListener _droneBehaviorListener;
        private bool _hasDroneBehaviourListener;

        public DroneGameObject(float pX, float pY, float pWidth, float pHeight, float pSpeed = 200,
            float pRotation = 0) : base(
            "data/Drone spritesheet small.png", 3, 3, 5, false, true)
        {
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

            float diag = Mathf.Sqrt(width * width + height * height);

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
            
            _waitingRoutine = CoroutineManager.StartCoroutine(WaitingRoutine());
        }

        private IEnumerator WaitingRoutine()
        {
            var waitingMovementRoutine = WaitingMovementRoutine();
            CoroutineManager.StartCoroutine(waitingMovementRoutine);

            _state = DroneState.WAITING;
            
            while (_state == DroneState.WAITING)
            {
                if (_enemy == null)
                {
                    yield return null;
                }

                float dist = DistanceTo(_enemy);
                if (dist < _detectEnemyRange)
                {
                    //Enemy Detected
                    CoroutineManager.StopCoroutine(waitingMovementRoutine);

                    _enemyDetectedRoutine = CoroutineManager.StartCoroutine(EnemyDetectedRoutine());
                }
                
                yield return null;
            }
        }

        private IEnumerator EnemyDetectedRoutine()
        {
            _state = DroneState.ENEMY_DETECTED;
            
            if (_hasDroneBehaviourListener)
            {
                _droneBehaviorListener.OnEnemyDetected(this, _enemy);
            }
            
            yield return new WaitForMilliSeconds(500);

            _chasingRoutine = CoroutineManager.StartCoroutine(ChasingRoutine(_enemy));
        }

        private IEnumerator WaitingMovementRoutine()
        {
            _pos.x = x;
            _pos.y = y;

            //Random small movement

            while (_state == DroneState.WAITING)
            {
                var point = _startPosition + MRandom.OnUnitCircle() * width * 0.5f;

                var distance = point - _pos;
                float distanceMag = distance.Magnitude;

                while (distanceMag > 20)
                {
                    var distanceNorm = distance.Normalized;

                    float speed = (_state == DroneState.WAITING) ? _waitingSpeed : _maxSpeed;

                    var nextPos = distanceNorm * speed;

                    //Console.WriteLine($"{this}: {x:0.00} | {y:0.00} | dist: {distanceMag:0.00} | startpos: {_startPosition} | point: {point} | speed: {speed} | nextPos: {nextPos}");

                    Translate(nextPos.x * Time.deltaTime * 0.001f, nextPos.y * Time.deltaTime * 0.001f);

                    yield return null;

                    _pos.x = x;
                    _pos.y = y;

                    distance = point - _pos;
                    distanceMag = distance.Magnitude;
                }
            }
        }

        private IEnumerator ChasingRoutine(GameObject target)
        {
            _pos.x = x;
            _pos.y = y;

            _state = DroneState.CHASING_ENEMY;
            
            yield return ChasingMovementRoutine(target);

            _state = DroneState.STOP_CHASING_ENEMY;
            
            yield return new WaitForMilliSeconds(3000);

            _waitingRoutine = CoroutineManager.StartCoroutine(WaitingRoutine());
        }

        IEnumerator ChasingMovementRoutine(GameObject target)
        {
            var targetPos = new Vector2();
            var distance = new Vector2();
            var distanceNorm = new Vector2();
            float distanceMag = 0;
            
            while (_state == DroneState.CHASING_ENEMY && distanceMag < _stopChasingRange)
            {
                targetPos.x = target.x;
                targetPos.y = target.y;
                
                distance = targetPos - _pos;
                distanceNorm = distance.Normalized;
                distanceMag = distance.Magnitude;

                float speed = (_state == DroneState.WAITING) ? _waitingSpeed : _maxSpeed;

                var nextPos = distanceNorm * speed;

                Translate(nextPos.x * Time.deltaTime * 0.001f, nextPos.y * Time.deltaTime * 0.001f);

                yield return null;
            }
        }

        void OnCollision(GameObject other)
        {
            
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

        public IDroneBehaviorListener droneBehaviorListener
        {
            get => _droneBehaviorListener;
            set
            {
                _droneBehaviorListener = value;
                _hasDroneBehaviourListener = value != null;
            }
        }

        void Update()
        {
            _pos.x = x;
            _pos.y = y;

            if (MyGame.Debug)
            {
                CanvasDebugger2.Instance.DrawEllipse(x, y, _detectEnemyRange * 2, _detectEnemyRange * 2, Color.Brown);
                CanvasDebugger2.Instance.DrawEllipse(x, y, _stopChasingRange * 2, _stopChasingRange * 2, Color.DarkGreen);
                DrawBoundBox();
            }
        }

        void ChaseTarget(Vector2 point)
        {
            float delta = Time.deltaTime * 0.001f;

            _pos.x = x;
            _pos.y = y;

            var distance = point - _pos;

            float distanceMag = distance.Magnitude;
            if (distanceMag <= 25)
            {
                //Stop Chasing
                _state = DroneState.STOP_CHASING_ENEMY;
                return;
            }

            var distanceNorm = distance.Normalized;

            float speed = (_state == DroneState.WAITING) ? _waitingSpeed : _maxSpeed;

            var nextPos = _pos + distanceNorm * speed;

            Translate(nextPos.x * delta, nextPos.y * delta);
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
    }

    public interface IDroneBehaviorListener
    {
        void OnEnemyDetected(DroneGameObject drone, GameObject enemy);
    }
}