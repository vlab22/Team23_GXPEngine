using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using GXPEngine.Assets.Scripts.Tools;
using GXPEngine.Core;
using TiledMapParserExtended;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;

namespace GXPEngine
{
    public class MapGameObject : Canvas
    {
        /// <summary>
        /// MapData object parsed using TiledMapParserExtended
        /// </summary>
        private Map _mapData;

        /// <summary>
        /// Tileset images loaded from Tmx
        /// </summary>
        private Bitmap[] _tileSetImages;

        /// <summary>
        /// Used to draw tiles from TileSet, it's keep the rectangle and the tileset id
        /// </summary>
        private SpriteTileSetData[] _tileSetSpritesData;

        /// <summary>
        /// All the tiles layers in a mu;tidimensional array, example:
        /// [0][12,23] = 47 //means that the tile id in layer "0", at column "12" and row "23" is "47"
        /// This array is used to get the tile data of a layer at some position
        /// </summary>
        private short[][,] _tileArrays;

        /// <summary>
        /// Id of the layers, used across to make checks
        /// </summary>
        private int _bgLayer = -1;

        private ObjectGroup _objectGroup;

        public MapGameObject(Map mapData) : base(mapData.Width * mapData.TileWidth, mapData.Height * mapData.TileHeight, false)
        {
            _mapData = mapData;

            //Get the layers Index
            _bgLayer = GetLayerIndex("background image");
           
            
            //Load Object Group
            _objectGroup = _mapData.ObjectGroups[0];
            
            //Section to create the tiles Image and data to be drawn
            //The TilesSet Images are loaded and than used to draw each layer according with the layer data

            int totalTiles = mapData.TileSets.Sum(t => t.TileCount);
            _tileSetImages = new Bitmap[_mapData.TileSets.Length];
            _tileSetSpritesData = new SpriteTileSetData[totalTiles];
            int tileCounter = 0;
            
            for (int i = 0; i < _mapData.TileSets.Length; i++)
            {
                var watch = StopwatchTool.StartWatch();

                var tileSet = _mapData.TileSets[i];

                if (tileSet.TileCount > 0)
                {
                    int tileWidth = tileSet.TileWidth;
                    int tileHeight = tileSet.TileHeight;
                    int margin = tileSet.Margin;
                    int spacing = tileSet.Spacing;
                    int rows = tileSet.Rows;

                    var bm = new Bitmap(tileSet.Image.FileName);
                    _tileSetImages[i] = bm;

                    for (int j = 0; j < tileSet.TileCount; j++)
                    {
                        int row = j / tileSet.Columns;
                        int column = j % tileSet.Columns;

                        Rectangle cloneRect = new Rectangle(column * tileWidth + margin + spacing * (column),
                            row * tileHeight + margin + spacing * (row), tileWidth, tileHeight);
                        
                        //Create a data the keeps the ID and the rectangle where the sprite is in tileset
                        _tileSetSpritesData[tileCounter] = new SpriteTileSetData(i, cloneRect);
                        tileCounter++;
                    }

                    Console.WriteLine("tileSet.TileCount: " + tileSet.TileCount + " | rows: " + tileSet.Rows +
                                      " | columns: " + tileSet.Columns);

                    StopwatchTool.StopWatch(watch, tileSet.Name + " loaded: ");
                }
            }

            //Set the the layers data in a multidimensional array
            _tileArrays = new short[_mapData.Layers.Length][,];
            for (int i = 0; i < mapData.Layers.Length; i++)
            {
                _tileArrays[i] = _mapData.Layers[i].GetTileArray();
            }

            var drawWatch = StopwatchTool.StartWatch();

            DrawBackgroundLayer();

            StopwatchTool.StopWatch(drawWatch, "DrawBackgroundLayer: ");

            drawWatch = StopwatchTool.StartWatch();
            
            DrawLayers();
            
            StopwatchTool.StopWatch(drawWatch, "DrawLayers: ");
        }

        private void DrawBackgroundLayer()
        {
            var backGroundImageLayer = _mapData.ImageLayers.FirstOrDefault(l => l.Name == "background image");
            var bitmap = new Bitmap(backGroundImageLayer.Image.FileName);
            ImageAttributes imageAtt = new ImageAttributes();

            graphics.DrawImage(bitmap,
                new Rectangle(0, 0, bitmap.Width,
                    bitmap.Height), //_mapData.TileWidth, _mapData.TileHeight), // destination rectangle
                0, // source rectangle x 
                0, // source rectangle y
                bitmap.Width, //_mapData.TileWidth, // source rectangle width
                bitmap.Height, //_mapData.TileHeight, // source rectangle height
                GraphicsUnit.Pixel,
                imageAtt);
        }


