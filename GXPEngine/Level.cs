using System;
using System.Collections;
using System.Collections.Generic;
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

        private StorkManager _storkManager;
        private Stork _stork;

        private DronesManager _dronesesManager;

        private readonly Sprite[] _pizzasPool = new Sprite[20];
        private int _pizzaPoolIndex = 0;
        private HuntersManager _huntersManager;

        private AirplanesManager _airplanesManager;

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

            var playerInput = new PlayerInput();
            AddChild(playerInput);

            float storkMaxSpeed = spawnPointObject.GetFloatProperty("speed", 200);

            _stork = new Stork(storkMaxSpeed)
            {
                x = _spawnPoint.x,
                y = _spawnPoint.y,
                StorkInput = playerInput,
            };
            AddChild(_stork);

            _storkManager = new StorkManager(_stork, this);
            AddChild(_storkManager);

            _stork.IUpdater = _storkManager;

            var hunterBulletManager = new HunterBulletManager(this);
            AddChild(hunterBulletManager);

            _huntersManager = new HuntersManager(this, hunterBulletManager);
            _huntersManager.SpawnHunters();
            _huntersManager.SetHuntersTarget(_stork);

            _dronesesManager = new DronesManager(this);
            _dronesesManager.SpawnDrones();

            _dronesesManager.SetDronesTarget(_stork);

            for (int i = 0; i < _pizzasPool.Length; i++)
            {
                var pizza = new PizzaGameObject("data/pizza00.png");
                pizza.visible = false;
                this.AddChild(pizza);

                _pizzasPool[i] = pizza;
            }

            _airplanesManager = new AirplanesManager(this);
            _airplanesManager.SpawnAirplanes();

            AddChild(_airplanesManager);
            
            _map.DrawBorders(this, 0.5f);

            CoroutineManager.StartCoroutine(SetCamTargetRoutine(_stork), this);

            AddChild(_cam);
            _cam.Map = _map;
            _cam.SetXY(_map.MapWidthInPixels * 0.5f, _map.MapHeightInPixels * 0.5f);
            _cam.Start();

            var hud = new HUD(_cam);
        }

        void Update()
        {
            if (Input.GetKeyDown(Key.D))
            {
                CoroutineManager.StartCoroutine(_storkManager.DropPizzaRoutine(_stork.Pos), this);
            }
        }

        private IEnumerator SetCamTargetRoutine(Stork stork)
        {
            yield return new WaitForMilliSeconds(500);
            _cam.Target = stork;
            _cam.TargetFrontDistance = 200;
        }

        public Sprite GetPizzaFromPool()
        {
            var pizza = _pizzasPool[_pizzaPoolIndex];
            _pizzaPoolIndex++;
            _pizzaPoolIndex %= _pizzasPool.Length;

            return pizza;
        }

        public string ChildrenToString()
        {
            return string.Join(Environment.NewLine, GetChildren().Select(c => c.name));
        }

        public MapGameObject Map => _map;

        public int FirstAirplaneIndex => _airplanesManager.FirstAirplaneIndex;

        public IHasSpeed PlayerHasSpeed => _stork;

        public DronesManager DronesesManager => _dronesesManager;

        public HuntersManager HuntersManager => _huntersManager;

        public AirplanesManager AirplanesManager => _airplanesManager;
    }
}