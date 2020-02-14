namespace GXPEngine
{
    public class PizzaGameObject : Sprite
    {
        public PizzaGameObject(string filename, bool keepInCache = false, bool addCollider = false) : base(filename, keepInCache, addCollider)
        {
            SetOrigin(width * 0.5f, height * 0.5f);
        }
    }
}