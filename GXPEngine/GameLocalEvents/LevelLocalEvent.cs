using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine.GameLocalEvents
{
    public class LevelLocalEvent : GameLocalEvent
    {
        public enum EventType
        {
            LEVEL_START_COUNTER_START,
            LEVEL_START_COUNTER_END,
            DRONE_DETECTED_ENEMY,
            DRONE_HIT_ENEMY
        }

        public Level level;
        public Stork stork;
        public DroneGameObject drone;
        public int index;
        public Color color;
        public EventType evt;

        public LevelLocalEvent(Level pLevel, EventType pEvt)
        {
            this.level = pLevel;
            this.evt = pEvt;
        }
        
        public LevelLocalEvent(int pIndex, Stork pStork, Color pColor, Level pLevel, EventType pEvt) : this(pLevel, pEvt)
        {
            stork = pStork;
            index = pIndex;
            color = pColor;
        }
        
        public LevelLocalEvent(GameObject player, DroneGameObject pDrone, Level pLevel, EventType pEvt) : this(pLevel, pEvt)
        {
            stork = player as Stork;
            drone = pDrone;
        }
    }
}