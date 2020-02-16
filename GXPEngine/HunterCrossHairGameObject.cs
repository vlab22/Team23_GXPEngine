using System.Drawing;

namespace GXPEngine
{
    public class HunterCrossHairGameObject : AnimationSprite
    {
        private HunterGameObject _hunter;
        
        public HunterCrossHairGameObject(HunterGameObject pHunter) : base("data/Hunter Crosshair00.png", 1, 1)
        {
            _hunter = pHunter;
            
            SetOrigin(width * 0.5f, height * 0.5f);
        }

        void Update()
        {
            if (MyGame.Debug)
            {
                DrawBoundBox();
            }
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