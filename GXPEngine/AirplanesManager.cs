namespace GXPEngine
{
    public class AirplanesManager : GameObject
    {
        private Level _level;
        
        private Airplane[] _airplanesPool;
        
        public AirplanesManager(Level pLevel) : base(false)
        {
            _level = pLevel;
        }
    }
}