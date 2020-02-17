using System.Collections;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
    public class HunterGameObject : AnimationSprite
    {
        public enum HunterState
        {
            SCANNING,
            ENEMY_DETECTED,
            LOCKING_CROSSHAIR_ON_ENEMY,
            CROSSHAIR_LOCKED_ON_ENEMY,
            SHOOT,
            RECOVER_FROM_SHOOT,
        }

        private HunterState _state; 
        
        private IHunterBehaviorListener _hunterBehaviorListener;
        private bool _hasHunterBehaviourListener;

        private float _scanEnemyRange;
        private float _sightSpeed;
        private HunterCrossHairGameObject _crossHair;
        private EasyDraw _easyDrawDebug;

        private Stork _enemy;

        private Vector2 _pos;

        private IEnumerator _scanningForEnemyRoutine;
        private IEnumerator _enemyDetectedRoutine;
        private IEnumerator _lockingCrossHairOnEnemy;

        public HunterGameObject(float pX, float pY, float pWidth, float pHeight,
            float pScanEnemyRange = 400, float pSightSpeed = 300) : base("data/Female Hunter.png", 1, 1,
            -1, false, false)
        {
            _scanEnemyRange = pScanEnemyRange;
            _sightSpeed = pSightSpeed;
            
            float originalWidth = width;
            float originalHeight = height;

            SetOrigin(0, height);

            x = pX + pWidth * 0.5f;
            y = pY - pHeight * 0.5f;

            SetOrigin(width * 0.5f, height * 0.5f);

            _crossHair = new HunterCrossHairGameObject(this);
            AddChild(_crossHair);
            _crossHair.SetXY(0,0);
            //_crossHair.visible = false;

            _easyDrawDebug = new EasyDraw(200, 80, false);
            _easyDrawDebug.SetOrigin(0, _easyDrawDebug.height * 0.5f);
            _easyDrawDebug.Clear(Color.Black);
            AddChild(_easyDrawDebug);
            _easyDrawDebug.TextFont("data/Gaiatype.ttf", 8);
            _easyDrawDebug.x = 0;
            _easyDrawDebug.y = -40;

            CoroutineManager.StartCoroutine(WaitForEnemySet(), this);
        }

        private IEnumerator WaitForEnemySet()
        {
            while (_enemy == null)
            {
                yield return null;
            }
            
            yield return null;

            _scanningForEnemyRoutine = CoroutineManager.StartCoroutine(ScanningForEnemy(HunterState.SCANNING), this);
        }

        IEnumerator ScanningForEnemy(HunterState pState)
        {
            _state = pState;
            
            float distanceMag = float.MaxValue;
            do
            {
                var distance = _enemy.Pos - _pos;
                distanceMag = distance.Magnitude;

                if (distanceMag < _scanEnemyRange)
                {
                    //Change State to Enemy Detected
                    _enemyDetectedRoutine = CoroutineManager.StartCoroutine(EnemyDetectedRoutine(), this);
                }
                
                yield return null;
                
            } while (_state == HunterState.SCANNING);
        }

        private IEnumerator EnemyDetectedRoutine()
        {
            _state = HunterState.ENEMY_DETECTED;
            
            yield return new WaitForMilliSeconds(500);
            
            //Start Lock enemy

            _lockingCrossHairOnEnemy = CoroutineManager.StartCoroutine(LockingCrossHairOnEnemy(), this);
        }

        private IEnumerator LockingCrossHairOnEnemy()
        {
            _state = HunterState.LOCKING_CROSSHAIR_ON_ENEMY;

            _crossHair.visible = true;
            
            do
            {
                var crossHairWorldPos = TransformPoint(_crossHair.Pos.x, _crossHair.Pos.y);
                var distance = _enemy.Pos - crossHairWorldPos;
                var distanceNorm = distance.Normalized;

                var nextPos = distanceNorm * _sightSpeed * Time.delta;
                
                _crossHair.Translate(nextPos.x, nextPos.y);
                
                yield return null;
            } while (_state == HunterState.LOCKING_CROSSHAIR_ON_ENEMY);
        }

        public IHunterBehaviorListener HunterBehaviorListener
        {
            get => _hunterBehaviorListener;
            set
            {
                _hunterBehaviorListener = value;
                _hasHunterBehaviourListener = value != null;
            }
        }

        void Update()
        {
            if (!this.Enabled) return;

            _pos.x = x;
            _pos.y = y;
            
            if (MyGame.Debug)
            {
                _easyDrawDebug.Clear(Color.FromArgb(200, 1, 1, 1));
                _easyDrawDebug.Fill(Color.White);
                _easyDrawDebug.Stroke(Color.Aquamarine);

                string str = $"state: {_state.ToString()}";

                _easyDrawDebug.Text(str, 4, 30);

                CanvasDebugger2.Instance.DrawEllipse(x, y, _scanEnemyRange * 2, _scanEnemyRange * 2, Color.Brown);
            }
        }

        public void OnCollisionWithEnemy(GameObject other)
        {
        }
        
        public Stork Enemy
        {
            get => _enemy;
            set => _enemy = value;
        }

        public Vector2 Pos => _pos;
    }

    public interface IHunterBehaviorListener
    {
    }
}