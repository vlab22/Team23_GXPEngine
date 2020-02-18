using System;
using System.Collections;
using System.Linq;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;

namespace GXPEngine
{
    public class StorkManager : IColliderListener
    {
        private Stork _stork;

        private Level _level;

        private bool _inCollisionWithDeliveryPoint;

        private bool _inCollisionWithAirplane;
        private Airplane _lastAirplaneCollided;

        private bool _inCollisionWithDrone;
        private DroneGameObject _lastDroneCollided;

        private GameObject _lastMarker;

        private bool _inCollisionWithBullet;

        public StorkManager(Stork pStork, Level pLevel)
        {
            _stork = pStork;
            _level = pLevel;
            _stork.ColliderListener = this;

            _lastMarker = _level.GetChildren(false).Where(o => o is DeliveryPoint).LastOrDefault();
        }

        void IColliderListener.OnCollisionWith(GameObject go, GameObject other)
        {
            if (!_inCollisionWithDeliveryPoint && other is DeliveryPoint)
            {
                //Get to end

                //Drop the pizza to the center of the delivery point
                var dropPoint = new Vector2(other.x, other.y);
                CoroutineManager.StartCoroutine(DropPizzaRoutine(dropPoint), null);

                _inCollisionWithDeliveryPoint = true;
            }
            else if (!_inCollisionWithAirplane && other is CompoundCollider && other.parent is Airplane parent &&
                     parent != _lastAirplaneCollided)
            {
                _inCollisionWithAirplane = true;
                _lastAirplaneCollided = parent;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithAirplaneRoutine(parent), null);
            }
            
            if (!_inCollisionWithDrone && other is DroneGameObject drone && drone != _lastDroneCollided)
            {
                _lastDroneCollided = drone;
                _inCollisionWithDrone = true;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithDroneRoutine(drone), null);
            }

            if (!_inCollisionWithBullet && other is HunterBullet)
            {
                _inCollisionWithBullet = true;
                
                CoroutineManager.StartCoroutine(CollisionWithHunterBulletRoutine(other), null);
            }
        }

        private IEnumerator CollisionWithHunterBulletRoutine(GameObject other)
        {
            
            
            yield return null;
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

            yield return new WaitForMilliSeconds(500);

            //Drop a pizza
            yield return DropPizzaRoutine(_stork.Pos);

            yield return new WaitForMilliSeconds(1500);

            //_stork.InputEnabled = true;
            _inCollisionWithAirplane = false;
        }

        public IEnumerator DropPizzaRoutine(Vector2 dropPoint)
        {
            yield return new WaitForMilliSeconds(400);

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

                // Console.WriteLine(
                //     $"{this} : time: {time} | fromX: {fromX:0.00} | fromY: {fromY:0.00} | pizza.x: {pizza.x} | pizza.y: {pizza.y}");

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
                        pizza.parent.SetChildIndex(pizza, _lastMarker.Index+1);
                    }
                }

                pizza.SetScaleXY(scaleX, scaleY);

                time += Time.deltaTime;
                scaleTime += Time.deltaTime;
                yield return null;
            }

            ParticleManager.Instance.PlaySmallSmoke(_level, toX, toY, 1500, _lastMarker.Index+1);
        }
    }
}