using System;
using System.Drawing;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class HunterBullet : AnimationSprite
    {
        private float _speed;
        private float _range;
        private float _lifeTime;
        private float _lifeTimeCounter;

        private GameObject _shooter;

        private Rectangle _customColliderBounds;


        public HunterBullet(float pSpeed = 200, float pRange = 500) : base("data/Hunter Bullet.png", 1, 1, -1, false, true)
        {
            _speed = pSpeed;
            _range = pRange;

            _lifeTime = _speed > 0 ? _range / _speed : 1000;
            
            SetOrigin(width * 0.5f, height * 0.5f);

            _customColliderBounds = new Rectangle(-5, -4, 10, 8);
        }

        void Update()
        {
            if (!this.Enabled) return;

            if (_lifeTimeCounter > _lifeTime)
            {
                //Fall bullet
                float scale = 0.001f + Easing.Ease(Easing.Equation.QuadEaseIn, _lifeTimeCounter - _lifeTime, 0.999f, 0 - 0.999f, 400);
                SetScaleXY(scale, scale);
                
                Console.WriteLine($"{this.name}: lifeTimeCounter: {_lifeTimeCounter} | lifeTime: {_lifeTime} | scale {scale}");

                if (_lifeTimeCounter > _lifeTime + 400)
                {
                    this.SetActive(false);
                }
            }
            _lifeTimeCounter += Time.deltaTime;
            
            Move(_speed * Time.delta, 0);

            if (MyGame.Debug)
            {
                DrawBoundBox();
            }
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

        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value; 
                _lifeTime = _speed > 0 ? (_range / _speed) * 1000 : 1000;
            }
        }

        public float Range
        {
            get => _range;
            set
            {
                _range = value; 
                _lifeTime = _speed > 0 ? (_range / _speed) * 1000 : 1000;
            }
        }

        public GameObject Shooter
        {
            get => _shooter;
            set => _shooter = value;
        }

        public float LifeTimeCounter
        {
            get => _lifeTimeCounter;
            set => _lifeTimeCounter = value;
        }
    }
}