using System;
using System.Collections;

namespace GXPEngine
{
    public class Tutorial01Screen : Screen
    {
        private bool _buttonPressed;

        private AnimationSprite _wingsAnim;
        private int _wingAnimFrame;
        private int _wingAnimTime;
        private int _wingAnimSpeed = 500;

        public Tutorial01Screen() : base("data/Tutorial 01 screen.png")
        {
            _wingsAnim = new AnimationSprite("data/pressdown wings spritesheet.png", 2, 1, -1, false, false);
            AddChild(_wingsAnim);
            
            _wingsAnim.SetXY(137, 324);

            CoroutineManager.StartCoroutine(AnimateWings(), this);
        }

        private IEnumerator AnimateWings()
        {
            while (Destroyed == false)
            {
                _wingsAnim.SetFrame(0);
                yield return new WaitForMilliSeconds(200);
                
                _wingsAnim.SetFrame(1);
                yield return new WaitForMilliSeconds(900);
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
            
            SoundManager.Instance.FadeOutCurrentMusic(500);
            
            HudScreenFader.instance.FadeInOut( this.parent,1400, () =>
            {
                //Load Tutorial 01 screen
                Console.WriteLine($"{this}: to Level 00");
                
                MyGame.ThisInstance.LoadLevel(0);

                Destroy();
            });
        }
    }
}