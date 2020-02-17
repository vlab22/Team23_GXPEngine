using System;
using System.Drawing;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class HunterCrossHairGameObject : AnimationSprite
    {
        private HunterGameObject _hunter;
        private Rectangle _customColliderBounds;

        private uint _startColor;
        
        private bool _hasCollisionWithEnemy = false;

        public HunterCrossHairGameObject(HunterGameObject pHunter) : base("data/Hunter Crosshair00.png", 1, 1)
        {
            _hunter = pHunter;

           color = _startColor = 0x6acedd; //Light blue from stylesheet

            SetOrigin(width * 0.5f, height * 0.5f);

            _customColliderBounds = new Rectangle(-44 * 0.5f, -44 * 0.5f, 44, 44);
        }
        
        void Update()
        {
            if (!this.Enabled) return;

            if (MyGame.Debug)
            {
                DrawBoundBox();
            }
        }

        void OnCollision(GameObject other)
        {
            if (!_hasCollisionWithEnemy && other is Stork)
            {
                _hasCollisionWithEnemy = true;
                _hunter.OnCollisionWithEnemy(other);

                Console.WriteLine($"{this.name} ==> {other}");
            }
        }

        public void Reset()
        {
            SetXY(0, 0);
            _hasCollisionWithEnemy = false;
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
        
        public uint StartColor => _startColor;
    }
}