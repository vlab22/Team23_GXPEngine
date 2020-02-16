using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public class SpriteTweener
    {
        static Dictionary<Sprite, IEnumerator> _routinesMap = new Dictionary<Sprite, IEnumerator>();
        
        public static void TweenAlpha(Sprite sprite, float from, float to, int duration)
        {
            if (_routinesMap.ContainsKey(sprite))
            {
                CoroutineManager.StopCoroutine(_routinesMap[sprite]);
                _routinesMap.Remove(sprite);
            }

            var ie = CoroutineManager.StartCoroutine(TweenAlphaRoutine(sprite, from, to, duration));
            _routinesMap.Add(sprite, ie);
        }

        private static IEnumerator TweenAlphaRoutine(Sprite sprite, float from, float to, int duration, Easing.Equation equation = Easing.Equation.QuadEaseOut)
        {
            int time = 0;

            sprite.alpha = from;
            
            if (from > to)
            {
                while (time < duration)
                {
                    sprite.alpha = Easing.Ease(equation, time, from, to - from, duration);

                    time += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (time < duration)
                {
                    sprite.alpha = Easing.Ease(equation, time, from, to, duration);

                    time += Time.deltaTime;
                    yield return null;
                }
            }

            sprite.alpha = to;
        }
    }
}