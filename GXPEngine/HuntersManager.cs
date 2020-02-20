using System.Collections.Generic;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine
{
    public class HuntersManager : GameObject, IHunterBehaviorListener
    {
        private Level _level;
        private List<HunterGameObject> _hunters;

        private HunterBulletManager _bulletManager;

        public HuntersManager(Level pLevel, HunterBulletManager pHunterBulletManager) : base(false)
        {
            _level = pLevel;
            _bulletManager = pHunterBulletManager;
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

                float sightSpeed = hunterData.GetFloatProperty("sight_speed", 200);

                var hunter = new HunterGameObject(hunterData.X, hunterData.Y, hunterData.Width, hunterData.Height,
                    sightSpeed);

                hunter.HunterBehaviorListeners =
                    hunter.HunterBehaviorListeners.Concat(new IHunterBehaviorListener[] {this}).ToArray();

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

        public void EndLevelAllHunters()
        {
            for (int i = 0; i < _hunters.Count; i++)
            {
                var hunter = _hunters[i];

                if (!hunter.Enabled) continue;

                hunter.EndLevel();
            }
        }
        
        public void SetHuntersTarget(Stork stork)
        {
            for (int i = 0; i < _hunters.Count; i++)
            {
                _hunters[i].Enemy = stork;
            }
        }

        void IHunterBehaviorListener.OnShootAtEnemy(HunterGameObject hunter, Vector2 aimDistance, GameObject enemy)
        {
            _bulletManager.SpawnBullet(hunter.x, hunter.y, aimDistance, hunter);
        }
    }
}