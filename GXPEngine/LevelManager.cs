using System.Collections;
using System.Linq;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class LevelManager : GameObject
    {
        private Level _level;

        private int _endLayerIndex;
        
        public LevelManager(Level pLevel)
        {
            _level = pLevel;

            _endLayerIndex = _level.Map.GetLayerIndex("end point layer");

            CoroutineManager.StartCoroutine(SpawnAirplanes());
        }

        private IEnumerator SpawnAirplanes()
        {
            while (!_level.Destroyed)
            {
                yield return new WaitForMilliSeconds(10000);
                
                _level.SpawnAirplanes();
            }
            
            //Add more yields here
        }

        void Update()
        {
            
        }
    }
}