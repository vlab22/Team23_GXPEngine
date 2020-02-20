using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;
using MathfExtensions;

namespace GXPEngine
{
    public class StorkManager : GameObject, IColliderListener, IGridDataUpdater
    {
        private Stork _stork;

        private Level _level;
        private MapGameObject _map;

        private bool _inCollisionWithDeliveryPoint;

        private bool _inCollisionWithAirplane;
        private Airplane _lastAirplaneCollided;

        private bool _inCollisionWithDrone;
        private DroneGameObject _lastDroneCollided;

        private GameObject _lastMarker;

        private bool _inCollisionWithBullet;
        private HunterBullet _lastBulletCollided;

        private EasyDraw _storkOutOfMapCover;

        public StorkManager(Stork pStork, Level pLevel) : base(false)
        {
            _stork = pStork;
            _level = pLevel;
            _map = _level.Map;
            _stork.ColliderListener = this;

            _lastMarker = _level.GetChildren(false).Where(o => o is DeliveryPoint).LastOrDefault();

            _storkOutOfMapCover = new EasyDraw(_stork.width, _stork.height, false);
            _storkOutOfMapCover.SetOrigin(_stork.width * 0.5f, _stork.height * 0.5f);
            _storkOutOfMapCover.Clear(Color.White);
            _storkOutOfMapCover.SetActive(false);
            AddChild(_storkOutOfMapCover);
        }

        void Update()
        {
            if (!Enabled) return;


            // var posForward = _stork.Pos + _stork.Forward * _stork.width * 0.5f;
            // var posBack = _stork.Pos - _stork.Forward * _stork.width * 0.5f;
            //
            // int tileIndexForward = _map.GetTileIdFromWorld(0, posForward.x, posForward.y);
            // int tileIndexBack= _map.GetTileIdFromWorld(0, posBack.x, posBack.y);
            // Vector2 pF = _map.WorldToTilePoint(posForward.x, posForward.y);
            // Vector2 pB = _map.WorldToTilePoint(posBack.x, posBack.y);
            //
            //
            // //If stork is out of map, draw a white canvas over ir
            // if (tileIndexForward == -1 || tileIndexBack == -1)
            // {
            //     _storkOutOfMapCover.SetActive(true);
            //     _storkOutOfMapCover.SetXY(_stork.x, _stork.y);
            // }

            //Console.WriteLine($"{this}: tileIndex: {tileIndexForward} | pF: {pF} | tileBack: {tileIndexBack} | pB: {pB}");
        }

        void IColliderListener.OnCollisionWith(GameObject go, GameObject other)
        {
            if (!_inCollisionWithDeliveryPoint && other is DeliveryPoint)
            {
                if (!_level.IsLevelEnding)
                {
                    //Pizza delivered
                    LocalEvents.Instance.Raise(new LevelLocalEvent(_level, LevelLocalEvent.EventType.PIZZA_DELIVERED));

                    //Drop the pizza to the center of the delivery point
                    var dropPoint = new Vector2(other.x, other.y);
                    CoroutineManager.StartCoroutine(DropPizzaRoutine(dropPoint), this);
                }

                _inCollisionWithDeliveryPoint = true;

                CoroutineManager.StartCoroutine(WaitDeliveryPointBeDisabled(other), this);
            }
            else if (!_inCollisionWithAirplane && other is CompoundCollider && other.parent is Airplane parent &&
                     parent != _lastAirplaneCollided)
            {
                _inCollisionWithAirplane = true;
                _lastAirplaneCollided = parent;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithAirplaneRoutine(parent), this);
            }

            if (!_inCollisionWithDrone && other is DroneGameObject drone && drone != _lastDroneCollided &&
                drone.State != DroneGameObject.DroneState.RETURN_TO_START_POINT_AFTER_HIT)
            {
                _lastDroneCollided = drone;
                _inCollisionWithDrone = true;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithDroneRoutine(drone), this);
            }

