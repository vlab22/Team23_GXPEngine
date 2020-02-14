using System;

namespace GXPEngine
{
    public static class MRandom
    {
        static Random rand = new Random(Time.now);

        public static float Range(float min, float max)
        {
            return min + (float)rand.NextDouble() * max;
        }
        
        public static int Range(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}