        private void DrawLayers()
        {
            for (int i = 0; i < _mapData.Layers.Length; i++)
            {
                Layer layer = _mapData.Layers[i];
                if (layer.Visible)
                {
                    DrawLayer(i);
                }
            }
        }

        /// <summary>
        /// Draw a layer by index
        /// </summary>
        /// <param name="index"></param>
        public void DrawLayer(int index)
        {
            int tileSize = _mapData.Layers[index].GetIntProperty("tilesize", 32);
            int tileOrigin = _mapData.Layers[index].GetIntProperty("tileorigin", 0);

            // To draw mapData layer with opacity
            float[][] matrixItems =
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, _mapData.Layers[index].Opacity, 0},
                new float[] {0, 0, 0, 0, 1}
            };
            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

            // Create an ImageAttributes object and set its color matrix.
            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);

            //go through data and get the absolute ID, than with the absolute ID get the correspondent rectangle in tileset image
            int rows = _tileArrays[index].GetLength(0);
            int columns = _tileArrays[index].GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int tileIndex = _tileArrays[index][i, j] - 1;

                    if (tileIndex < 0)
                    {
                        continue;
                    }

                    var spriteData = _tileSetSpritesData[tileIndex];
                    var sourceRect = spriteData.rect;

                    var pt = new Point(i * _mapData.TileHeight, j * _mapData.TileWidth - tileOrigin);

                    graphics.DrawImage(_tileSetImages[spriteData.tileSetId],
                        new Rectangle(pt.X, pt.Y, tileSize,
                            tileSize), // destination rectangle
                        sourceRect.X, // source rectangle x 
                        sourceRect.Y, // source rectangle y
                        tileSize, // source rectangle width
                        tileSize, // source rectangle height
                        GraphicsUnit.Pixel,
                        imageAtt);
                }
            }
        }
        public Vector2 WorldToTilePoint(float x, float y)
        {
            int row = Mathf.Floor((y) / _mapData.TileHeight);
            int column = Mathf.Floor((x) / _mapData.TileWidth);

            return new Vector2(column, row);
        }

        public int WorldToTile(float x, float y)
        {
            if (x < 0 || x > _mapData.Height * _mapData.TileHeight || y < 0 || y > _mapData.Width * _mapData.TileWidth)
            {
                return -1;
            }

            int row = Mathf.Floor(y / _mapData.TileHeight);

            if (row < 0 || row >= _mapData.Height)
            {
                return -1;
            }

            int column = Mathf.Floor(x / _mapData.TileWidth);

            if (column < 0 || column >= _mapData.Width)
            {
                return -1;
            }

            return row * _mapData.Height + column;
        }

        public Vector2 TileToWorld(int tile)
        {
            int row = tile / _mapData.Height;
            int column = tile % _mapData.Width;

            return new Vector2(column * _mapData.TileWidth + _mapData.TileWidth * 0.5f,
                row * _mapData.TileHeight + _mapData.TileHeight * 0.5f);
        }

        public Vector2 TileToWorld(int column, int row)
        {
            return new Vector2(column * _mapData.TileWidth + _mapData.TileWidth * 0.5f,
                row * _mapData.TileHeight + _mapData.TileHeight * 0.5f);
        }

        public int GetTileIdFromWorld(int layer, float x, float y)
        {
            int row = Mathf.Floor(y / _mapData.TileHeight);

            if (row < 0 || row >= this._mapData.Height)
            {
                return -1;
            }

            int column = Mathf.Floor(x / _mapData.TileWidth);
            if (column < 0 || column >= this._mapData.Width)
            {
                return -1;
            }

            return _tileArrays[layer][column, row];
        }

       public Map MapData => _mapData;

        public int GetLayerIndex(string pName)
        {
            for (int i = 0; i < _mapData.Layers.Length; i++)
            {
                if (_mapData.Layers[i].Name == pName)
                {
                    return i;
                }
            }

            return -1;
        }

        public int BgLayer => _bgLayer;

        public ObjectGroup ObjectGroup => _objectGroup;
    }

    /// <summary>
    /// Data to be used when drawing tiles from the tileset
    /// </summary>
    internal struct SpriteTileSetData
    {
        public Rectangle rect;
        public int tileSetId;

        public SpriteTileSetData(int tileSetId, Rectangle rect)
        {
            this.tileSetId = tileSetId;
            this.rect = rect;
        }
    }
}