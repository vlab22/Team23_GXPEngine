using System.Collections;
using System.Linq;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class LevelManager : GameObject
    {
        private Level _level;
        
        public LevelManager(Level pLevel)
        {
            _level = pLevel;
            
            CoroutineManager.StartCoroutine(SpawnAirplanes(), this);
        }

        private IEnumerator SpawnAirplanes()
        {
            while (!_level.Destroyed)
            {
                yield return new WaitForMilliSeconds(15000);
                
                _level.SpawnAirplanes();
            }
            
            //Add more yields here
        }
    }
}