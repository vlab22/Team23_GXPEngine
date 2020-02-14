using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace GXPEngine
{
    public class HudTextBoard : GameObject, IHasColor
    {
        private Color _clearColor = Color.Black; //FromArgb(1, 1, 1, 0);
        private EasyDraw _easyDraw;

        private string _text = "";
        private float _textX;
        private float _textY;
        private Color _mainColor = Color.White;
        private int _alpha = 255;

        public HudTextBoard(string pText, float px, float py, int width, int height, int textSize,
            CenterMode hor = CenterMode.Min,
            CenterMode ver = CenterMode.Min)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(@"data/Gaiatype.ttf");
            
            var font = new Font(pfc.Families[0], 22, FontStyle.Regular);

            this.x = px;
            this.y = py;
            this._text = pText;

            _easyDraw = new EasyDraw(width, height, false);
            _easyDraw.TextFont(font);
            
            if (!string.IsNullOrEmpty(pText))
            {
                _easyDraw.TextSize(textSize);
                _easyDraw.TextDimensions(_text, out var w, out var h);
                var wr = Mathf.Round(w);
                var hr = Mathf.Round(h);

                if (wr <= 0)
                {
                    wr = 10;
                }

                if (hr <= 0)
                {
                    hr = 10;
                }

                _easyDraw = new EasyDraw(wr, hr, false);
            }
            _easyDraw.TextFont(font);
            _easyDraw.TextSize(textSize);
            _easyDraw.TextAlign(hor, ver);
           
            AddChild(_easyDraw);

            if (hor == CenterMode.Center)
            {
                _textX = _easyDraw.width * 0.5f;
            }

            if (ver == CenterMode.Center)
            {
                _textY = _easyDraw.height * 0.5f;
            }
            
            SetText(_text);
        }

        public HudTextBoard(int width, int height, int textSize, Color pClearColor) : this("", 0, 0, width, height,
            textSize, CenterMode.Min,
            CenterMode.Min)
        {
            _clearColor = pClearColor;
        }

        public HudTextBoard(string pText, int px, int py, int textSize, CenterMode hor = CenterMode.Min,
            CenterMode ver = CenterMode.Min) : this(pText, px, py, 10, 10, textSize, hor,
            ver)
        {
        }
        
        public HudTextBoard(string pText, int px, int py, int textSize, Color pClearColor) : this(pText, px, py, 10, 10, textSize, CenterMode.Min,
            CenterMode.Min)
        {
            _clearColor = pClearColor;
        }

        public HudTextBoard(string pText, int px, int py, int textSize) : this(pText, px, py, 10, 10, textSize,
            CenterMode.Min,
            CenterMode.Min)
        {
        }

        public HudTextBoard(int width, int height, int textSize, CenterMode hor = CenterMode.Min,
            CenterMode ver = CenterMode.Min) : this("", 0, 0, width, height, textSize, hor,
            ver)
        {
        }


        public void FadeIn(int duration = 1000)
        {
            _alpha = 0;
            DrawableTweener.TweenColorAlpha(this, 0, 255, duration, Easing.Equation.CubicEaseOut);
            CoroutineManager.StartCoroutine(UpdateFadeIn());
        }

        public void FadeOut(int duration = 1000)
        {
            DrawableTweener.TweenColorAlpha(this, 255, 0, duration, Easing.Equation.CubicEaseOut);
            CoroutineManager.StartCoroutine(UpdateFadeOut());
        }

        private IEnumerator UpdateFadeIn()
        {
            visible = true;

            while (_alpha < 255)
            {
                UpdateAlpha();
                yield return null;
            }

            _alpha = 255;
            UpdateAlpha();
        }

        private IEnumerator UpdateFadeOut()
        {
            while (_alpha > 0)
            {
                UpdateAlpha();
                yield return null;
            }

            visible = false;
        }

        public void Blink(int duration = 200)
        {
            CoroutineManager.StartCoroutine(BlinkRoutine(duration));
        }

        private IEnumerator BlinkRoutine(int duration)
        {
            int interval = duration / 2;
            visible = true;

            while (visible)
            {
                _alpha = 0;
                DrawableTweener.TweenColorAlpha(this, 0, 255, interval, Easing.Equation.Linear);
                while (_alpha < 255)
                {
                    UpdateAlpha();
                    yield return null;
                }

                _alpha = 255;
                DrawableTweener.TweenColorAlpha(this, 255, 0, duration, Easing.Equation.Linear);
                while (_alpha > 0)
                {
                    UpdateAlpha();
                    yield return null;
                }
            }
        }

        private void UpdateAlpha()
        {
            //Console.WriteLine("_alpha: " + _alpha);

            _clearColor = Color.FromArgb(_alpha, _clearColor);
            _easyDraw.Clear(_clearColor);
            _easyDraw.Fill(_mainColor, _alpha);
            _easyDraw.Text(_text, _textX, _textY);
        }

        public void SetText(string text)
        {
            _text = text;
            _easyDraw.Clear(_clearColor);
            _easyDraw.Fill(_mainColor, _alpha);
            _easyDraw.Text(text, _textX, _textY);
        }

        Color IHasColor.MainColor
        {
            get => _mainColor;
            set
            {
                _mainColor = value;
                SetText(_text);
            }
        }

        float IHasColor.Alpha
        {
            get => _alpha;
            set => _alpha = Mathf.Round(Mathf.Clamp(value, 0, 255));
        }

        List<GameObject> IHasColor.children => GetChildren();

        public float Width => _easyDraw.width;
        public float Height => _easyDraw.height;

        public void Centralize()
        {
            Centralize(Game.main.width, Game.main.height);
        }
        
        public void Centralize(int pWidth, int pHeight)
        {
            this.x = pWidth * 0.5f - this.Width * 0.5f;
            this.y = pHeight * 0.5f - this.Height * 0.5f;
        }
    }
}