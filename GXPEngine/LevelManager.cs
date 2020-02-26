using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXPEngine.GameLocalEvents;
using GXPEngine.Managers;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class LevelManager : GameObject
    {
        private Level _level;

        private uint _totalScore;
        private uint _levelScore;

        private int _pizzaLives;

        private bool _timerToDeliveryStarts;
        private bool _timerToDeliveryPause;
        private uint _levelTimeToDelivery;
        private uint _levelTimer;

        private Dictionary<LevelLocalEvent.EventType, uint> _scoreValuesMap;

        private int[] _cloudsIds = new int[]
        {
            21,22,23,24,
            37,38,39,40,
            49,50,
            65, 66,
            81, 82,
            97, 98,
        };

        private int _cloudsFirstGid;
        private int[,] _cloudsLayer;

        public LevelManager(Level pLevel, uint pLevelTimeToDelivery = 30000)
        {
            _level = pLevel;
            _levelTimeToDelivery = pLevelTimeToDelivery;

            _cloudsFirstGid = pLevel.Map.MapData.TileSets.FirstOrDefault(ts => ts.Name == "Clouds Tileset").FirstGId;

            LoadScoresValuesMap();

            LocalEvents.Instance.AddListener<LevelLocalEvent>(LevelLocalEventHandler);

            CoroutineManager.StartCoroutine(WaitForHudInstance(), this);

            CoroutineManager.StartCoroutine(WaitToStartTimeCounter(), this);
        }

        private IEnumerator WaitToStartTimeCounter()
        {
            yield return new WaitForMilliSeconds(2000);

            _levelTimeToDelivery = _level.CurrentDeliveryPoint.Timer;

            _timerToDeliveryStarts = true;

            var playerInput = new PlayerInput();
            _level.AddChild(playerInput);

            _level.Stork.StorkInput = playerInput;

            HUD.Instance.ArrowToObjective.Target = _level.CurrentDeliveryPoint;
            HUD.Instance.ArrowToObjective.TargetOrigin = _level.Stork;
            HUD.Instance.ArrowToObjective.Cam = MyGame.ThisInstance.Camera;
            HUD.Instance.ArrowToObjective.Enabled = true;
        }

        void Update()
        {
            if (!this.Enabled || !_timerToDeliveryStarts || _timerToDeliveryPause) return;

            _levelTimer += (uint) Time.deltaTime;

            int cloudId = _level.Map.GetCloudLayerTileIdFromWorld(_level.Stork.x, _level.Stork.y);

            if (IsInsideCloud(cloudId - _cloudsFirstGid))
            {
                //Console.WriteLine($"{this}: cloudId: {cloudId - _cloudsFirstGid} | {Time.time}");
            }

            //Update Hud
            float val = (float) _levelTimer / _levelTimeToDelivery;
            HUD.Instance.Thermometer.Value = 1 - val;

            HUD.Instance.DebugTimer.SetText(Timer.ToString("00"));
        }

        private IEnumerator WaitForHudInstance()
        {
            while (HUD.Instance == null)
            {
                yield return null;
            }
            
            this.PizzaLives = 3;
        }

        private void LevelLocalEventHandler(LevelLocalEvent e)
        {
            switch (e.evt)
            {
                case LevelLocalEvent.EventType.NONE:
                    break;
                case LevelLocalEvent.EventType.LEVEL_START_COUNTER_START:
                    break;
                case LevelLocalEvent.EventType.LEVEL_START_COUNTER_END:
                    break;
                case LevelLocalEvent.EventType.DRONE_DETECTED_ENEMY:
                    break;
                case LevelLocalEvent.EventType.PLANE_HIT_PLAYER:
                    PizzaLives--;
                    break;
                case LevelLocalEvent.EventType.DRONE_HIT_PLAYER:
                    PizzaLives--;
                    break;
                case LevelLocalEvent.EventType.HUNTER_HIT_PLAYER:
                    PizzaLives--;
                    break;
                case LevelLocalEvent.EventType.STORK_GET_POINTS_EVADE_DRONE:
                    if (_scoreValuesMap.TryGetValue(LevelLocalEvent.EventType.STORK_GET_POINTS_EVADE_DRONE,
                        out uint score))
                    {
                        _levelScore += score;

                        HUD.Instance.UpdateLevelScore(_levelScore);
                    }

                    break;
                case LevelLocalEvent.EventType.PIZZA_DELIVERED:

                    CoroutineManager.StartCoroutine(StartDeliveryToNextPoint(), this);

                    break;
                default:
                    break;
            }
        }

        private IEnumerator StartDeliveryToNextPoint()
        {
            //Sum points
            uint timeLeft = _levelTimeToDelivery - _levelTimer;
            uint points = (uint) Mathf.Round(timeLeft * 0.001f * 1000);

            _levelScore += points;

            _timerToDeliveryPause = true;

            HUD.Instance.UpdateLevelScore(_levelScore, 1000);

            yield return new WaitForMilliSeconds(1000);

            bool hasNextDelivery = _level.ActivateNextDeliveryPoint();

            if (hasNextDelivery)
            {
                _levelTimer = 0;
                _timerToDeliveryPause = false;
                _levelTimeToDelivery = _level.CurrentDeliveryPoint.Timer;

                HUD.Instance.ArrowToObjective.Target = _level.CurrentDeliveryPoint;
            }
            else
            {
                //Level end
                CoroutineManager.StartCoroutine(EndLevelByWonRoutine(), this);

                HUD.Instance.ArrowToObjective.Enabled = false;
            }

            yield return null;
        }

        private IEnumerator EndLevelByLost()
        {
            _level.IsLevelEndingByLost = true;

            //Send all drones away
            _level.DronesesManager.EndLevelAllDrones();

            //Stop all hunters
            _level.HuntersManager.EndLevelAllHunters();

            yield return new WaitForMilliSeconds(1400);

            var gameOverScreen = new GameOverScreen();

            HudScreenFader.instance.FadeInOut(MyGame.ThisInstance.Camera, 1400,
                () =>
                {
                    MyGame.ThisInstance.UnLoadCurrentLevel();
                    
                    SoundManager.Instance.DisableAllSounds();
                    
                    _level.RemoveChild(MyGame.ThisInstance.Camera);
                    HudScreenFader.instance.parent = MyGame.ThisInstance;
                    HudScreenFader.instance.SetXY(0,0);
                    MyGame.ThisInstance.AddChildAt(gameOverScreen, HudScreenFader.instance.Index - 1);
                    
                },
                null, CenterMode.Center);
        }

        private IEnumerator EndLevelByWonRoutine()
        {
            _level.IsLevelEndingByWon = true;

            //Send all drones away
            _level.DronesesManager.EndLevelAllDrones();

            //Stop all hunters
            _level.HuntersManager.EndLevelAllHunters();

            yield return new WaitForMilliSeconds(3000);

            var levelEndScreen = new LevelEndScreen();

            HudScreenFader.instance.FadeInOut(MyGame.ThisInstance.Camera, 1400,
                () =>
                {
                    MyGame.ThisInstance.UnLoadCurrentLevel();
                    
                    SoundManager.Instance.DisableAllSounds();
                    
                    _level.RemoveChild(MyGame.ThisInstance.Camera);
                    HudScreenFader.instance.parent = MyGame.ThisInstance;
                    HudScreenFader.instance.SetXY(0,0);
                    MyGame.ThisInstance.AddChildAt(levelEndScreen, HudScreenFader.instance.Index - 1);
                    
                },
                null, CenterMode.Center);
        }

        protected override void OnDestroy()
        {
            LocalEvents.Instance.RemoveListener<LevelLocalEvent>(LevelLocalEventHandler);
            base.OnDestroy();
        }

        void LoadScoresValuesMap()
        {
            _scoreValuesMap = new Dictionary<LevelLocalEvent.EventType, uint>();

            //Load File
            string[] valuesLines = File.ReadAllLines("data/ScoresValues.txt");

            for (int i = 0; i < valuesLines.Length; i++)
            {
                var line = valuesLines[i];
                var lineSplit = line.Split('=');
                if (lineSplit.Length != 2)
                {
                    throw new Exception("data/ScoresValues.txt incorrectly config, all lines must be key=value");
                }

                string key = lineSplit[0].Trim();
                string val = lineSplit[1].Trim();

                if (!LevelLocalEvent.EventType.TryParse(key, true, out LevelLocalEvent.EventType evt))
                {
                    Console.WriteLine($"Error: {key} not defined");
                    continue;
                }

                if (!uint.TryParse(val, out uint score))
                {
                    Console.WriteLine($"Error: {val} not a valid positive integer");
                    continue;
                }

                _scoreValuesMap.Add(evt, score);
            }
        }

        public bool IsInsideCloud(int tileId)
        {
            return Array.BinarySearch(_cloudsIds, 0, _cloudsIds.Length, tileId) > -1;
        }

        public int PizzaLives
        {
            get { return _pizzaLives; }
            private set
            {
                _pizzaLives = value;
                HUD.Instance.UpdatePizzaLives(_pizzaLives);

                if (_pizzaLives <= 0 && _level.IsLevelEndingByLost == false)
                {
                    CoroutineManager.StartCoroutine(EndLevelByLost(), this);
                }
            }
        }

        public uint Timer => (_levelTimeToDelivery - _levelTimer) / 1000;
    }
}