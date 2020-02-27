using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GXPEngine
{
    public class GameOverScreen : Screen
    {
        private bool _buttonPressed;

        private HudTextBoard _scoreTitle;

        private List<HudTextBoard> _scoreBoardTexts;

        private const string FONT_PATH = "data/Chantal W00 Medium.ttf";
        private const int FONT_SIZE = 32;

        private const int SCOREBOARD_X = 700;

        public GameOverScreen() : base("data/GameOver Screen.png")
        {
            _buttonPressed = true;

            _scoreTitle = new HudTextBoard("Score Board", 0, 0, 500, 500, FONT_SIZE, CenterMode.Center,
                CenterMode.Center);
            AddChild(_scoreTitle);
            _scoreTitle.SetClearColor(Color.FromArgb(0, 1, 1, 1));
            ((IHasColor) _scoreTitle).MainColor = GlobalVars.Color2;

            _scoreTitle.EasyDraw.TextFont(FONT_PATH, FONT_SIZE);
            _scoreTitle.SetText("Score Board");
            _scoreTitle.Centralize();
            _scoreTitle.y = 100;

            _scoreTitle.EasyDraw.TextFont(FONT_PATH, FONT_SIZE);

            var playerScoreData = new PlayerScoreData()
            {
                name = "you",
                score = MyGame.ThisInstance.TotalScore
            };

            var scorePointsList = new List<PlayerScoreData>(MyGame.ThisInstance.ScoreBoardList) {playerScoreData};

            scorePointsList = scorePointsList.OrderByDescending(ps => ps.score).ToList();

            int pad = 0;

            if (scorePointsList.Count > 0)
                pad = scorePointsList[0].score.ToString().Length;

            var first10Scores = scorePointsList.Take(10).ToList();

            _scoreBoardTexts = new List<HudTextBoard>();
            for (int i = 0; i < first10Scores.Count; i++)
            {
                var scoreData = scorePointsList[i];

                var format = "{0,2}\t{1}\t{2," + pad + "}";

                var text = string.Format(format, i + 1, scoreData.name.ToUpper().Substring(0, 3),
                    scoreData.score); //$"{scoreData.name.ToUpper().Substring(0, 3)} {$"{scoreData.score:10}"}";

                var hudScoreText = new HudTextBoard(text, 0, 0, 1000, 200, FONT_SIZE, CenterMode.Min, CenterMode.Min);
                AddChild(hudScoreText);
                hudScoreText.SetClearColor(Color.FromArgb(0, 1, 1, 1));

                ((IHasColor) hudScoreText).MainColor =
                    (scoreData.name == "you") ? GlobalVars.Color1 : GlobalVars.Color2;

                hudScoreText.EasyDraw.TextFont(FONT_PATH, FONT_SIZE);

                hudScoreText.SetText(text);
                hudScoreText.Centralize();

                hudScoreText.x = SCOREBOARD_X;
                hudScoreText.y = 200 + i * 40;
            }

            //Get player position, if greater than 10, put it at the end
            var playerPos = scorePointsList.Select(ps => ps.name).ToList().IndexOf("you");
            if (playerPos > 9)
            {
                var format = "{0,2}\t{1}\t{2," + pad + "}";

                var text = string.Format(format, playerPos + 1, playerScoreData.name.ToUpper().Substring(0, 3),
                    playerScoreData.score);

                var hudScoreText = new HudTextBoard(text, 0, 0, 1000, 200, FONT_SIZE, CenterMode.Min, CenterMode.Min);
                AddChild(hudScoreText);
                hudScoreText.SetClearColor(Color.FromArgb(0, 1, 1, 1));

                ((IHasColor) hudScoreText).MainColor = GlobalVars.Color1;

                hudScoreText.EasyDraw.TextFont(FONT_PATH, FONT_SIZE);

                hudScoreText.SetText(text);
                hudScoreText.Centralize();

                hudScoreText.x = SCOREBOARD_X;
                hudScoreText.y = 200 + 30 + first10Scores.Count * 40;
            }

            MyGame.ThisInstance.SaveScoreBoard();

            CoroutineManager.StartCoroutine(WaitSomeTimeToEnableInput(), this);
        }

        private IEnumerator WaitSomeTimeToEnableInput()
        {
            SoundManager.Instance.PlayMusic(1);
            
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