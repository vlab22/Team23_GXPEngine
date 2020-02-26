using System;
using System.Collections;
using System.Drawing;

namespace GXPEngine
{
    public class StartScreen : Screen
    {
        private HudTextBoard _startText;

        private bool _buttonPressed;
        
        public StartScreen() : base("data/Startscreen Bg.png")
        {
            _startText = new HudTextBoard("Flap Wings to Start", 0,0,32, Color.FromArgb(0,1,1,1));
            _startText.SetText("Flap Wings to Start");
            AddChild(_startText);
            _startText.Centralize();

            SoundManager.Instance.PlayMusic(0);
            
            CoroutineManager.StartCoroutine(BlinkText(), this);
        }

        IEnumerator BlinkText()
        {
            int blinkDuration = 800;
            
            while (true)
            {

                SpriteTweener.TweenAlpha(_startText.EasyDraw, 1, 0.2f, blinkDuration/2, o =>
                {
                    SpriteTweener.TweenAlpha(_startText.EasyDraw, 0.2f, 1, blinkDuration/2);
                });
                
                yield return new WaitForMilliSeconds(blinkDuration);
            }
        }

        void Update()
        {
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
            
            HudScreenFader.instance.FadeInOut( this.parent,1400, () =>
            {
                //Load Tutorial 01 screen
                Console.WriteLine($"{this}: to tutorial 01");
                    
                var tut01Screen = new Tutorial01Screen();
                Game.main.AddChildAt(tut01Screen, HudScreenFader.instance.Index - 1);
                Destroy();
            });
        }
    }
}