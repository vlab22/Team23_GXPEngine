using System;
using System.Collections;

namespace GXPEngine
{
    public class LevelEndScreen : Screen
    {
        private bool _buttonPressed = true;
        
        public LevelEndScreen() : base("data/Level End Screen.png")
        {
            //Wait some time to enable input
            CoroutineManager.StartCoroutine(WaitSomeTime(), this);
        }

        private IEnumerator WaitSomeTime()
        {
            yield return new WaitForMilliSeconds(2000);

            _buttonPressed = false;
        }

        void Update()
        {
            if (!_buttonPressed && (Input.GetKeyDown(Key.LEFT) || Input.GetKeyDown(Key.RIGHT)))
            {
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