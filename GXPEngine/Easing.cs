﻿using System;
using GXPEngine;

/**
 * Easing
 * Animates the value of a float property between two target values using 
 * Robert Penner's easing equations for interpolation over a specified Duration.
 *
 * Original Author:  Darren David darren-code@lookorfeel.com
 *
 * Ported to be easily used in Unity by Marco Mastropaolo
 *
 * Credit/Thanks:
 * Robert Penner - The easing equations we all know and love 
 *   (http://robertpenner.com/easing/) [See License.txt for license info]
 * 
 * Lee Brimelow - initial port of Penner's equations to WPF 
 *   (http://thewpfblog.com/?p=12)
 * 
 * Zeh Fernando - additional equations (out/in) from 
 *   caurina.transitions.Tweener (http://code.google.com/p/tweener/)
 *   [See License.txt for license info]
 */
/// <summary>
/// Animates the value of a float property between two target values using 
/// Robert Penner's easing equations for interpolation over a specified Duration.
/// </summary>
/// <example>
/// <code>
/// // C#
/// PennerDoubleAnimation anim = new PennerDoubleAnimation();
/// anim.Type = PennerDoubleAnimation.Equations.Linear;
/// anim.From = 1;
/// anim.To = 0;
/// myControl.BeginAnimation( OpacityProperty, anim );
/// 
/// // XAML
/// <Storyboard x:Key="AnimateXamlRect">
///  <animation:PennerDoubleAnimation 
///    Storyboard.TargetName="myControl" 
///    Storyboard.TargetProperty="(Canvas.Left)"
///    From="0" 
///    To="600" 
///    Equation="BackEaseOut" 
///    Duration="00:00:05" />
/// </Storyboard>
/// 
/// <Control.Triggers>
///   <EventTrigger RoutedEvent="FrameworkElement.Loaded">
///     <BeginStoryboard Storyboard="{StaticResource AnimateXamlRect}"/>
///   </EventTrigger>
/// </Control.Triggers>
/// </code>
/// </example>
public static class Easing
{
    public enum Equation
    {
        Linear,
        ExpoEaseInOut,
        ExpoEaseOut,
        ExpoEaseIn,
        ExpoEaseOutIn,
        CircEaseOut,
        CircEaseIn,
        CircEaseInOut,
        CircEaseOutIn,
        QuadEaseOut,
        QuadEaseIn,
        QuadEaseInOut,
        QuadEaseOutIn,
        SineEaseOut,
        SineEaseIn,
        SineEaseInOut,
        SineEaseOutIn,
        CubicEaseOut,
        CubicEaseIn,
        CubicEaseInOut,
        CubicEaseOutIn,
        QuartEaseIn,
        QuartEaseOut,
        QuartEaseInOut,
        QuartEaseOutIn,
        QuintEaseIn,
        QuintEaseOut,
        QuintEaseInOut,
        QuintEaseOutIn,
        ElasticEaseIn,
        ElasticEaseOut,
        ElasticEaseInOut,
        ElasticEaseOutIn,
        BounceEaseIn,
        BounceEaseOut,
        BounceEaseInOut,
        BounceEaseOutIn,
        BackEaseIn,
        BackEaseOut,
        BackEaseInOut,
        BackEaseOutIn,
    };


