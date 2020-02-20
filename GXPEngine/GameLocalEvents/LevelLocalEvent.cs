using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine.GameLocalEvents
{
    public class LevelLocalEvent : GameLocalEvent
    {
        public enum EventType
        {
            NONE,
            LEVEL_START_COUNTER_START,
            LEVEL_START_COUNTER_END,
            DRONE_DETECTED_ENEMY,
            DRONE_HIT_PLAYER,
            STORK_GET_POINTS_EVADE_DRONE,
            HUNTER_HIT_PLAYER
        }

        public Level level;
        public Stork stork;
        public DroneGameObject drone;
        public GameObject gameObj;
        public int index;
        public Color color;
        public EventType evt = EventType.NONE;

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
        
        public LevelLocalEvent(GameObject player, GameObject pGObj, Level pLevel, EventType pEvt) : this(pLevel, pEvt)
        {
            stork = player as Stork;
            gameObj = pGObj;
        }
    }
}