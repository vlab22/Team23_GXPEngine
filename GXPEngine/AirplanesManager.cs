using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class AirplanesManager : GameObject
    {
        private Level _level;

        private Airplane[] _airplanesPool;
        private int _airplanesPoolIndex;
        private MapGameObject _map;

        private Dictionary<Airplane, TiledObject> _airplanesMap;

        private int _firstAirplaneIndex;

        public AirplanesManager(Level pLevel) : base(false)
        {
            _level = pLevel;

            _map = _level.Map;

            _airplanesPool = new Airplane[20];

            for (int i = 0; i < _airplanesPool.Length; i++)
            {
                var airplane = new Airplane();

                _level.AddChild(airplane);

                _firstAirplaneIndex = airplane.Index;

                airplane.SetActive(false);

                _airplanesPool[i] = airplane;
            }

            _airplanesMap = new Dictionary<Airplane, TiledObject>();
        }

        public void SpawnAirplanes()
        {
            //Load Airplanes
            var airPlanesObjects = _map.ObjectGroup.Objects.Where(o => o.Name.StartsWith("airplane ")).ToArray();

            for (int i = 0; i < airPlanesObjects.Length; i++)
            {
                SpawnAirplane(airPlanesObjects[i]);
            }
        }

        private void SpawnAirplane(TiledObject airData)
        {
            float airSpeed = airData.GetFloatProperty("speed", 200);
            int lifeTime = (int) (airData.GetFloatProperty("life_time", 12) * 1000);
            int spawnFrequency = (int) (airData.GetFloatProperty("spawn_frequency_time", 0) * 1000);

            var airplane = GetAirplaneFromPool();

            airplane.LoadStartupData(airData.X, airData.Y, airData.Width, airData.Height, _level, airSpeed,
                airData.rotation, lifeTime);

            airplane.SetActive(true);

            if (lifeTime > 0)
            {
                CoroutineManager.StartCoroutine(DespawnAirplaneAfterLifeTime(airplane, lifeTime), this);
            }

            if (spawnFrequency > 0)
            {
                CoroutineManager.StartCoroutine(SpawnAirplaneAfterDelay(airData, spawnFrequency), this);
            }

            _airplanesMap.Add(airplane, airData);
        }

        private IEnumerator SpawnAirplaneAfterDelay(TiledObject airData, int spawnFrequency)
        {
            if (spawnFrequency > 0)
            {
                yield return new WaitForMilliSeconds(spawnFrequency);
                
                SpawnAirplane(airData);
            }
        }

        private IEnumerator DespawnAirplaneAfterLifeTime(Airplane airplane, int lifeTime)
        {
            yield return new WaitForMilliSeconds(lifeTime);

            DespawnAirplane(airplane);

            _airplanesMap.Remove(airplane);
        }

        void DespawnAirplane(Airplane airplane)
        {
            airplane.SetActive(false);
        }

        Airplane GetAirplaneFromPool()
        {
            var airplane = _airplanesPool[_airplanesPoolIndex];
            _airplanesPoolIndex = ++_airplanesPoolIndex % _airplanesPool.Length;

            return airplane;
        }

        public int FirstAirplaneIndex => _firstAirplaneIndex;
    }
}