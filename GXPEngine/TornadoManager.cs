using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GXPEngine.Core;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class TornadoManager : GameObject
    {
        private Level _level;

        private List<TornadoGameObject> _tornadosList;

        private EnemiesSoundManager _enemiesSoundManager;

        public TornadoManager(Level pLevel, EnemiesSoundManager pEnemiesSoundManager) : base(false)
        {
            _level = pLevel;
            _enemiesSoundManager = pEnemiesSoundManager;
            _tornadosList = new List<TornadoGameObject>();
        }

        public void SpawnTornadoes()
        {
            var mapData = _level.Map.MapData;

            var tornadoObjectsData = _level.Map.ObjectGroup.Objects.Where(o => o.Name.StartsWith("tornado ")).ToArray();

            for (int i = 0; i < tornadoObjectsData.Length; i++)
            {
                SpawnTornado(tornadoObjectsData[i]);
            }
        }

        public void SpawnTornado(TiledObject tornadoData)
        {
            int throwAngleMin = tornadoData.GetIntProperty("throw_angle_min", 0);
            int throwAngleMax = tornadoData.GetIntProperty("throw_angle_max", 359);
            float throwDistance = tornadoData.GetFloatProperty("throw_distance", 512);
            
            var tornado = new TornadoGameObject(tornadoData.X, tornadoData.Y, tornadoData.Width, tornadoData.Height, throwAngleMin, throwAngleMax, throwDistance);
            tornado.OnUpdateListeners = tornado.OnUpdateListeners.Concat(new IOnUpdateListener[] {_enemiesSoundManager})
                .ToArray();

            CoroutineManager.StartCoroutine(WaitForTargetBeenSetInLevel(tornado), this);

            string idString = tornadoData.Name.Replace("tornado ", "");
            if (int.TryParse(idString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                //Check if has path
                var pathObj = _level.Map.ObjectGroup.Objects.FirstOrDefault(ob => ob.Name == $"path tornado {id}");
                if (pathObj != null && pathObj.polygons?.Length == 1)
                {
                    var pathPos = new Vector2(pathObj.X, pathObj.Y);
                    
                    //Get points
                    var pts = pathObj.polygons[0].points.Select(pt => pt + pathPos).ToArray();

                    if (pts.Length > 0)
                    {
                        tornado.Path = pts;
                    }
                }
            }

            _level.AddChild(tornado);

            _tornadosList.Add(tornado);
        }

        private IEnumerator WaitForTargetBeenSetInLevel(TornadoGameObject tornado)
        {
            while (_level.Stork == null)
            {
                yield return null;
            }

            tornado.Target = _level.Stork;
        }
    }
}