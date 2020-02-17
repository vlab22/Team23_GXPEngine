using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class HuntersManager : GameObject, IHunterBehaviorListener
    {
        private Level _level;
        private List<HunterGameObject> _hunters;

        public HuntersManager(Level pLevel) : base(false)
        {
            _level = pLevel;
            _hunters = new List<HunterGameObject>();
        }

        public List<HunterGameObject> Hunters => _hunters;

        public void SpawnHunters()
        {
            //Load Hunters
            var huntersObjects = _level.Map.ObjectGroup.Objects.Where(o => o.Name.StartsWith("hunter")).ToArray();

            for (int i = 0; i < huntersObjects.Length; i++)
            {
                var hunterData = huntersObjects[i];

                float scanRange = hunterData.GetFloatProperty("scan_range", 400);
                float sightSpeed = hunterData.GetFloatProperty("sight_speed", 300);

                var hunter = new HunterGameObject(hunterData.X, hunterData.Y, hunterData.Width, hunterData.Height,
                    scanRange, sightSpeed);

                hunter.HunterBehaviorListener = this;

                _hunters.Add(hunter);

                _level.AddChild(hunter);
            }

            for (int i = _hunters.Count() - 1; i > -1; i--)
            {
                if (_hunters[i].Destroyed)
                {
                    _hunters.RemoveAt(i);
                }
            }
        }

        public void SetHuntersTarget(Stork stork)
        {
            for (int i = 0; i < _hunters.Count; i++)
            {
                _hunters[i].Enemy = stork;
            }
        }
    }
}