using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine
{
    public class HudScreenFader : EasyDraw, IHasColor
    {
        public static HudScreenFader instance;

        private Color _mainColor;

        //private int _alpha;
        private Color _clearColor;

        public delegate void OnTransition();

        public delegate void OnFinished();

        public HudScreenFader(int pWidth = 10, int pHeight = 10) : base(pWidth, pHeight, false)
        {
            instance = this;

            this.width = Game.main.width;
            this.height = Game.main.height;

            _clearColor = Color.FromArgb(0, 1, 1, 1);

            Clear(_clearColor);
            NoStroke();
            Fill(255, 255, 255);
            Rect(0, 0, width, height);

            SetActive(false);
        }

        void Update()
        {
            //Console.WriteLine($"{this}: alpha: {alpha} | {parent}");
        }

        public void FadeInOut(GameObject parent, int duration = 1400, OnTransition onTransition = null,
            OnFinished onFinished = null, CenterMode centerMode = CenterMode.Min)
        {
            parent.AddChild(this);

            if (centerMode == CenterMode.Center)
            {
                SetXY(-width * 0.5f, -height * 0.5f);
            }
            else
            {
                SetXY(0, 0);
            }

            SetActive(true);

            SpriteTweener.TweenAlpha(this, 0, 1, duration / 2, o =>
            {
                onTransition?.Invoke();

                SpriteTweener.TweenAlpha(this, 1, 0, duration / 2, go =>
                {
                    onFinished?.Invoke();

                    SetActive(false);

                    this.parent.RemoveChild(this);
                });
            });

            //CoroutineManager.StartCoroutine(instance.FadeInOutRoutine(time, onFinished));
        }

        private IEnumerator FadeInOutRoutine(int time, OnFinished onFinished)
        {
            instance.visible = true;
            //instance._alpha = 0;
            instance.alpha = 0;

            int interval = time / 2;

            DrawableTweener.TweenColorAlpha(instance, 0, 1, interval, Easing.Equation.CubicEaseOut);

            yield return new WaitForMilliSeconds(interval);

            DrawableTweener.TweenColorAlpha(instance, 1, 0, interval, Easing.Equation.CubicEaseOut);

            onFinished?.Invoke();

            yield return new WaitForMilliSeconds(interval);

            Destroy();
        }

        Color IHasColor.MainColor
        {
            get => _mainColor;
            set => _mainColor = value;
        }

        float IHasColor.Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        List<GameObject> IHasColor.children => this.GetChildren();
    }
}