    public static float Ease(Equation equation, float t, float b, float c, float d)
    {
        switch (equation)
        {
            case Equation.ExpoEaseInOut:
                return ExpoEaseInOut(t, b, c, d);
            case Equation.ExpoEaseOut:
                return ExpoEaseOut(t, b, c, d);
            case Equation.ExpoEaseIn:
                return ExpoEaseIn(t, b, c, d);
            case Equation.ExpoEaseOutIn:
                return ExpoEaseOutIn(t, b, c, d);
            case Equation.CircEaseOut:
                return CircEaseOut(t, b, c, d);
            case Equation.CircEaseIn:
                return CircEaseIn(t, b, c, d);
            case Equation.CircEaseInOut:
                return CircEaseInOut(t, b, c, d);
            case Equation.CircEaseOutIn:
                return CircEaseOutIn(t, b, c, d);
            case Equation.QuadEaseOut:
                return QuadEaseOut(t, b, c, d);
            case Equation.QuadEaseIn:
                return QuadEaseIn(t, b, c, d);
            case Equation.QuadEaseInOut:
                return QuadEaseInOut(t, b, c, d);
            case Equation.QuadEaseOutIn:
                return QuadEaseOutIn(t, b, c, d);
            case Equation.SineEaseOut:
                return SineEaseOut(t, b, c, d);
            case Equation.SineEaseIn:
                return SineEaseIn(t, b, c, d);
            case Equation.SineEaseInOut:
                return SineEaseInOut(t, b, c, d);
            case Equation.SineEaseOutIn:
                return SineEaseOutIn(t, b, c, d);
            case Equation.CubicEaseOut:
                return CubicEaseOut(t, b, c, d);
            case Equation.CubicEaseIn:
                return CubicEaseIn(t, b, c, d);
            case Equation.CubicEaseInOut:
                return CubicEaseInOut(t, b, c, d);
            case Equation.CubicEaseOutIn:
                return CubicEaseOutIn(t, b, c, d);
            case Equation.QuartEaseIn:
                return QuartEaseIn(t, b, c, d);
            case Equation.QuartEaseOut:
                return QuartEaseOut(t, b, c, d);
            case Equation.QuartEaseInOut:
                return QuartEaseInOut(t, b, c, d);
            case Equation.QuartEaseOutIn:
                return QuartEaseOutIn(t, b, c, d);
            case Equation.QuintEaseIn:
                return QuintEaseIn(t, b, c, d);
            case Equation.QuintEaseOut:
                return QuintEaseOut(t, b, c, d);
            case Equation.QuintEaseInOut:
                return QuintEaseInOut(t, b, c, d);
            case Equation.QuintEaseOutIn:
                return QuintEaseOutIn(t, b, c, d);
            case Equation.ElasticEaseIn:
                return ElasticEaseIn(t, b, c, d);
            case Equation.ElasticEaseOut:
                return ElasticEaseOut(t, b, c, d);
            case Equation.ElasticEaseInOut:
                return ElasticEaseInOut(t, b, c, d);
            case Equation.ElasticEaseOutIn:
                return ElasticEaseOutIn(t, b, c, d);
            case Equation.BounceEaseIn:
                return BounceEaseIn(t, b, c, d);
            case Equation.BounceEaseOut:
                return BounceEaseOut(t, b, c, d);
            case Equation.BounceEaseInOut:
                return BounceEaseInOut(t, b, c, d);
            case Equation.BounceEaseOutIn:
                return BounceEaseOutIn(t, b, c, d);
            case Equation.BackEaseIn:
                return BackEaseIn(t, b, c, d);
            case Equation.BackEaseOut:
                return BackEaseOut(t, b, c, d);
            case Equation.BackEaseInOut:
                return BackEaseInOut(t, b, c, d);
            case Equation.BackEaseOutIn:
                return BackEaseOutIn(t, b, c, d);
            case Equation.Linear:
                return Linear(t, b, c, d);
            default:
                Console.WriteLine(((int) equation) + " is not one of the availabled equations");
                return Linear(t, b, c, d);
        }
    }

    #region Equations

    // These methods are all public to enable reflection in GetCurrentValueCore.

    #region Linear

    /// <summary>
    /// Easing equation function for a simple linear tweening, with no easing.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float Linear(float t, float b, float c, float d)
    {
        return c * t / d + b;

        //float value = t / d;
        //return Mathf.Map(value, 0, 1, @b, c);
    }

    #endregion

