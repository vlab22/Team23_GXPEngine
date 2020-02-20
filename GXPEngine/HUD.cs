using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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

        private Sprite[] _pizzaLifes;

        public Map _mapData;

        public static Color[] CarsColors = new Color[]
        {
            Color.DeepPink,
            Color.OrangeRed,
            Color.White,
            Color.Yellow,
        };

        private HudSlider _slider00;
        private HudSlider _slider01;


        public HUD(Camera camera)
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

            _pizzaLifes = new Sprite[pizzasHudData.Length];

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

                var pizzaLostHud = new Sprite(pizzaHudBitmapMap[pizzaLostImageFileName], false);
                AddChild(pizzaLostHud);

                pizzaLostHud.SetScaleXY(pizzaHudData.Width / pizzaHudOriginalW,
                    pizzaHudData.Height / pizzaHudOriginalH);
                pizzaLostHud.SetXY(pizzaHudData.X, pizzaHudData.Y - pizzaHudData.Height);

                pizzaLostHud.SetActive(false);
            }

            _camera = camera;
            _camera.AddChild(this);

            this.x = -MyGame.HALF_SCREEN_WIDTH;
            this.y = -MyGame.HALF_SCREEN_HEIGHT;

            var centerText = $@"<= and => arrows to flap wings
Turn is weird flapping one wing while the other is static";

            _centerTextBoard = new HudTextBoard(centerText, 312, 32, 20, CenterMode.Center, CenterMode.Center);
            _centerTextBoard.SetText(centerText);
            _centerTextBoard.visible = true;
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

            _slider01.OnValueChanged += ChangeDroneDetectRange;

            _slider01.OnValueChanged += DebugChangeThermometerValue;

            _hudThermometer = new HudThermometer();
            AddChild(_hudThermometer);

            _hudThermometer.SetXY(game.width - (1920 - 1794), 28);
        }

        public void UpdateLevelScore(uint newLevelScore)
        {
            _hudScore.UpdateScore(newLevelScore);
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
    }
}