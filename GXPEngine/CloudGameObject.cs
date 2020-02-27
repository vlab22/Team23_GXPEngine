using System;
using System.Drawing;

namespace GXPEngine
{
    public class CloudGameObject : AnimationSprite
    {
        private int _animationSpeed;
        private int _animationTime = 0;
        private int _animationDirection;


        private static Bitmap _Bitmap = new Bitmap("data/clouds_spritesheet.png");

        public CloudGameObject(int pAnimationDirection) : base(_Bitmap, 5, 3, -1, false)
        {
            _animationDirection = pAnimationDirection;
            _animationSpeed = 300 * frameCount;

            // var easy = new EasyDraw(width, height);
            // easy.Clear(0,0,0);
            // AddChild(easy);
        }

        void Update()
        {
            int frame;

            if (_animationDirection >= 0)
            {
                frame = Mathf.Round(Mathf.Map(_animationTime, 0, _animationSpeed, 0, frameCount - 1));
            }
            else
            {
                frame = Mathf.Round(Mathf.Map(_animationTime, 0, _animationSpeed, frameCount - 1, 0));
            }

            SetFrame(frame);

            _animationTime += Time.deltaTime;
            _animationTime = _animationTime % _animationSpeed;
        }
    }
}