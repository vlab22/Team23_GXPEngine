using System;
using System.Collections;

namespace GXPEngine
{
    public class LevelEndScreen : Screen
    {
        private bool _buttonPressed = true;

        private Sprite _nextLevelSprite;

        public LevelEndScreen() : base("data/Next level screen.png")
        {
            _nextLevelSprite = new Sprite("data/Next Level Text.png", true, false);
            AddChild(_nextLevelSprite);
            _nextLevelSprite.SetXY(1225, 743);
            _nextLevelSprite.SetOrigin(_nextLevelSprite.width * 0.5f, _nextLevelSprite.height * 0.5f);

            //Wait some time to enable input
            CoroutineManager.StartCoroutine(WaitSomeTime(), this);

            //Animate Text
            SpriteTweener.TweenScalePingPong(_nextLevelSprite, 1, 1.05f, 300);
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