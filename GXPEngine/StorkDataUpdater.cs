using System.Collections.Generic;
using GXPEngine.Core;

namespace GXPEngine
{
    public class StorkDataUpdater : IGridDataUpdater
    {
        private MapGameObject _map;
        
        private Dictionary<Stork, GridData> _storksGridDataMap;

        public StorkDataUpdater(MapGameObject pMap)
        {
            _map = pMap;
            _storksGridDataMap = new Dictionary<Stork, GridData>();
        }
        
        public void AddStork(Stork stork)
        {
            if (!_storksGridDataMap.ContainsKey(stork))
            {
                _storksGridDataMap.Add(stork, new GridData());
            }
        }
        
        void IGridDataUpdater.OnMove(Vector2 pos, Vector2 lastPos)
        {
            
        }
    }

    public struct GridData
    {
        public int currentTile;
        public int currentTileX;
        public int currentTileY;
        
        public int lastCurrentTile;
        public int lastCurrentTileX;
        public int lastCurrentTileY;
    }
}