namespace GXPEngine.GameLocalEvents
{
    public class StorkLocalEvent : GameLocalEvent
    {
        public enum Event
        {
            STORK_HIT_BY_PLANE,
            STORK_AFTER_HIT_BY_DRONE,
            STORK_AFTER_HIT_BY_PLANE,
            STORK_LOSE_PIZZA,
            STORK_HIT_BY_DRONE,
            STORK_HIT_BY_HUNTER, 
        }
        
        public Stork stork;
        public GameObject enemy;
        public Event evt;

        public StorkLocalEvent(Stork pStork, Event pEvt)
        {
            this.stork = pStork;
            this.evt = pEvt;
        }
        
        public StorkLocalEvent(Stork pStork, GameObject pEnemy, Event pEvt)
        {
            this.stork = pStork;
            this.enemy = pEnemy;
            this.evt = pEvt;
        }
    }
}