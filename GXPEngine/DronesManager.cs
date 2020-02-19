using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;

namespace GXPEngine
{
    public class DronesManager : GameObject, IDroneBehaviorListener
    {
        Level _level;
        List<DroneGameObject> _drones;
        private IEnumerator _droneReleasePizzaRoutine;

        public DronesManager(Level pLevel) : base(false)
        {
            _level = pLevel;
            _drones = new List<DroneGameObject>();

            //CoroutineManager.StartCoroutine(Foo(), this);
        }

        private IEnumerator Foo()
        {
            yield return new WaitForMilliSeconds(6000);

            var drone = _drones.FirstOrDefault();
            drone?.SetActive(false);
            
            yield return new WaitForMilliSeconds(3000);

            drone?.SetActive(true);
        }

        public void SpawnDrones()
        {
            //Load Drones
            var droneObjects = _level.Map.ObjectGroup.Objects.Where(o => o.Name.StartsWith("drone")).ToArray();

            for (int i = 0; i < droneObjects.Length; i++)
            {
                var droneData = droneObjects[i];

                float droneSpeed = droneData.GetFloatProperty("speed", 100);

                var drone = new DroneGameObject(droneData.X, droneData.Y, droneData.Width, droneData.Height, droneSpeed,
                    droneData.rotation);

                drone.DroneBehaviorListener = this;

                _drones.Add(drone);

                _level.AddChild(drone);
            }

            for (int i = _drones.Count() - 1; i > -1; i--)
            {
                if (_drones[i].Destroyed)
                {
                    _drones.RemoveAt(i);
                }
            }

            Console.WriteLine($"{this}: drone count: {droneObjects.Length}");
        }

        public void SetDronesTarget(GameObject target)
        {
            for (int i = 0; i < _drones.Count; i++)
            {
                var drone = _drones[i];
                if (drone.Destroyed)
                    continue;

                drone.Enemy = target;
            }
        }

        void IDroneBehaviorListener.OnEnemyDetected(DroneGameObject drone, GameObject enemy)
        {
            LocalEvents.Instance.Raise(new LevelLocalEvent(enemy, drone, _level,
                LevelLocalEvent.EventType.DRONE_DETECTED_ENEMY));
        }

        void IDroneBehaviorListener.OnEnemyCollision(DroneGameObject drone, GameObject enemy)
        {
            if (enemy is Stork)
            {
                if (drone.State != DroneGameObject.DroneState.HIT_ENEMY &&
                    drone.State != DroneGameObject.DroneState.RETURN_TO_START_POINT_AFTER_HIT &&
                    drone.State != DroneGameObject.DroneState.ENEMY_DETECTED)
                {
                    CoroutineManager.StartCoroutine(DroneHitEnemyRoutine(drone, enemy), this);
                }
            }
        }

        IEnumerator DroneHitEnemyRoutine(DroneGameObject drone, GameObject enemy)
        {
            drone.SetState(DroneGameObject.DroneState.HIT_ENEMY);

            LocalEvents.Instance.Raise(new LevelLocalEvent(enemy, drone, _level,
                LevelLocalEvent.EventType.DRONE_HIT_ENEMY));

            //Stole pizza animation
            yield return StolePizzaAnimationRoutine(drone, enemy);

            drone.DroneHitEnemy();

            yield return null;
        }

        private IEnumerator StolePizzaAnimationRoutine(DroneGameObject drone, GameObject enemy)
        {
            CoroutineManager.StopCoroutine(_droneReleasePizzaRoutine);
            
            var pizza = _level.GetPizzaFromPool();

            pizza.SetScaleXY(1, 1);
            pizza.SetXY(enemy.x, enemy.y);
            pizza.alpha = 1;
            pizza.visible = true;

            int firstAirplaneIndex = _level.FirstAirplaneIndex;

            _level.SetChildIndex(pizza, firstAirplaneIndex - 1);

            //Animate pizza

            float fromX = enemy.x;
            float fromY = enemy.y;

            float offsetX = 0;
            float offsetY = -25;

            int time = 0;
            int duration = 450;
            while (time < duration && !drone.Destroyed)
            {
                pizza.x = Easing.Ease(Easing.Equation.CubicEaseOut, time, fromX, drone.x + offsetX - fromX, duration);
                pizza.y = Easing.Ease(Easing.Equation.CubicEaseOut, time, fromY, drone.y + offsetY - fromY, duration);

                time += Time.deltaTime;
                yield return null;
            }

            pizza.parent = drone;
            pizza.SetXY(offsetX, offsetY);

            _droneReleasePizzaRoutine = CoroutineManager.StartCoroutine(DroneReleasePizzaRoutine(drone, (Sprite) pizza), this);
        }

        IEnumerator DroneReleasePizzaRoutine(DroneGameObject drone, Sprite pizza)
        {
            while (drone.State == DroneGameObject.DroneState.HIT_ENEMY || drone.State == DroneGameObject.DroneState.RETURN_TO_START_POINT_AFTER_HIT)
            {
                yield return null;
            }
            
            int duration = 1000;

            SpriteTweener.TweenAlpha(pizza, 1, 0, duration);
            
            yield return new WaitForMilliSeconds(duration);

            pizza.visible = false;
            pizza.alpha = 1;
            pizza.parent = _level;
        }
    }
}