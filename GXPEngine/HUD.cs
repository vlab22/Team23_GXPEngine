﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

 namespace GXPEngine
{
    public class HUD : GameObject
    {
        private Camera _camera;

        private HudTextBoard _centerTextBoard;

        private HashSet<int> _vehiclesEndRacePosition;

        public static Color[] CarsColors = new Color[]
        {
            Color.DeepPink,
            Color.OrangeRed,
            Color.White,
            Color.Yellow,
        };

        public HUD(Camera camera)
        {
            _camera = camera;
            _camera.AddChild(this);

            this.x = -MyGame.HALF_SCREEN_WIDTH;
            this.y = -MyGame.HALF_SCREEN_HEIGHT;

            var centerText = $@"<= and => arrows to flap wings
Turn is weird flapping one wing while the other is static";
            
            _centerTextBoard = new HudTextBoard(centerText, 312, 32, 22, CenterMode.Center, CenterMode.Center);
            _centerTextBoard.SetText(centerText);
            _centerTextBoard.visible = true;
            _centerTextBoard.Centralize();
            _centerTextBoard.y = Game.main.height - _centerTextBoard.Height;
            
            AddChild(_centerTextBoard);
        }

        public override void Destroy()
        {
            // LocalEvents.Instance.RemoveListener<LevelLocalEvent>(LevelLocalEventsHandler);
            // LocalEvents.Instance.RemoveListener<VehicleLocalEvent>(VehicleLocalEventHandler);

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