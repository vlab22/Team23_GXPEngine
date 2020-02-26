using System;
using System.Collections;

namespace GXPEngine
{
    public class GameOverScreen : Screen
    {
        private bool _buttonPressed;

        public GameOverScreen() : base("data/GameOver Screen.png")
        {
            _buttonPressed = true;

            SoundManager.Instance.StopAllSounds();
            
            CoroutineManager.StartCoroutine(WaitSomeTimeToEnableInput(), this);
        }

        private IEnumerator WaitSomeTimeToEnableInput()
        {
            yield return new WaitForMilliSeconds(1200);

            _buttonPressed = false; //reenable input
        }

        void Update()
        {
            //SetXY(-width * 0.5f, -height * 0.5f); //ugly
            
            if (Input.GetKeyDown(Key.LEFT) || Input.GetKeyDown(Key.RIGHT))
            {
                if (_buttonPressed) return;

                _buttonPressed = true;
                
                CoroutineManager.StartCoroutine(TransitionRoutine(), this);
            }
        }
        
        private IEnumerator TransitionRoutine()
        {
            yield return null;
            
            SoundManager.Instance.FadeOutCurrentMusic(500);
            
            HudScreenFader.instance.FadeInOut( this.parent,3000, () =>
            {
                //Load Tutorial 01 screen
                Console.WriteLine($"{this}: to StartScreen");

                MyGame.ThisInstance.StartScreen();

                Destroy();
            }, null);
        }
    }
}