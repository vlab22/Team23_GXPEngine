using GXPEngine.Core;

namespace GXPEngine
{
    public class CompoundCollider : Sprite
    {
        private IHasCompoundCollider _compoundColliderListener;
        private bool _hasCompoundColliderListener;

        public CompoundCollider(float px, float py, int pWidth, int pHeight) : base("data/CompoundColliderSprite.png")
        {
            x = px;
            y = py;
            width = pWidth;
            height = pHeight;
        }

        public IHasCompoundCollider CompoundColliderListener
        {
            get => _compoundColliderListener;
            set
            {
                _compoundColliderListener = value;
                _hasCompoundColliderListener = value != null;
            }
        }

        void Update()
        {
            visible = MyGame.Debug;
        }

    }

    public interface IHasCompoundCollider
    {
        GameObject Parent { get; }
    }
    
    
}