    #region Expo

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseOut(float t, float b, float c, float d)
    {
        return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseIn(float t, float b, float c, float d)
    {
        return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseInOut(float t, float b, float c, float d)
    {
        if (t == 0)
            return b;

        if (t == d)
            return b + c;

        if ((t /= d / 2) < 1)
            return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;

        return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
    }

    /// <summary>
    /// Easing equation function for an exponential (2^t) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ExpoEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return ExpoEaseOut(t * 2, b, c / 2, d);

        return ExpoEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Circular

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseOut(float t, float b, float c, float d)
    {
        return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseIn(float t, float b, float c, float d)
    {
        return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;

        return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
    }

    /// <summary>
    /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CircEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return CircEaseOut(t * 2, b, c / 2, d);

        return CircEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Quad

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseOut(float t, float b, float c, float d)
    {
        return -c * (t /= d) * (t - 2) + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t + b;

        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    }

    /// <summary>
    /// Easing equation function for a quadratic (t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuadEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return QuadEaseOut(t * 2, b, c / 2, d);

        return QuadEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Sine

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseOut(float t, float b, float c, float d)
    {
        return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseIn(float t, float b, float c, float d)
    {
        return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * (Mathf.Sin(Mathf.PI * t / 2)) + b;

        return -c / 2 * (Mathf.Cos(Mathf.PI * --t / 2) - 2) + b;
    }

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return SineEaseOut(t * 2, b, c / 2, d);

        return SineEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Cubic

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseOut(float t, float b, float c, float d)
    {
        return c * ((t = t / d - 1) * t * t + 1) + b;
        // float value = t / d;
        // value--;
        // c -= b;
        // return c * (value * value * value + 1) + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t + b;

        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    /// <summary>
    /// Easing equation function for a cubic (t^3) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float CubicEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return CubicEaseOut(t * 2, b, c / 2, d);

        return CubicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Quartic

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseOut(float t, float b, float c, float d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t * t + b;

        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    /// <summary>
    /// Easing equation function for a quartic (t^4) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuartEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return QuartEaseOut(t * 2, b, c / 2, d);

        return QuartEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Quintic

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseOut(float t, float b, float c, float d)
    {
        return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * t * t * t + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }

    /// <summary>
    /// Easing equation function for a quintic (t^5) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float QuintEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return QuintEaseOut(t * 2, b, c / 2, d);
        return QuintEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Elastic

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseOut(float t, float b, float c, float d)
    {
        if ((t /= d) == 1)
            return b + c;

        float p = d * .3f;
        float s = p / 4;

        return (c * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b);
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseIn(float t, float b, float c, float d)
    {
        if ((t /= d) == 1)
            return b + c;

        float p = d * .3f;
        float s = p / 4;

        return -(c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) == 2)
            return b + c;

        float p = d * (.3f * 1.5f);
        float s = p / 4;

        if (t < 1)
            return -.5f * (c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
        return c * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + c + b;
    }

    /// <summary>
    /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float ElasticEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return ElasticEaseOut(t * 2, b, c / 2, d);
        return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Bounce

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseOut(float t, float b, float c, float d)
    {
        if ((t /= d) < (1f / 2.75f))
            return c * (7.5625f * t * t) + b;
        else if (t < (2f / 2.75f))
            return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
        else if (t < (2.5f / 2.75f))
            return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
        else
            return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseIn(float t, float b, float c, float d)
    {
        return c - BounceEaseOut(d - t, 0, c, d) + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseInOut(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return BounceEaseIn(t * 2, 0, c, d) * .5f + b;
        else
            return BounceEaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
    }

    /// <summary>
    /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BounceEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return BounceEaseOut(t * 2, b, c / 2, d);
        return BounceEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #region Back

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
    /// decelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseOut(float t, float b, float c, float d)
    {
        return c * ((t = t / d - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
    /// accelerating from zero velocity.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t * ((1.70158f + 1) * t - 1.70158f) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseInOut(float t, float b, float c, float d)
    {
        float s = 1.70158f;
        if ((t /= d / 2) < 1)
            return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
        return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
    }

    /// <summary>
    /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
    /// deceleration until halfway, then acceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float BackEaseOutIn(float t, float b, float c, float d)
    {
        if (t < d / 2)
            return BackEaseOut(t * 2, b, c / 2, d);
        return BackEaseIn((t * 2) - d, b + c / 2, c / 2, d);
    }

    #endregion

    #endregion
}