using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine.GameLocalEvents;

namespace GXPEngine
{
    public class DroneManager : GameObject, IDroneBehaviorListener
    {
        Level _level;
        List<DroneGameObject> _drones;

        public DroneManager(Level pLevel) : base(false)
        {
            _level = pLevel;
            _drones = new List<DroneGameObject>();
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
    }
}