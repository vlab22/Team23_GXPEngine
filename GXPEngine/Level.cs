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

        private CloudsManager _cloudsManager;
        
        private bool _isLevelEndingByWon;
        private bool _isLevelEndingByLost;

        private DeliveryPoint[] _deliveryPoints;
        private int _currentDeliveryPoint;
        private HUD _hud;

        public Level(FollowCamera pCam, MapGameObject pMap)
        {
            _cam = pCam;
            _map = pMap;

            var enemiesSoundManager = new EnemiesSoundManager();
            AddChild(enemiesSoundManager);
            
            var spawnPointObject = _map.ObjectGroup.Objects.FirstOrDefault(o => o.Name == "spawn point");
            _spawnPoint = new Vector2(spawnPointObject.X, spawnPointObject.Y);

            AddChild(_map);

            //Create delivery points
            var deliveryPointObjects = _map.ObjectGroup.Objects.Where(o => o.Name.StartsWith("delivery point"))
                .OrderBy(o => o.Name);
            _deliveryPoints = new DeliveryPoint[deliveryPointObjects.Count()];

            int dCounter = 0;
            foreach (var deliveryPointObj in deliveryPointObjects)
            {
                uint deliveryTimer = (uint)(deliveryPointObj.GetIntProperty("timer", 60) * 1000);
                
                var deliveryPoint = new DeliveryPoint(deliveryPointObj.X, deliveryPointObj.Y,
                    deliveryPointObj.Width, deliveryPointObj.Height, deliveryTimer);
                AddChild(deliveryPoint);

                deliveryPoint.SetActive(false);

                _deliveryPoints[dCounter] = deliveryPoint;
                dCounter++;
            }

            //Enable the first delivery point
            _deliveryPoints[0].SetActive(true);

            float storkMaxSpeed = spawnPointObject.GetFloatProperty("speed", 200);

            _stork = new Stork(storkMaxSpeed)
            {
                x = _spawnPoint.x,
                y = _spawnPoint.y,
                rotation = spawnPointObject.rotation
            };
            
            AddChild(_stork);
            _stork.SetScaleXY(1.5f, 1.5f);

            _storkManager = new StorkManager(_stork, this);
            AddChild(_storkManager);

            _stork.IUpdater = _storkManager;

            var hunterBulletManager = new HunterBulletManager(this);
            AddChild(hunterBulletManager);

            _huntersManager = new HuntersManager(this, hunterBulletManager);
            _huntersManager.SpawnHunters();
            _huntersManager.SetHuntersTarget(_stork);

            _dronesesManager = new DronesManager(this, enemiesSoundManager);
            _dronesesManager.SpawnDrones();

            _dronesesManager.SetDronesTarget(_stork);

            for (int i = 0; i < _pizzasPool.Length; i++)
            {
                var pizza = new PizzaGameObject("data/pizza00.png");
                pizza.visible = false;
                this.AddChild(pizza);

                _pizzasPool[i] = pizza;
            }
            
            _airplanesManager = new AirplanesManager(this, enemiesSoundManager);
            _airplanesManager.SpawnAirplanes();

            AddChild(_airplanesManager);

            _map.DrawBorders(this, 0.5f);
            
            _cloudsManager = new CloudsManager(_map, this);
            AddChild(_cloudsManager);
            
            _cloudsManager.SpawnClouds();
            
            CoroutineManager.StartCoroutine(SetCamTargetRoutine(_stork), this);

            AddChild(_cam);
            _cam.Map = _map;
            _cam.SetXY(_map.MapWidthInPixels * 0.5f, _map.MapHeightInPixels * 0.5f);
            _cam.Start();
            
            _cam.AddChild(HudScreenFader.instance);
            HudScreenFader.instance.SetXY(-game.width * 0.5f, -game.height * 0.5f);

            _hud = new HUD(_cam, _stork);
        }

        public bool ActivateNextDeliveryPoint()
        {
            var lastDeliveryPoint = _deliveryPoints[_currentDeliveryPoint];
            
            SpriteTweener.TweenAlpha(lastDeliveryPoint, 1, 0, 600, go =>
            {
                go.SetActive(false);
            });
            
            if (_currentDeliveryPoint >= _deliveryPoints.Length - 1)
            {
                return false;
            }
            
            _currentDeliveryPoint++;
            
            var nextDeliveryPoint = _deliveryPoints[_currentDeliveryPoint];

            nextDeliveryPoint.SetActive(true);
            SpriteTweener.TweenAlpha(nextDeliveryPoint,  0, 1, 600);

            return true;
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

        public bool IsLevelEnding => _isLevelEndingByLost || _isLevelEndingByWon;

        public bool IsLevelEndingByWon
        {
            get { return _isLevelEndingByWon; }
            set { _isLevelEndingByWon = value; }
        }
        
        public bool IsLevelEndingByLost
        {
            get { return _isLevelEndingByLost; }
            set { _isLevelEndingByLost = value; }
        }

        public DeliveryPoint[] DeliveryPoints => _deliveryPoints;

        public DeliveryPoint CurrentDeliveryPoint => _deliveryPoints[_currentDeliveryPoint];
        
        public Stork Stork => _stork;

        public HUD Hud => _hud;
    }
}