using System;

namespace GXPEngine
{
    public class LevelEndScreen : Screen
    {
        private bool _buttonPressed;

        public LevelEndScreen() : base("data/Level End Screen.png")
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(Key.LEFT) || Input.GetKeyDown(Key.RIGHT))
            {
                if (_buttonPressed) return;

                _buttonPressed = true;

                HudScreenFader.instance.FadeInOut(this.parent, 1400, () =>
                {
                    //Load Tutorial 01 screen
                    Console.WriteLine($"{this}: to next Tutorial Screen");

                    MyGame.ThisInstance.NextLevel();
                    
                    Destroy();
                });
            }
        }
    }
}