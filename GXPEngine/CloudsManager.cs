using System;
using System.Collections.Generic;
using System.Linq;

namespace GXPEngine
{
    public class CloudsManager : GameObject
    {
        private MapGameObject _map;
        private List<CloudGameObject> _clouds;
        private Level _level;

        private int[] _verticalCloudsIds = new int[]
        {
            48,
            65, 66,
            81, 82,
            97, 98,
            113, 114
        };

        private int[] _horizontalCloudsIds = new int[]
        {
            21, 22, 23, 24,
            36, 37, 38, 39, 40,
        };

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
                        cloud = new CloudGameObject(MRandom.Range(-1, 1));
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

                    //Check if a cloud is incorrectly drew in Tiled and than "ERASE" the tile,
                    //a cloud need the "pivot" tile (20 for horizontal clouds, 50 for vertical clouds)
                    //Ugly solution, hard coded

                    int tileCheck;
                    if (tile >= 21 && tile <= 24)
                    {
                        int gap = tile - 20;

                        if (gap > -1)
                        {
                            tileCheck = cloudsTileArray[col - gap, row] - cloudsLayerFirstGid;
                            if (tileCheck != 20)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 21");
                            }
                        }
                    }
                    else if (tile >= 36 && tile <= 40)
                    {
                        int gapX = tile - 36;
                        int gapY = 1;

                        if (gapX > -1 && gapY > -1)
                        {
                            tileCheck = cloudsTileArray[col - gapX, row - gapY] - cloudsLayerFirstGid;
                            if (tileCheck != 20)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 37");
                            }
                        }
                    }
                    else if (tile == 49)
                    {
                        int gapX = 50 - tile;
                        if (gapX < _map.MapData.Height)
                        {
                            tileCheck = cloudsTileArray[col + gapX, row] - cloudsLayerFirstGid;
                            if (tileCheck != 50)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 50");
                            }
                        }
                    }
                    else if (tile >= 65 && tile <= 66)
                    {
                        int gapX = 66 - tile;
                        int gapY = 1;

                        if (gapX < _map.MapData.Height && gapY > -1)
                        {
                            tileCheck = cloudsTileArray[col + gapX, row - gapY] - cloudsLayerFirstGid;
                            if (tileCheck != 50)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 50, 65");
                            }
                        }
                    }
                    else if (tile >= 81 && tile <= 82)
                    {
                        int gapX = 82 - tile;
                        int gapY = 2;

                        if (gapX < _map.MapData.Height && gapY > -1)
                        {
                            tileCheck = cloudsTileArray[col + gapX, row - gapY] - cloudsLayerFirstGid;
                            if (tileCheck != 50)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 50, 81");
                            }
                        }
                    }
                    else if (tile >= 97 && tile <= 98)
                    {
                        int gapX = 98 - tile;
                        int gapY = 3;

                        if (gapX < _map.MapData.Height && gapY > -1)
                        {
                            tileCheck = cloudsTileArray[col + gapX, row - gapY] - cloudsLayerFirstGid;
                            if (tileCheck != 50)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 50, 97");
                            }
                        }
                    }
                    else if (tile >= 113 && tile <= 114)
                    {
                        int gapX = 114 - tile;
                        int gapY = 4;

                        if (gapX < _map.MapData.Height && gapY > -1)
                        {
                            tileCheck = cloudsTileArray[col + gapX, row - gapY] - cloudsLayerFirstGid;
                            if (tileCheck != 49)
                            {
                                _map.SetCloudsTileArray(col, row, 0);
                                Console.WriteLine($"{this}: {col}|{row} cloud tile erased for 50, 113");
                            }
                        }
                    }
                }
            }
        }
    }
}