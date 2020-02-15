using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
    public class HudSlider : EasyDraw
    {
        private EasyDraw _thumb;
        private EasyDraw _fore;

        private float _value;
        
        public delegate void ValueChanged(float val);

        private ValueChanged _onValueChanged;
            
        public HudSlider(int width, int height, float startValue = 1.0f, bool addCollider = true) : base(width, height, addCollider)
        {
            Clear(Color.DarkGray);

            _fore = new EasyDraw(width - 4, height - 4, false);
            _fore.SetOrigin(0, -_fore.height / 2f);
            _fore.Clear(Color.LightGray);
            AddChild(_fore);
          
            _fore.x = 2;
            _fore.y = -_fore.height * 0.5f + 2;
         
            _thumb = new EasyDraw(10, height, false);
            _thumb.Clear(Color.Aqua);
            _thumb.Fill(Color.White);
            _thumb.Stroke(Color.Black);
            _thumb.ShapeAlign(CenterMode.Min, CenterMode.Min);
            _thumb.Rect(0, 0, _thumb.width-1, height-1);
            AddChild(_thumb);

            _thumb.x = 0;
            _thumb.y = 0;
            
            _thumb.SetOrigin(_thumb.width * 0.5f, 0);
        }

        void Update()
        {
            if (this.visible == false)
            {
                return;
            }
            
            var pos = new Vector2(x, y);
            var mousePoint = new Vector2(Input.mouseX,  Input.mouseY);
            var worldMousePoint = TransformPoint(mousePoint.x - pos.x, mousePoint.y - pos.y);

            if (Input.GetMouseButtonDown(0) && HitTestPoint(worldMousePoint.x, worldMousePoint.y))
            {
                var localPoint = InverseTransformPoint(worldMousePoint.x, worldMousePoint.y);

                float posX = Mathf.Clamp(localPoint.x, _thumb.width * 0.5f, width - _thumb.width * 0.5f);

                _thumb.x = posX;

                float lastVal = _value;
                _value = Mathf.Map(posX, _thumb.width * 0.5f, width - _thumb.width * 0.5f, 0, 1);

                if (Math.Abs(lastVal - _value) > 0.001f)
                {
                    _onValueChanged.Invoke(_value);
                }
                
                Console.WriteLine($"hit {Time.time} | mousepos: {mousePoint} | world: {worldMousePoint} | local: {localPoint} | val: {_value}");
            }
            
            DrawBoundBox();
        }

        private IEnumerator Anim(EasyDraw fore)
        {
            int time = 0;
            while (true)
            {
                float scaleX = Easing.Ease(Easing.Equation.CubicEaseOut, time % 800, 0.01f, 1f, 800);
                
                fore.SetScaleXY(scaleX, 1);
                
                time += Time.deltaTime;
                
                yield return null;
            }
        }
        
        public float Value => _value;

        public ValueChanged OnValueChanged
        {
            get => _onValueChanged;
            set => _onValueChanged = value;
        }

        void DrawBoundBox()
        {

            var p0 = this.TransformPoint(_bounds.left, _bounds.top);
            var p1 = this.TransformPoint(_bounds.right, _bounds.top);
            var p2 = this.TransformPoint(_bounds.right, _bounds.bottom);
            var p3 = this.TransformPoint(_bounds.left, _bounds.bottom);

            CanvasDebugger2.Instance.DrawLine(p0.x, p0.y, p1.x, p1.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p1.x, p1.y, p2.x, p2.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p2.x, p2.y, p3.x, p3.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p3.x, p3.y, p0.x, p0.y, Color.Blue);
        }
    }
}