namespace GXPEngine.GameLocalEvents
{
    public class StorkLocalEvent : GameLocalEvent
    {
        public enum Event
        {
            STORK_HIT_BY_PLANE,
            STORK_AFTER_HIT_BY_PLANE,
            STORK_LOSE_PIZZA
        }
        
        public Stork stork;
        public Event evt;

        public StorkLocalEvent(Stork pStork, Event pEvt)
        {
            this.stork = pStork;
            this.evt = pEvt;
        }
    }
}