            if (!_inCollisionWithBullet && other is HunterBullet bullet && bullet != _lastBulletCollided)
            {
                _inCollisionWithBullet = true;

                if (bullet.IsCollisionEnabled) //When bullet is falling, ignore collision
                {
                    _lastBulletCollided = bullet;

                    CoroutineManager.StartCoroutine(CollisionWithHunterBulletRoutine(bullet), this);
                }
            }
        }

        private IEnumerator WaitDeliveryPointBeDisabled(GameObject other)
        {
            while (other.Enabled)
            {
                yield return null;
            }

            _inCollisionWithDeliveryPoint = false;
        }

        private IEnumerator CollisionWithHunterBulletRoutine(HunterBullet bullet)
        {
            //Shake Camera
            MyGame.ThisInstance.Camera.shakeDuration = 500;

            LocalEvents.Instance.Raise(new LevelLocalEvent(_stork, bullet.Shooter, _level,
                LevelLocalEvent.EventType.HUNTER_HIT_PLAYER));

            LocalEvents.Instance.Raise(new StorkLocalEvent(_stork, bullet.Shooter,
                StorkLocalEvent.Event.STORK_HIT_BY_HUNTER));

            //Drop a pizza
            CoroutineManager.StartCoroutine(DropPizzaRoutine(_stork.Pos - _stork.Forward * 40f, -1), this);

            yield return new WaitForMilliSeconds(1000);

            _inCollisionWithBullet = false;

            while (bullet.Enabled)
            {
                yield return null;
            }

            _lastBulletCollided = null;
        }

        private IEnumerator CollisionWithDroneRoutine(DroneGameObject drone)
        {
            //_stork.InputEnabled = false;

            //Shake Camera
            MyGame.ThisInstance.Camera.shakeDuration = 500;

            LocalEvents.Instance.Raise(new StorkLocalEvent(_stork, StorkLocalEvent.Event.STORK_HIT_BY_DRONE));

            yield return new WaitForMilliSeconds(2500);

            //_stork.InputEnabled = true;
            _inCollisionWithDrone = false;
            _lastDroneCollided = null;
        }

        private IEnumerator CollisionWithAirplaneRoutine(Airplane plane)
        {
            //_stork.InputEnabled = false;

            //Shake Camera
            MyGame.ThisInstance.Camera.shakeDuration = 500;

            LocalEvents.Instance.Raise(new StorkLocalEvent(_stork, StorkLocalEvent.Event.STORK_HIT_BY_PLANE));

            if (!_level.IsLevelEnding)
            {
                //Drop a pizza
                CoroutineManager.StartCoroutine(DropPizzaRoutine(_stork.Pos - _stork.Forward * 40f), this);
            }

            LocalEvents.Instance.Raise(new LevelLocalEvent(_stork, plane, _level,
                LevelLocalEvent.EventType.PLANE_HIT_PLAYER));

            yield return new WaitForMilliSeconds(1500);

            //_stork.InputEnabled = true;
            _inCollisionWithAirplane = false;
        }

        public IEnumerator DropPizzaRoutine(Vector2 dropPoint, int delay = 400)
        {
            if (delay > 0)
                yield return new WaitForMilliSeconds(delay);

            Console.WriteLine($"{this}: droppoint: {dropPoint}");

            //Get pizza game object
            var pizza = _level.GetPizzaFromPool();

            if (pizza == null)
            {
                yield break;
            }

            _level.SetChildIndex(pizza, _level.GetChildren(false).Count);
            pizza.visible = true;
            pizza.SetScaleXY(1, 1);
            var pizzaPos = _stork.Pos; // + _stork.Forward * 35;
            pizza.SetXY(pizzaPos.x, pizzaPos.y);

            var fromX = pizzaPos.x;
            var fromY = pizzaPos.y;

            var toX = dropPoint.x;
            var toY = dropPoint.y;

            int time = 0;
            int duration = 1800;

            pizza.SetScaleXY(1, 1);

            var fromScaleX = pizza.scaleX;
            var fromScaleY = pizza.scaleY;

            int scaleTime = 0;

            bool hasChangedDepthFlag = false;

            while (time < duration)
            {
                pizza.x = Easing.Ease(Easing.Equation.Linear, time, fromX, toX - fromX, duration);
                pizza.y = Easing.Ease(Easing.Equation.Linear, time, fromY, toY - fromY, duration);

                float scaleX = 0;
                float scaleY = 0;

                if (scaleTime < 400)
                {
                    scaleX = Easing.Ease(Easing.Equation.CubicEaseOut, scaleTime, fromScaleX, 1.5f - fromScaleX,
                        400);
                    scaleY = Easing.Ease(Easing.Equation.CubicEaseOut, scaleTime, fromScaleY, 1.5f - fromScaleY,
                        400);
                }
                else
                {
                    scaleX = Easing.Ease(Easing.Equation.CubicEaseIn, scaleTime - 400, 1.5f, 0f - 1.5f, duration - 400);
                    scaleY = Easing.Ease(Easing.Equation.CubicEaseIn, scaleTime - 400, 1.5f, 0f - 1.5f, duration - 400);

                    if (!hasChangedDepthFlag && scaleX < 1)
                    {
                        hasChangedDepthFlag = true;
                        pizza.parent.SetChildIndex(pizza, _lastMarker.Index + 1);
                    }
                }

                pizza.SetScaleXY(scaleX, scaleY);

                time += Time.deltaTime;
                scaleTime += Time.deltaTime;
                yield return null;
            }

            ParticleManager.Instance.PlaySmallSmoke(_level, toX, toY, 1500, _lastMarker.Index + 1);
        }

        void IGridDataUpdater.OnMove(Vector2 pos, Vector2 lastPos)
        {
        }

        void IGridDataUpdater.NextPosition(Vector2 pos, Vector2 nextPos)
        {
            int nextTileIndex = _map.GetBoundariesTileId(nextPos);

            if (nextTileIndex == -1)
            {
                _stork.IsMoveAllowed = false;

                bool isOutOfHorLimits = nextPos.x < 0 || nextPos.x > _map.MapWidthInPixels;
                bool isOutOfVerLimits = nextPos.y < 0 || nextPos.y > _map.MapHeightInPixels;

                if (isOutOfHorLimits && isOutOfVerLimits)
                {
                    //Do nothing, keeps stopped in the edges
                }
                else if (isOutOfHorLimits) //Correct vertical position

                {
                    float verticalSpeed = Mathf.Sin(_stork.rotation.DegToRad()) * ((IHasSpeed) _stork).Speed;

                    var perpVerticalVec = Vector2.up * verticalSpeed * Time.delta;

                    _stork.Translate(perpVerticalVec.x, perpVerticalVec.y);
                }
                else if (isOutOfVerLimits) //Correct horizontal position

                {
                    float horizontalSpeed = Mathf.Cos(_stork.rotation.DegToRad()) * ((IHasSpeed) _stork).Speed;

                    var perpHorizontalVec = Vector2.right * horizontalSpeed * Time.delta;

                    _stork.Translate(perpHorizontalVec.x, perpHorizontalVec.y);
                }
            }
            else
            {
                _stork.IsMoveAllowed = true;
            }
        }
    }
}