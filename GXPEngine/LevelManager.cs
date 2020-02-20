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
        }
    }
}