using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class CloudsManager : GameObject
    {
        private MapGameObject _map;
        private List<CloudGameObject> _clouds;
        private Level _level;

        public CloudsManager(MapGameObject pMap, Level pLevel) : base(false)
        {
            _map = pMap;
            _level = pLevel;
            _clouds = new List<CloudGameObject>();
        }

        public void SpawnClouds()
        {
            var cloudsTileArray = _map.GetCloudsTileArray();
            var cloudsLayerFirstGid = _map.MapData.TileSets.FirstOrDefault(ts => ts.Name == "Clouds Tileset").FirstGId;
            
            for (int col = 0; col < cloudsTileArray.GetLength(0); col++)
            {
                for (int row = 0; row < cloudsTileArray.GetLength(1); row++)
                {
                    CloudGameObject cloud;
                    var tile = cloudsTileArray[col, row] - cloudsLayerFirstGid;
                    
                    if (tile == 20 || tile == 50)
                    {
                        //Instantiate a horizontal cloud
                        cloud = new CloudGameObject();
                        _level.AddChild(cloud);

                        float px = (col) * _map.MapData.TileWidth;
                        float py = (row) * _map.MapData.TileHeight;
                        
                        cloud.SetXY(px, py);

                        if (tile == 50)
                        {
                            cloud.SetXY(px + _map.MapData.TileWidth, py);
                            cloud.Turn(90);
                        }
                        
                        _clouds.Add(cloud);
                    }
                    
                }
            }
        }
    }
}