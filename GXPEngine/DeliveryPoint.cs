using System;
using System.Collections;
using System.Drawing;
using System.Linq.Expressions;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class DeliveryPoint : Sprite
    {
        private Rectangle _customColliderBounds;

        private uint _timer;
        
        public DeliveryPoint(float pX, float pY, float pWidth, float pHeight, uint pTimer) : base("data/Delivery Point.png")
        {
            _timer = pTimer;
            width = Mathf.Round(pWidth);
            height = Mathf.Round(pHeight);

            x = pX + width / 2f;
            y = pY - height + height / 2f;

            SetOrigin(width / 2f, height / 2f);

            _customColliderBounds = new Rectangle(-width * 0.25f, -height * 0.25f, width * 0.5f, height * 0.5f);

            // var easy = new EasyDraw(100, 100);
            // easy.Clear(Color.Black);
            // AddChild(easy);

            CoroutineManager.StartCoroutine(ResizeRoutine(), this);
        }

        public uint Timer => _timer;

        private IEnumerator ResizeRoutine()
        {
            int d = 400;
            float from = 1;
            float to = 0.95f;
            while (true)
            {
                int time = 0;
                while (time < d)
                {
                    float scaleX = 1 - Easing.Ease(Easing.Equation.CubicEaseOut, time, 0, 0.05f, d);
                    float scaleY = 1 - Easing.Ease(Easing.Equation.CubicEaseOut, time, 0, 0.05f, d);

                    SetScaleXY(scaleX, scaleY);

                    //Console.WriteLine($"{this}: {time} | {scaleX}");

                    time += Time.deltaTime;

                    yield return null;
                }

                time = 0;
                while (time < d)
                {
                    float scaleX = 0.95f + Easing.Ease(Easing.Equation.CubicEaseIn, time, 0, 0.05f, d);
                    float scaleY = 0.95f + Easing.Ease(Easing.Equation.CubicEaseIn, time, 0, 0.05f, d);

                    SetScaleXY(scaleX, scaleY);

                    time += Time.deltaTime;

                    yield return null;
                }
            }
        }

        void Update()
        {
            if (!this.Enabled) return;
            
            var p0 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            var p1 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            var p2 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            var p3 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);

            CanvasDebugger2.Instance.DrawLine(p0.x, p0.y, p1.x, p1.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p1.x, p1.y, p2.x, p2.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p2.x, p2.y, p3.x, p3.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p3.x, p3.y, p0.x, p0.y, Color.Blue);
        }

        public override Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            ret[1] = TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            ret[2] = TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            ret[3] = TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);
            return ret;
        }
    }
}