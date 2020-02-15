using System.Drawing;
using GXPEngine.Core;
using MathfExtensions;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class DroneGameObject : AnimationSprite
    {
        private float _speed;
        private Rectangle _customColliderBounds;


        public DroneGameObject(float pX, float pY, float pWidth, float pHeight, float pSpeed = 200,
            float pRotation = 0) : base(
            "data/Drone spritesheet small.png", 3, 3, 5, false, true)
        {
            _customColliderBounds = new Rectangle(-27, -24, 53, 50);
            
            _speed = pSpeed;

            float originalWidth = width;
            float originalHeight = height;

            SetOrigin(0, height);

            float lScaleX = pWidth / width;
            float lScaleY = pHeight / height;

            SetScaleXY(lScaleX, lScaleY);

            x = pX; // + width / 2;
            y = pY; // - height + height / 2f;

            float diag = Mathf.Sqrt(width * width + height * height);

            SetOrigin(originalWidth / 2f, originalHeight / 2f);

            Turn(pRotation);

            x = pX + Mathf.Cos(rotation.DegToRad()) * width * 0.5f;
            y = pY + Mathf.Sin(rotation.DegToRad()) * width * 0.5f;

            var pos = new Vector2(x - pX, y - pY);
            var perp = new Vector2(pos.y, -pos.x).Normalized;

            pos = perp * height * 0.5f;
            
            SetScaleXY(1, 1);

            x += pos.x;
            y += pos.y;
        }

        void Update()
        {
            DrawBoundBox();
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
        
        void DrawBoundBox()
        {
            var p0 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.top);
            var p1 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.top);
            var p2 = this.TransformPoint(_customColliderBounds.right, _customColliderBounds.bottom);
            var p3 = this.TransformPoint(_customColliderBounds.left, _customColliderBounds.bottom);

            CanvasDebugger2.Instance.DrawLine(p0.x, p0.y, p1.x, p1.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p1.x, p1.y, p2.x, p2.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p2.x, p2.y, p3.x, p3.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p3.x, p3.y, p0.x, p0.y, Color.Blue);
        }
    }
}