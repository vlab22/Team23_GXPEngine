﻿using System;
using System.Collections;
using System.Linq;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;

namespace GXPEngine
{
    public class StorkStateManager : IColliderListener
    {
        private Stork _stork;

        private Level _level;

        private bool _inCollisionWithDeliveryPoint;

        private bool _inCollisionWithAirplane;
        private Airplane _lastAirplaneCollided;

        private bool _inCollisionWithDrone;
        private DroneGameObject _lastDroneCollided;

        private Sprite[] _pizzasPool;
        private int _pizzaPoolIndex = 0;

        public StorkStateManager(Stork pStork, Level pLevel)
        {
            _stork = pStork;
            _level = pLevel;
            _stork.ColliderListener = this;

            _pizzasPool = new Sprite[5];
            for (int i = 0; i < _pizzasPool.Length; i++)
            {
                var pizza = new PizzaGameObject("data/pizza00.png");
                pizza.visible = false;
                _level.AddChild(pizza);

                _pizzasPool[i] = pizza;
            }
        }

        void IColliderListener.OnCollisionWith(GameObject go, GameObject other)
        {
            if (!_inCollisionWithDeliveryPoint && other is DeliveryPoint)
            {
                //Get to end

                //Drop the pizza to the center of the delivery point
                var dropPoint = new Vector2(other.x, other.y);
                CoroutineManager.StartCoroutine(DropPizzaRoutine(dropPoint));

                _inCollisionWithDeliveryPoint = true;
            }
            else if (!_inCollisionWithAirplane && other is CompoundCollider && other.parent is Airplane parent &&
                     parent != _lastAirplaneCollided)
            {
                _inCollisionWithAirplane = true;
                _lastAirplaneCollided = parent;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithAirplaneRoutine(parent));
            }
            else if (!_inCollisionWithDrone && other is DroneGameObject drone && drone != _lastDroneCollided)
            {
                _lastDroneCollided = drone;
                _inCollisionWithDrone = true;

                //Lose Pizza
                CoroutineManager.StartCoroutine(CollisionWithDroneRoutine(drone));
            }
        }

        private IEnumerator CollisionWithDroneRoutine(DroneGameObject drone)
        {
            //_stork.InputEnabled = false;

            //Shake Camera
            MyGame.ThisInstance.Camera.shakeDuration = 500;

            LocalEvents.Instance.Raise(new StorkLocalEvent(_stork, StorkLocalEvent.Event.STORK_HIT_BY_PLANE));

            yield return new WaitForMilliSeconds(200);

            //Drop a pizza
            yield return DropPizzaRoutine(_stork.Pos);

            yield return new WaitForMilliSeconds(2500);

            //_stork.InputEnabled = true;
            _inCollisionWithDrone = false;
            _lastDroneCollided = null;
        }

        private IEnumerator CollisionWithAirplaneRoutine(Airplane plane)
        {
            _stork.InputEnabled = false;

            //Shake Camera
            MyGame.ThisInstance.Camera.shakeDuration = 500;

            LocalEvents.Instance.Raise(new StorkLocalEvent(_stork, StorkLocalEvent.Event.STORK_HIT_BY_PLANE));

            yield return new WaitForMilliSeconds(500);

            //Drop a pizza
            yield return DropPizzaRoutine(_stork.Pos);

            yield return new WaitForMilliSeconds(1500);

            _stork.InputEnabled = true;
            _inCollisionWithAirplane = false;
        }

        public IEnumerator DropPizzaRoutine(Vector2 dropPoint)
        {
            yield return new WaitForMilliSeconds(400);

            Console.WriteLine($"{this}: droppoint: {dropPoint}");

            //Get pizza game object
            var pizza = _pizzasPool[_pizzaPoolIndex];
            _pizzaPoolIndex++;
            _pizzaPoolIndex %= _pizzasPool.Length;

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

            while (time < duration)
            {
                //TODO: parei aqui, apremder os easing
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
                }

                Console.WriteLine($"scaleX: {scaleX}");

                pizza.SetScaleXY(scaleX, scaleY);

                time += Time.deltaTime;
                scaleTime += Time.deltaTime;
                yield return null;
            }

            ParticleManager.Instance.PlaySmallSmoke(_level, toX, toY);
        }
    }
}