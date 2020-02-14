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
        }

        void Update()
        {
            
        }
    }
}