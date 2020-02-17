using System.Drawing;

namespace GXPEngine
{
    public static class ColorTools
    {
        public static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) |
                          (color.G << 8)  | (color.B << 0));
        }
    }
}