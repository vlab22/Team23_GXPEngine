using GXPEngine.Core;

namespace GXPEngine
{
    public class SmallSnowFlakeParticle : Sprite
    {
        private GameObject _target;
        private Vector2 _offset;
        private Vector2 _offset2;

        public SmallSnowFlakeParticle() : base("data/small snowflakee.png", true, false)
        {
            SetOriginToCenter();
        }

        public void Show(GameObject target, Vector2 pos, Vector2 offset2)
        {
            _target = target;
            _offset = pos;
            _offset2 = offset2;

            this.SetScaleXY(1f, 1f);
            this.alpha = 1;
            SpriteTweener.TweenScale(this, 1f, 2, 400, o =>
            {
                parent?.RemoveChild(this);
                _target = null;
            });
            //SpriteTweener.TweenAlpha(this, 1, 0.4f, 400);
        }

        void Update()
        {
            if (!Enabled || _target == null) return;

            SetXY(_target.x + _offset.x + _offset2.x, _target.y + _offset.y + _offset2.y);
        }
    }
}