using System;
using System.Drawing;

namespace GXPEngine
{
    public class HudScore : Sprite, IHasTweenInteger
    {
        private uint _score;
        private EasyDraw _scoreLabelEasyDraw;
        private int _intValue;

        public HudScore(string filename) : base(filename, false, false)
        {
            //Hud score text
            //"VAGRundschriftD"
            _scoreLabelEasyDraw = new EasyDraw(350, 40, false);
            AddChild(_scoreLabelEasyDraw);
            
            //78 115
            _scoreLabelEasyDraw.SetXY(60, 70);
            _scoreLabelEasyDraw.Clear(Color.FromArgb(1, 1, 1, 1));
            _scoreLabelEasyDraw.TextFont($"data/VAGRundschriftD.ttf", 32);
            _scoreLabelEasyDraw.Fill(Color.White);
            _scoreLabelEasyDraw.TextAlign(CenterMode.Min, CenterMode.Min);
            _scoreLabelEasyDraw.Text($"{_score:00000000000000}", 0, 0); //"00000000000000" 14 digits
        }

        public void UpdateScore(uint score)
        {
            IntegerTweener.TweenInteger(this, (int)Score, (int)score);
        }

        public uint Score
        {
            get => _score;
            set
            {
                _score = value;
                _scoreLabelEasyDraw.Clear(Color.FromArgb(0, 1, 1, 1));
                _scoreLabelEasyDraw.Text($"{_score:00000000000000}", 0, 0);
            }
        }

        public int IntValue
        {
            get => (int)_score;
            set => Score = (uint)value;
        }
    }
}