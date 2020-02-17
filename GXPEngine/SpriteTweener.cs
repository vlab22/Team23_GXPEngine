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


        public static void TweenAlpha(Sprite sprite, float from, float to, int duration)
        {
            if (_alphaRoutinesMap.ContainsKey(sprite))
            {
                CoroutineManager.StopCoroutine(_alphaRoutinesMap[sprite]);
                _alphaRoutinesMap.Remove(sprite);
            }

            var ie = CoroutineManager.StartCoroutine(TweenAlphaRoutine(sprite, from, to, duration), null);
            _alphaRoutinesMap.Add(sprite, ie);
        }

        private static IEnumerator TweenAlphaRoutine(Sprite sprite, float from, float to, int duration,
            Easing.Equation equation = Easing.Equation.QuadEaseOut)
        {
            int time = 0;

            sprite.alpha = from;

            float direction = from > to ? from : 0;

            while (time < duration)
            {
                sprite.alpha = Easing.Ease(equation, time, from, to - direction, duration);

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

        private static IEnumerator TweenColorRoutine(Sprite sprite, uint from, uint to, int duration,
            Easing.Equation equation)
        {
            int time = 0;

            sprite.color = from;

            byte fromA = (byte) (from >> 24);
            byte fromR = (byte) (from >> 16);
            byte fromG = (byte) (from >> 8);
            byte fromB = (byte) (from >> 0);

            byte toA = (byte) (to >> 24);
            byte toR = (byte) (to >> 16);
            byte toG = (byte) (to >> 8);
            byte toB = (byte) (to >> 0);

            byte redFromDir = fromR > toR ? fromR : (byte) 0;
            byte greenFromDir = fromG > toG ? fromG : (byte) 0;
            byte blueFromDir = fromB > toB ? fromB : (byte) 0;

            byte redToDir = fromR > toR ? (byte) 0 : fromR;
            byte greenToDir = fromG > toG ? (byte) 0 : fromG;
            byte blueToDir = fromB > toB ? (byte) 0 : fromB;

            while (time < duration)
            {
                int deltaRFrom = fromR - redFromDir;
                int deltaRTo = toR - redFromDir;

                byte r = (byte) (redToDir + Mathf.Round(Easing.Ease(equation, time, redFromDir, toR - fromR,
                    duration)));
                byte g = (byte) (greenToDir +
                                 Mathf.Round(Easing.Ease(equation, time, greenFromDir, toG - fromG,
                                     duration)));
                byte b = (byte) (blueToDir +
                                 Mathf.Round(Easing.Ease(equation, time, blueFromDir, toB - fromB,
                                     duration)));

                Console.WriteLine($"color: {deltaRFrom:000}|{deltaRTo:000} --- {r:000}|{g:000}|{b:000}");

                sprite.color = ColorTools.ColorToUInt(Color.FromArgb(Mathf.Round(sprite.alpha * 255), r, g, b));

                time += Time.deltaTime;
                yield return null;
            }

            sprite.color = to;
        }
    }
}