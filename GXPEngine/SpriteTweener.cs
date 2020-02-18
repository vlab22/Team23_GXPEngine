using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine
{
    public class SpriteTweener
    {
        static Dictionary<Sprite, IEnumerator> _alphaRoutinesMap = new Dictionary<Sprite, IEnumerator>();
        static Dictionary<Sprite, IEnumerator> _colorRoutinesMap = new Dictionary<Sprite, IEnumerator>();


        public static void TweenAlpha(Sprite sprite, float from, float to, int duration, int delay = 0, Easing.Equation equation = Easing.Equation.QuadEaseOut)
        {
            if (_alphaRoutinesMap.ContainsKey(sprite))
            {
                CoroutineManager.StopCoroutine(_alphaRoutinesMap[sprite]);
                _alphaRoutinesMap.Remove(sprite);
            }

            var ie = CoroutineManager.StartCoroutine(TweenAlphaRoutine(sprite, from, to, duration, delay, equation), null);
            _alphaRoutinesMap.Add(sprite, ie);
        }

        private static IEnumerator TweenAlphaRoutine(Sprite sprite, float from, float to, int duration, int delay,
            Easing.Equation equation)
        {
            //TODO: remove this call to WaitForMilliSeconds and implement it in the while loop to prevent another instantiation of a yield
            if (delay > 0)
                yield return new WaitForMilliSeconds(delay);

            int time = 0;

            sprite.alpha = from;

            float fromDir = from > to ? from : 0;
            float toDir = from > to ? 0 : from;

            while (time < duration)
            {
                sprite.alpha = toDir + Easing.Ease(equation, time, fromDir, to - from, duration);

                time += Time.deltaTime;
                yield return null;
            }

            sprite.alpha = to;
        }

        public static void TweenColor(Sprite sprite, uint from, uint to, int duration = 400,
            Easing.Equation equation = Easing.Equation.QuadEaseOut)
        {
            if (_colorRoutinesMap.ContainsKey(sprite))
            {
                CoroutineManager.StopCoroutine(_colorRoutinesMap[sprite]);
                _colorRoutinesMap.Remove(sprite);
            }

            var ie = CoroutineManager.StartCoroutine(TweenColorRoutine(sprite, from, to, duration, equation), null);
            _colorRoutinesMap.Add(sprite, ie);
        }

        public static IEnumerator TweenColorRoutine(Sprite sprite, uint from, uint to, int duration,
            Easing.Equation equation)
        {
            int time = 0;

            sprite.color = from;

            //float fromA = ((byte) (from >> 24)) / 255f;
            float fromR = ((byte) (from >> 16)) / 255f;
            float fromG = ((byte) (from >> 8)) / 255f;
            float fromB = ((byte) (from >> 0)) / 255f;

            //float toA = ((byte) (to >> 24)) / 255f;
            float toR = ((byte) (to >> 16)) / 255f;
            float toG = ((byte) (to >> 8)) / 255f;
            float toB = ((byte) (to >> 0)) / 255f;

            float redFromDir = fromR > toR ? fromR : 0;
            float greenFromDir = fromG > toG ? fromG : 0;
            float blueFromDir = fromB > toB ? fromB : 0;

            float redToDir = fromR > toR ? 0 : fromR;
            float greenToDir = fromG > toG ? 0 : fromG;
            float blueToDir = fromB > toB ? 0 : fromB;

            while (time < duration)
            {
                float r = redToDir + Easing.Ease(equation, time, redFromDir, toR - fromR,
                    duration);
                float g = greenToDir +
                          Easing.Ease(equation, time, greenFromDir, toG - fromG,
                              duration);
                float b = blueToDir +
                          Easing.Ease(equation, time, blueFromDir, toB - fromB,
                              duration);

                sprite.SetColor(r, g, b);

                time += Time.deltaTime;
                yield return null;
            }

            sprite.color = to;
        }
    }
}