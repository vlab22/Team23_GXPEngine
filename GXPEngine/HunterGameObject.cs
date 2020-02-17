using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
    public class HunterGameObject : AnimationSprite
    {
        private IHunterBehaviorListener _hunterBehaviorListener;
        private bool _hasHunterBehaviourListener;

        private float _scanEnemyRange;
        private float _sightSpeed;
        private AnimationSprite _crossHair;

        public HunterGameObject(float pX, float pY, float pWidth, float pHeight,
            float pScanEnemyRange = 400, float pSightSpeed = 300) : base("data/Female Hunter.png", 1, 1,
            -1, false, false)
        {
            _scanEnemyRange = pScanEnemyRange;
            _sightSpeed = pSightSpeed;
            
            float originalWidth = width;
            float originalHeight = height;

            SetOrigin(0, height);

            x = pX + pWidth * 0.5f;
            y = pY - pHeight * 0.5f;

            SetOrigin(width * 0.5f, height * 0.5f);

            _crossHair = new HunterCrossHairGameObject(this);
            AddChild(_crossHair);
            _crossHair.SetXY(0,0);

            // var easyDraw = new EasyDraw(width, height);
            // easyDraw.SetOrigin(width * 0.5f, height * 0.5f);
            // easyDraw.Clear(Color.FromArgb(150, 1, 1, 1));
            // AddChild(easyDraw);
        }

        public IHunterBehaviorListener HunterBehaviorListener
        {
            get => _hunterBehaviorListener;
            set
            {
                _hunterBehaviorListener = value;
                _hasHunterBehaviourListener = value != null;
            }
        }

        void Update()
        {
            if (!this.Enabled) return;
        }
    }

    public interface IHunterBehaviorListener
    {
    }
}