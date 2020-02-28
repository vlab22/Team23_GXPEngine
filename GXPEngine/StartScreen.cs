using System;
using System.Collections;
using System.Drawing;

namespace GXPEngine
{
    public class StartScreen : Screen
    {
        private Sprite _startText;

        private bool _buttonPressed;
        
        public StartScreen() : base("data/Startscreen Bg.png")
        {
            //1260 207
            
            _startText = new Sprite("data/Start Screen - pressstart.png", true, false);
            AddChild(_startText);
            _startText.SetOrigin(_startText.width * 0.5f, _startText.height * 0.5f);
            _startText.SetXY(1260, 207);


            float mScale = _startText.scale;
            SpriteTweener.TweenScalePingPong(_startText, mScale, mScale * 1.02f, 300);
            
            SoundManager.Instance.PlayMusic(0);
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