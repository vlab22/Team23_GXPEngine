using GXPEngine.Core;

namespace GXPEngine
{
    public interface IGridDataUpdater
    {
        void OnMove(Vector2 pos, Vector2 lastPos);
    }
}