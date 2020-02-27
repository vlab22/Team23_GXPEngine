using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine
{
    public class GameOverScreen : Screen
    {
        private bool _buttonPressed;

        private HudTextBoard _scoreTitle;

        private List<HudTextBoard> _scoreBoardTexts;

        public GameOverScreen() : base("data/GameOver Screen.png")
        {
            _buttonPressed = true;

            _scoreTitle = new HudTextBoard("Score Board", 0, 0, 500, 500, 32, CenterMode.Center, CenterMode.Center);
            AddChild(_scoreTitle);
            _scoreTitle.SetClearColor(Color.FromArgb(0,1,1,1));
            ((IHasColor) _scoreTitle).MainColor = GlobalVars.Color2;
            
            _scoreTitle.EasyDraw.TextFont("data/Chantal W00 Medium.ttf", 32);
            _scoreTitle.SetText("Score Board");
            _scoreTitle.Centralize();
            _scoreTitle.y = 100;

            var scorePointsList = MyGame.ThisInstance.ScoreBoardList;

            int pad = scorePointsList[0].score.ToString().Length;
            
            _scoreBoardTexts = new List<HudTextBoard>();
            for (int i = 0; i < scorePointsList.Count; i++)
            {
                var scoreData = scorePointsList[i];

                var format = "{0}\t{1,"+pad+"}";
                
                var text = string.Format(format, scoreData.name.ToUpper().Substring(0, 3), scoreData.score);//$"{scoreData.name.ToUpper().Substring(0, 3)} {$"{scoreData.score:10}"}";
                
                var hudScoreText = new HudTextBoard(text, 0, 0, 1000, 200, 32, CenterMode.Min, CenterMode.Min);
                AddChild(hudScoreText);
                hudScoreText.SetClearColor(Color.FromArgb(0,1,1,1));
                ((IHasColor) hudScoreText).MainColor = GlobalVars.Color2;
                hudScoreText.EasyDraw.TextFont("data/Chantal W00 Medium.ttf", 32);

                hudScoreText.SetText(text);
                hudScoreText.Centralize();

                hudScoreText.x = 750;
                hudScoreText.y = 200 + i * 40;
            }
            
            
            // _scoreBoard = new HudTextBoard("none", 0, 0, 32, CenterMode.Center, CenterMode.Center);
            // AddChild(_scoreBoard);
            // _scoreBoard.SetClearColor(Color.FromArgb(0,1,1,1));
            // ((IHasColor) _scoreBoard).MainColor = GlobalVars.Color2;

            string scoreText = "";
            
            
            _scoreTitle.EasyDraw.TextFont("data/Chantal W00 Medium.ttf", 32);
            
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

            HudScreenFader.instance.FadeInOut(this.parent, 3000, () =>
            {
                //Load Tutorial 01 screen
                Console.WriteLine($"{this}: to StartScreen");

                MyGame.ThisInstance.StartScreen();

                Destroy();
            }, null);
        }
    }
}