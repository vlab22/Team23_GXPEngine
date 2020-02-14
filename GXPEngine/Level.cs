using System.Collections;
using System.Linq;
using GXPEngine.Core;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class Level : GameObject
    {
        private FollowCamera _cam;

        private MapGameObject _map;

        private Vector2 _spawnPoint;

        public Level(FollowCamera pCam, MapGameObject pMap)
        {
            _cam = pCam;

            _map = pMap;

            var spawnPointObject = _map.ObjectGroup.Objects.FirstOrDefault(o => o.Name == "spawn point");
            _spawnPoint = new Vector2(spawnPointObject.X, spawnPointObject.Y);

            AddChild(_map);

            //Create delivery point
            var deliveryPointObject = _map.ObjectGroup.Objects.FirstOrDefault(o => o.Name == "delivery point");
            var deliveryPoint = new DeliveryPoint(deliveryPointObject.X, deliveryPointObject.Y,
                deliveryPointObject.Width, deliveryPointObject.Height);
            AddChild(deliveryPoint);

            // var sprite = new Sprite("data/mapData-top-view-rooftops-city-streets-trees-highways-61413117.png");
            // AddChild(sprite);

            var playerInput = new PlayerInput();
            AddChild(playerInput);

            var stork = new Stork
            {
                x = _spawnPoint.x,
                y = _spawnPoint.y,
                StorkInput = playerInput
            };

            var pizza = new PizzaGameObject("data/pizza00.png");
            stork.AddChildAt(pizza, 0);
            pizza.x = 15;

            CoroutineManager.StartCoroutine(SetCamTargetRoutine(stork));

            var storkStateManager = new StorkStateManager(stork);

            AddChild(stork);

            AddChild(_cam);

            var hud = new HUD(_cam);
        }

        private IEnumerator SetCamTargetRoutine(Stork stork)
        {
            yield return new WaitForMilliSeconds(500);
            _cam.Target = stork;
            _cam.TargetFrontDistance = 200;
        }

        public MapGameObject Map => _map;
    }
}