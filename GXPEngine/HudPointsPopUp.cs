using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
    public class HudPointsPopUp : HudTextBoard
    {
        private GameObject _target;
        private readonly Vector2 _offset = Vector2.right * 30 + Vector2.up * -50;

        private HudTextBoard _shadowText;

        private int _fadeInOutDuration = 1500;

        private int _offSetAnimTimer;
        private Vector2 _offsetAnim = Vector2.up * - 30;
        private bool _isFading;

        public HudPointsPopUp() : base("+100", 0, 0, 50, 50, 18, CenterMode.Center, CenterMode.Center)
        {
            EasyDraw.TextFont("data/Chantal W00 Medium.ttf", 18);
            
            _shadowText = new HudTextBoard("+100", -1, -1, 50, 50, 18, CenterMode.Center, CenterMode.Center);
            _shadowText.EasyDraw.TextFont("data/Chantal W00 Medium.ttf", 18);
            _shadowText.SetClearColor(Color.FromArgb(0, 1, 1, 1));
            ((IHasColor) _shadowText).MainColor = GlobalVars.Color2;

            AddChild(_shadowText);

            _clearColor = Color.FromArgb(0, 1, 1, 1);
            ((IHasColor) this).MainColor = Color.DimGray;

            visible = false;
        }

        public void Show(int val)
        {
            this.Text = val >= 0 ? $"+{val}" : $"-{val}";
            FadeInOut();
        }

        private void FadeInOut()
        {
            visible = true;
            _isFading = true;
            _offSetAnimTimer = 0;
            
            SpriteTweener.TweenAlpha(this.EasyDraw, 0, 1, _fadeInOutDuration / 2,
                go =>
                {
                    SpriteTweener.TweenAlpha(this.EasyDraw, 1, 0, _fadeInOutDuration / 2, go2 =>
                    {
                        _offSetAnimTimer = 0;
                        _isFading = false;
                        visible = false;
                    });
                });
        }

        void Update()
        {
            if (!Enabled || _target == null || _target.Destroyed) return;

            var targetLocalPos = parent.InverseTransformPoint(_target.Pos);

            var nextPos = targetLocalPos + _offset;

            SetXY(nextPos.x, nextPos.y);

            _shadowText.EasyDraw.alpha = this.EasyDraw.alpha;

            if (_isFading && _offSetAnimTimer <= _fadeInOutDuration)
            {
                float xPos = _offsetAnim.x * Easing.Ease(Easing.Equation.QuadEaseOut, _offSetAnimTimer, 0, 1, _fadeInOutDuration);
                float yPos = _offsetAnim.y * Easing.Ease(Easing.Equation.QuadEaseOut, _offSetAnimTimer, 0, 1, _fadeInOutDuration);
                
                SetXY(nextPos.x + xPos, nextPos.y + yPos);
                
                _offSetAnimTimer += Time.deltaTime;
            }

            if (Input.GetKeyDown(Key.L))
            {
                Show(100);
            }
        }

        public GameObject Target
        {
            get => _target;
            set => _target = value;
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                SetText(_text);
                _shadowText.SetText(_text);
            }
        }
    }
}