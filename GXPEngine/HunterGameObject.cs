using System;
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
            LOST_LOCK_ON_ENEMY,
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
            _crossHair.SetXY(0, 0);
            _crossHair.alpha = 0;

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

            _crossHair.Reset();

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

            SpriteTweener.TweenAlpha(_crossHair, 0, 1, 400);
            
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

        private IEnumerator SnapCrossHairToEnemy(GameObject enemy)
        {
            int time = 0;
            int duration = 300;

            var crossStartPos = TransformPoint(_crossHair.Pos.x, _crossHair.Pos.y);
            Vector2 localPos;
            
            while (time < duration)
            {
                float pointX = Easing.Ease(Easing.Equation.QuadEaseOut, time, crossStartPos.x,
                    enemy.x - crossStartPos.x, duration);
                float pointY = Easing.Ease(Easing.Equation.QuadEaseOut, time, crossStartPos.y,
                    enemy.y - crossStartPos.y, duration);

                localPos = InverseTransformPoint(pointX, pointY);

                _crossHair.SetXY(localPos.x, localPos.y);

                time += Time.deltaTime;
                yield return null;
            }

            //After Snap, follow target while distance less than range
            //Countdown to Shoot
            
            SpriteTweener.TweenColor(_crossHair, _crossHair.StartColor, ColorTools.ColorToUInt(Color.Red), 300);

            do
            {
                var distance = enemy.Pos - _pos;
                float distanceMag = distance.Magnitude;
                
                localPos = InverseTransformPoint(enemy.x, enemy.y);
                _crossHair.SetXY(localPos.x, localPos.y);
                
                //Lost lock on enemy, out of range
                if (distanceMag > _scanEnemyRange)
                {
                    CoroutineManager.StartCoroutine(LostLockOnEnemyOutOfRangeRoutine(enemy), this);
                }
                
                yield return null;
            } while (_state == HunterState.CROSSHAIR_LOCKED_ON_ENEMY);
        }

        private IEnumerator LostLockOnEnemyOutOfRangeRoutine(GameObject enemy)
        {
            _state = HunterState.LOST_LOCK_ON_ENEMY;

            SpriteTweener.TweenAlpha(_crossHair, 1, 0, 400);

            _crossHair.color = _crossHair.StartColor;

            yield return new WaitForMilliSeconds(1000);
            
            //Return to scanning state
            _scanningForEnemyRoutine = CoroutineManager.StartCoroutine(ScanningForEnemy(HunterState.SCANNING), this);
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

        public void OnCollisionWithEnemy(GameObject enemy)
        {
            _state = HunterState.CROSSHAIR_LOCKED_ON_ENEMY;

            //Snap to enemy
            CoroutineManager.StartCoroutine(SnapCrossHairToEnemy(enemy), this);
        }

        public Stork Enemy
        {
            get => _enemy;
            set => _enemy = value;
        }
    }

    public interface IHunterBehaviorListener
    {
    }
}