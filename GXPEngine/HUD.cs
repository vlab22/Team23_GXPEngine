using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using GXPEngine.Core;
using GXPEngine.GameLocalEvents;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class HUD : GameObject
    {
        public static HUD Instance;

        private Camera _camera;

        private HudTextBoard _centerTextBoard;

        private HashSet<int> _vehiclesEndRacePosition;

        private HudScore _hudScore;

        private HudThermometer _hudThermometer;

        private Sprite[] _pizzaLives;
        private Sprite[] _pizzaLostLives;

        public Map _mapData;

        private HudPointsPopUp _hudPointsPopUp;

        private HudArrowToObjective _hudArrowToObjective;

        private HudSlider _slider00;
        private HudSlider _slider01;

        private HudTextBoard _debugTimer;

        public HUD(Camera camera, GameObject player)
        {
            Instance = this;

            _mapData = TiledMapParserExtended.MapParser.ReadMap("HUD.tmx");

            var objectsDepth0 = _mapData.ObjectGroups.FirstOrDefault(og => og.Name == "Depth 0");

            //Hud Score set
            var hudData = objectsDepth0.Objects.FirstOrDefault(o => o.Name == "Score Bg");
            var hudScoreImage = _mapData.TileSets.FirstOrDefault(ts => ts.FirstGId == hudData.GID).Image.FileName;

            _hudScore = new HudScore(hudScoreImage);
            AddChild(_hudScore);

            _hudScore.SetScaleXY(hudData.Width / _hudScore.width, hudData.Height / _hudScore.height);
            _hudScore.SetXY(hudData.X, hudData.Y - hudData.Height);

            //Pizza life set
            var pizzasHudData = objectsDepth0.Objects.Where(o => o.Name.Trim().StartsWith("Hud Pizza"))
                .OrderBy(o => o.Name)
                .ToArray();

            var pizzaHudBitmapMap = new Dictionary<string, Bitmap>();

            var pizzaLostImageFileName = _mapData.TileSets.FirstOrDefault(ts => ts.Name == "Pizza Lost").Image.FileName;

            pizzaHudBitmapMap.Add(pizzaLostImageFileName, new Bitmap(pizzaLostImageFileName));

            _pizzaLives = new Sprite[pizzasHudData.Length];
            _pizzaLostLives = new Sprite[pizzasHudData.Length];

            for (int i = 0; i < pizzasHudData.Length; i++)
            {
                var pizzaHudData = pizzasHudData[i];

                var imageFile = _mapData.TileSets.FirstOrDefault(ts => ts.FirstGId == pizzaHudData.GID).Image.FileName;

                Bitmap bitMap;

                if (!pizzaHudBitmapMap.ContainsKey(imageFile))
                {
                    bitMap = new Bitmap(imageFile);
                    pizzaHudBitmapMap.Add(imageFile, bitMap);
                }
                else
                {
                    bitMap = pizzaHudBitmapMap[imageFile];
                }

                var pizzaHud = new Sprite(bitMap, false);
                AddChild(pizzaHud);

                int pizzaHudOriginalW = pizzaHud.width;
                int pizzaHudOriginalH = pizzaHud.height;

                pizzaHud.SetScaleXY(pizzaHudData.Width / pizzaHudOriginalW, pizzaHudData.Height / pizzaHudOriginalH);
                pizzaHud.SetXY(pizzaHudData.X, pizzaHudData.Y - pizzaHudData.Height);

                _pizzaLives[i] = pizzaHud;

                var pizzaLostHud = new Sprite(pizzaHudBitmapMap[pizzaLostImageFileName], false);
                AddChild(pizzaLostHud);

                pizzaLostHud.SetScaleXY(pizzaHudData.Width / pizzaHudOriginalW,
                    pizzaHudData.Height / pizzaHudOriginalH);
                pizzaLostHud.SetXY(pizzaHudData.X, pizzaHudData.Y - pizzaHudData.Height);

                pizzaLostHud.SetActive(false);

                _pizzaLostLives[i] = pizzaLostHud;
            }

            _hudPointsPopUp = new HudPointsPopUp();
            AddChild(_hudPointsPopUp);
            _hudPointsPopUp.Target = player;

            _camera = camera;
            _camera.AddChild(this);

            this.x = -MyGame.HALF_SCREEN_WIDTH;
            this.y = -MyGame.HALF_SCREEN_HEIGHT;

            var centerText = $@"<= and => arrows to flap wings
Turn is weird flapping one wing while the other is static";

            _centerTextBoard = new HudTextBoard(centerText, 312, 32, 20, CenterMode.Center, CenterMode.Center);
            _centerTextBoard.SetText(centerText);
            _centerTextBoard.visible = false;
            _centerTextBoard.Centralize();
            _centerTextBoard.y = Game.main.height - _centerTextBoard.Height;

            AddChild(_centerTextBoard);

            _slider00 = new HudSlider(200, 22);
            AddChild(_slider00);

            _slider00.x = 50;
            _slider00.y = 50;

            _slider00.OnValueChanged += ChangeDroneWaitingSpeed;

            _slider01 = new HudSlider(200, 22);
            AddChild(_slider01);

            _slider01.x = 50;
            _slider01.y = 50 + 34;

            //_slider01.OnValueChanged += ChangeDroneDetectRange;

            //_slider01.OnValueChanged += DebugChangeThermometerValue;

            _slider01.OnValueChanged += ChangeDroneFrameSpeed;

            _hudThermometer = new HudThermometer();
            AddChild(_hudThermometer);

            _hudThermometer.SetXY(game.width - (1920 - 1731), 85);

            _hudArrowToObjective = new HudArrowToObjective();
            AddChild(_hudArrowToObjective);

            _hudArrowToObjective.SetScaleXY(0.4f, 0.4f);
            _hudArrowToObjective.SetXY(game.width * 0.5f, game.height * 0.5f);
            _hudArrowToObjective.SetActive(false);

            _debugTimer = new HudTextBoard(60, 60, 12);
            _debugTimer.x = game.width - 60;
            _debugTimer.y = 60;
            AddChild(_debugTimer);
        }

        public void UpdateLevelScore(uint newLevelScore, int animDuration = 400)
        {
            _hudScore.UpdateScore(newLevelScore, animDuration);
        }

        public void UpdatePizzaLives(int lives)
        {
            lives = Mathf.Round(Mathf.Clamp(lives, 0, 3));

            for (int i = 0; i < lives; i++)
            {
                _pizzaLives[i].SetActive(true);
                _pizzaLostLives[i].SetActive(false);
            }

            for (int i = lives; i < _pizzaLives.Length; i++)
            {
                _pizzaLostLives[i].SetActive(true);
                _pizzaLives[i].SetActive(false);
            }

            if (lives < 3 && lives > 0)
                CoroutineManager.StartCoroutine(BlinkPizzaLives(lives), this);
        }

        IEnumerator BlinkPizzaLives(int lives)
        {
            int blink = 0;

            while (blink < 4)
            {
                yield return new WaitForMilliSeconds(400);

                _pizzaLostLives[lives].SetActive(false);
                _pizzaLives[lives].SetActive(true);

                yield return new WaitForMilliSeconds(400);

                _pizzaLostLives[lives].SetActive(true);
                _pizzaLives[lives].SetActive(false);

                blink++;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _pizzaLives.Length; i++)
            {
                _pizzaLives[i].SetActive(false);
                _pizzaLostLives[i].SetActive(false);
            }
        }

        private void ChangeDroneDetectRange(float val)
        {
            var allDrones = MyGame.ThisInstance.CurrentLevel.GetChildren(true)
                .Where(g => g != null && !g.Destroyed && g is DroneGameObject);
            foreach (var o in allDrones)
            {
                var drone = (DroneGameObject) o;

                drone.DetectEnemyRange = 1000 * val;
            }
        }

        private void ChangeDroneWaitingSpeed(float val)
        {
            var allDrones = MyGame.ThisInstance.CurrentLevel.GetChildren(true)
                .Where(g => g != null && !g.Destroyed && g is DroneGameObject);
            foreach (var o in allDrones)
            {
                var drone = (DroneGameObject) o;

                drone.WaitingSpeed = 100 * val;
            }
        }

        private void ChangeDroneFrameSpeed(float val)
        {
            int intVal = Mathf.Round(Mathf.Map(val, 0, 1, 5, 500));

            DroneGameObject.FrameSpeed = intVal;

            Console.WriteLine($"{this}: framespeed: {intVal}");
        }

        private void DebugChangeThermometerValue(float val)
        {
            _hudThermometer.Value = val;
        }

        public override void Destroy()
        {
            base.Destroy();
        }


        private void CreateBlankScreen()
        {
            var blankScreen = new EasyDraw(Game.main.width, Game.main.height);
            blankScreen.NoStroke();
            blankScreen.Fill(Color.Black);
            blankScreen.ShapeAlign(CenterMode.Min, CenterMode.Min);
            blankScreen.Rect(0, 0, blankScreen.width, blankScreen.height);
            AddChild(blankScreen);
        }


        void Update()
        {
            _slider00.visible = MyGame.Debug;
            _slider01.visible = MyGame.Debug;
        }

        private IEnumerator MoveTextBoarToPosition(int lapPosition, HudTextBoard textBoard)
        {
            float xPos = game.width - textBoard.Width;
            float yPos = 50;
            float toYPos = yPos + (lapPosition - 1) * 32;
            float fromY = textBoard.y;

            textBoard.x = xPos;
            float time = 0;
            float duration = 800;

            do
            {
                textBoard.y = Easing.Ease(Easing.Equation.CubicEaseOut, time, fromY, toYPos, duration);

                yield return null;
                time += Time.deltaTime;
            } while (time < duration);
        }

        public HudThermometer Thermometer => _hudThermometer;

        public HudArrowToObjective ArrowToObjective => _hudArrowToObjective;

        public HudTextBoard DebugTimer => _debugTimer;

        public HudPointsPopUp HudPointsPopUp => _hudPointsPopUp;
    }
}