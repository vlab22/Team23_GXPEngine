using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class HunterGameObject : AnimationSprite, IHasDistanceToTarget
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
            END_LEVEL
        }

        private HunterState _state;

        private IHunterBehaviorListener[] _hunterBehaviorListeners;

        private float _scanEnemyRange;
        private float _sightSpeed;
        private HunterCrossHairGameObject _crossHair;
        private EasyDraw _easyDrawDebug;

        private Stork _enemy;

        private IEnumerator _scanningForEnemyRoutine;
        private IEnumerator _enemyDetectedRoutine;
        private IEnumerator _lockingCrossHairOnEnemy;
        private IEnumerator _snapCrosshairOnEnemyRoutine;
        private IEnumerator _lostLockOnEnemyOutOfRangeRoutine;
        private IEnumerator _countDownToShootRoutine;
        private IEnumerator _shootAtEnemyRoutine;

        private Vector2 _aimDistance;

        private HunterFollowRangeCone _hunterFollowRangeCone;
        private Vector2 _distanceToTarget;

        public HunterGameObject(float pX, float pY, float pWidth, float pHeight,
            float pSightSpeed = 200) : base("data/Hunter.png", 1, 1,
            -1, false, false)
        {
            _scanEnemyRange = pWidth;
            _sightSpeed = pSightSpeed;

            _hunterBehaviorListeners = new IHunterBehaviorListener[0];

            SetOrigin(0, height);

            x = pX + pWidth * 0.5f;
            y = pY - pHeight * 0.5f;

            SetOrigin(width * 0.5f, height * 0.5f);

            _crossHair = new HunterCrossHairGameObject(this);
            AddChild(_crossHair);
            _crossHair.SetXY(0, 0);
            _crossHair.alpha = 0;

            _hunterFollowRangeCone = new HunterFollowRangeCone(this);
            _hunterFollowRangeCone.SetColor(0.9f, 0.9f, 0);
            _hunterFollowRangeCone.alpha = 0;
            AddChild(_hunterFollowRangeCone);

            _easyDrawDebug = new EasyDraw(200, 80, false);
            _easyDrawDebug.SetOrigin(0, _easyDrawDebug.height * 0.5f);
            //_easyDrawDebug.Clear(Color.Black);
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

            _scanningForEnemyRoutine =
                CoroutineManager.StartCoroutine(ScanningForEnemy(HunterState.SCANNING, true), this);
        }

        IEnumerator ScanningForEnemy(HunterState pState, bool resetCrossHairPosition)
        {
            _state = pState;

            _crossHair.Reset(resetCrossHairPosition);

            float distanceMag = float.MaxValue;
            do
            {
                _distanceToTarget = _enemy.Pos - _pos;
                distanceMag = _distanceToTarget.Magnitude;

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

            SoundManager.Instance.SetFxVolume(4, 0.5f);
            
            yield return new WaitForMilliSeconds(500);

            //Start Lock enemy

            _lockingCrossHairOnEnemy = CoroutineManager.StartCoroutine(LockingCrossHairOnEnemy(), this);
        }

        private IEnumerator LockingCrossHairOnEnemy()
        {
            _state = HunterState.LOCKING_CROSSHAIR_ON_ENEMY;

            _crossHair.visible = true;

            SpriteTweener.TweenAlpha(_hunterFollowRangeCone, 0, 0.4f, 500);

            //Seeking target
            do
            {
                var crossHairWorldPos = TransformPoint(_crossHair.Pos.x, _crossHair.Pos.y);
                var distance = _enemy.Pos - crossHairWorldPos;
                var distanceNorm = distance.Normalized;

                var nextPos = distanceNorm * _sightSpeed * Time.delta;

                _crossHair.Translate(nextPos.x, nextPos.y);

                if (!IsEnemyInRange())
                {
                    _lostLockOnEnemyOutOfRangeRoutine =
                        _lostLockOnEnemyOutOfRangeRoutine =
                            CoroutineManager.StartCoroutine(LostLockOnEnemyOutOfRangeRoutine(_enemy), this);
                }

                yield return null;
            } while (_state == HunterState.LOCKING_CROSSHAIR_ON_ENEMY);
        }

        private IEnumerator SnapCrossHairToEnemy(GameObject enemy)
        {
            _state = HunterState.CROSSHAIR_LOCKED_ON_ENEMY;

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

            SpriteTweener.TweenColor(_crossHair, _crossHair.StartColor, ColorTools.ColorToUInt(Color.Red), 300);

            //After Snap, follow target while distance less than range
            //Countdown to Shoot
            _countDownToShootRoutine = CoroutineManager.StartCoroutine(CountDownToShootRoutine(enemy), this);

            do
            {
                _distanceToTarget = enemy.Pos - _pos;
                float distanceMag = _distanceToTarget.Magnitude;

                localPos = InverseTransformPoint(enemy.x, enemy.y);
                _crossHair.SetXY(localPos.x, localPos.y);

                //Lost lock on enemy, out of range
                if (distanceMag > _scanEnemyRange)
                {
                    CoroutineManager.StopCoroutine(_countDownToShootRoutine);

                    _lostLockOnEnemyOutOfRangeRoutine =
                        CoroutineManager.StartCoroutine(LostLockOnEnemyOutOfRangeRoutine(enemy), this);
                }

                yield return null;
            } while (_state == HunterState.CROSSHAIR_LOCKED_ON_ENEMY);
        }

        private IEnumerator CountDownToShootRoutine(GameObject enemy)
        {
            yield return new WaitForMilliSeconds(500);

            int counter = 3;

            do
            {
                yield return new WaitForMilliSeconds(1000);

                counter--;
            } while (counter > 0 && _state == HunterState.CROSSHAIR_LOCKED_ON_ENEMY);

            //Shoot
            if (counter == 0 && _state == HunterState.CROSSHAIR_LOCKED_ON_ENEMY)
            {
                _shootAtEnemyRoutine = CoroutineManager.StartCoroutine(ShootAtEnemyRoutine(enemy), this);
            }
        }

        private IEnumerator ShootAtEnemyRoutine(GameObject enemy)
        {
            _state = HunterState.SHOOT;

            SoundManager.Instance.PlayFx(3);
            
            Console.WriteLine($"{this}: SHOOT!!!");

            for (int i = 0; i < _hunterBehaviorListeners.Length; i++)
            {
                _hunterBehaviorListeners[i].OnShootAtEnemy(this, _aimDistance, enemy);
            }

            //Cooldown
            _state = HunterState.RECOVER_FROM_SHOOT;
            yield return new WaitForMilliSeconds(1500);

            if (IsEnemyInRange())
            {
                _scanningForEnemyRoutine =
                    CoroutineManager.StartCoroutine(ScanningForEnemy(HunterState.SCANNING, false), this);
            }
            else
            {
                _lostLockOnEnemyOutOfRangeRoutine =
                    CoroutineManager.StartCoroutine(LostLockOnEnemyOutOfRangeRoutine(enemy), this);
            }
        }

        private IEnumerator LostLockOnEnemyOutOfRangeRoutine(GameObject enemy)
        {
            _state = HunterState.LOST_LOCK_ON_ENEMY;

            SpriteTweener.TweenAlpha(_hunterFollowRangeCone, 0.4f, 0, 500);

            SpriteTweener.TweenAlpha(_crossHair, 1, 0, 400);

            yield return new WaitForMilliSeconds(1000);

            //Return to scanning state
            _scanningForEnemyRoutine =
                CoroutineManager.StartCoroutine(ScanningForEnemy(HunterState.SCANNING, true), this);
        }

        public void EndLevel()
        {
            _state = HunterState.END_LEVEL;
            
            HunterBehaviorListeners = null;

            CoroutineManager.StopAllCoroutines(this);

            CoroutineManager.StartCoroutine(EndLevelRoutine(), this);
            
        }

        private IEnumerator EndLevelRoutine()
        {
            SpriteTweener.TweenAlpha(_crossHair, 1, 0, 400);

            SpriteTweener.TweenAlpha(_hunterFollowRangeCone, _hunterFollowRangeCone.alpha, 0, 400);
            
            yield return null;
        }

        void Update()
        {
            if (!this.Enabled) return;

            _aimDistance = TransformPoint(_crossHair.Pos.x, _crossHair.Pos.y) - _pos;

            _easyDrawDebug.SetActive(MyGame.Debug);

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
            //Snap to enemy
            _snapCrosshairOnEnemyRoutine = CoroutineManager.StartCoroutine(SnapCrossHairToEnemy(enemy), this);
        }

        bool IsEnemyInRange()
        {
            return DistanceTo(_enemy) <= _scanEnemyRange;
        }

        public IHunterBehaviorListener[] HunterBehaviorListeners
        {
            get => _hunterBehaviorListeners;
            set { _hunterBehaviorListeners = value; }
        }

        public Stork Enemy
        {
            get => _enemy;
            set => _enemy = value;
        }

        public HunterState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public float ScanEnemyRange => _scanEnemyRange;

        public Vector2 Distance => _distanceToTarget;
        
        GameObject IHasDistanceToTarget.gameObject => this;
    }

    internal class HunterFollowRangeCone : Sprite
    {
        private HunterGameObject _hunter;

        public HunterFollowRangeCone(HunterGameObject pHunter) : base("data/Drone Follow Range Cone.png", false, false)
        {
            _hunter = pHunter;
            SetOrigin(0, height * 0.5f);

            float scaleX = _hunter.ScanEnemyRange / 435f;
            SetScaleXY(scaleX, 1);
        }

        void Update()
        {
            if (_hunter.State != HunterGameObject.HunterState.LOCKING_CROSSHAIR_ON_ENEMY &&
                _hunter.State != HunterGameObject.HunterState.CROSSHAIR_LOCKED_ON_ENEMY)
            {
                return;
            }

            var targetPos = new Vector2(_hunter.Enemy.x, _hunter.Enemy.y);
            var direction = targetPos - _hunter.Pos;
            var directionNorm = direction.Normalized;

            float angle = Mathf.Atan2(directionNorm.y, directionNorm.x);

            this.rotation = angle.RadToDegree();
        }
    }

    public interface IHunterBehaviorListener
    {
        void OnShootAtEnemy(HunterGameObject hunter, Vector2 aimDistance, GameObject enemy);
    }
}