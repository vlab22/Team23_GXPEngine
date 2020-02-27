using System;
using System.Collections;

namespace GXPEngine
{
    public class Tutorial01Screen : Screen
    {
        private bool _buttonPressed;

        private AnimationSprite _controllerFlapAnim;
        private int _controllerFlapAnimFrame;
        private int _controllerFlapAnimTime;
        private int _controllerFlapAnimSpeed = 500;
        
        private AnimationSprite _controllerOneAnim;
        private int _controllerOneAnimFrame;
        private int _controllerOneAnimTime;
        private int _controllerOneAnimSpeed = 500;
        
        private AnimationSprite _storkFlapAnim;
        private int _storkFlapAnimFrame;
        private int _storkFlapAnimTime;
        private int _storkFlapAnimSpeed = 500;
        
        private AnimationSprite _storkOneAnim;
        private int _storkOneAnimFrame;
        private int _storkOneAnimTime;
        private int _storkOneAnimSpeed = 500;

        public Tutorial01Screen() : base("data/Tutorial 01 screen.png")
        {
            _controllerFlapAnim = new AnimationSprite("data/pressdown wings spritesheet.png", 2, 1, -1, false, false);
            AddChild(_controllerFlapAnim);
            _controllerFlapAnim.SetXY(112, 238);
            
            _controllerOneAnim = new AnimationSprite("data/pressonedown.png", 2, 1, -1, false, false);
            AddChild(_controllerOneAnim);
            _controllerOneAnim.SetXY(931, 570);
            
            _storkFlapAnim = new AnimationSprite("data/spritesheet flapdown.png", 2, 1, -1, false, false);
            AddChild(_storkFlapAnim);
            _storkFlapAnim.SetXY(539, 48);
            _storkFlapAnim.Turn(10);
            
            _storkOneAnim = new AnimationSprite("data/spritesheet flaponedown.png", 2, 1, -1, false, false);
            AddChild(_storkOneAnim);
            _storkOneAnim.SetXY(1361, 462);
            _storkOneAnim.Turn(8);

            CoroutineManager.StartCoroutine(AnimateWings(), this);
        }

        private IEnumerator AnimateWings()
        {
            while (Destroyed == false)
            {
                _controllerFlapAnim.SetFrame(0);
                _controllerOneAnim.SetFrame(0);
                _storkFlapAnim.SetFrame(0);
                _storkOneAnim.SetFrame(0);
                yield return new WaitForMilliSeconds(200);
                
                _controllerFlapAnim.SetFrame(1);
                _controllerOneAnim.SetFrame(1);
                _storkFlapAnim.SetFrame(1);
                _storkOneAnim.SetFrame(1);
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