using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public static class IntegerTweener
    {
        static Dictionary<IHasTweenInteger, IEnumerator> _tweenMap = new Dictionary<IHasTweenInteger, IEnumerator>();
        
        public static void TweenInteger(IHasTweenInteger tweened, int from, int to, int duration = 400, int delay = 0, Easing.Equation equation = Easing.Equation.QuadEaseOut)
        {
            if (_tweenMap.TryGetValue(tweened, out var ieRunning))
            {
                CoroutineManager.StopCoroutine(ieRunning);
                _tweenMap.Remove(tweened);
            }
            
            var ie = CoroutineManager.StartCoroutine(TweenIntegerRoutine(tweened, from, to, duration, delay, equation), null);
            _tweenMap.Add(tweened, ie);
        }

        private static IEnumerator TweenIntegerRoutine(IHasTweenInteger tweened, int @from, int to, int duration, int delay, Easing.Equation equation)
        {
            if (delay > 0)
            {
                yield return new WaitForMilliSeconds(delay);
            }

            int time = 0;
            
            float fromDir = from > to ? from : 0;
            float toDir = from > to ? 0 : from;

            while (time < duration)
            {
                tweened.IntValue = Mathf.Round(toDir + Easing.Ease(equation, time, fromDir, to - from, duration));

                time += Time.deltaTime;
                yield return null;
            }

            tweened.IntValue = to;
            
            if (_tweenMap.ContainsKey(tweened))
            {
                _tweenMap.Remove(tweened);
            }
        }
    }

    public interface IHasTweenInteger
    {
        int IntValue { get; set; }
    }
}