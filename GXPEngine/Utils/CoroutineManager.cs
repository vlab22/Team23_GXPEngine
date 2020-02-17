﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GXPEngine;

/// <summary>
/// This class was inspired in Coroutines of Unity
/// </summary>
public static class CoroutineManager
{
    static HashSet<YieldInstruction> waitSecondsRoutine = new HashSet<YieldInstruction>();
    static HashSet<IEnumerator> routines = new HashSet<System.Collections.IEnumerator>();
    static HashSet<IEnumerator> routinesToAdd = new HashSet<System.Collections.IEnumerator>();
    static HashSet<IEnumerator> routinesToRemove = new HashSet<System.Collections.IEnumerator>();

    static Dictionary<IEnumerator, IEnumerator> routineWaitMap = new Dictionary<IEnumerator, IEnumerator>();

    static Dictionary<IEnumerator, GameObject> routinesInvokerMap = new Dictionary<IEnumerator, GameObject>();

    private static bool _isIterating;

    public static IEnumerator StartCoroutine(IEnumerator ie, GameObject invoker)
    {
        if (_isIterating)
        {
            if (invoker != null)
                routinesInvokerMap.Add(ie, invoker);
            
            ie.MoveNext();
            routinesToAdd.Add(ie);
        }
        else
        {
            ie.MoveNext();
            routines.Add(ie);
        }

        return ie;
    }

    public static void StopCoroutine(IEnumerator ie)
    {
        RemoveRoutine(ie);
    }

    public static void Tick(int delta)
    {
        _isIterating = true;

        if (routinesToAdd.Count > 0)
        {
            routines.UnionWith(routinesToAdd);
            routinesToAdd.Clear();
        }

        foreach (var ie in routines)
        {
            if (routinesInvokerMap.TryGetValue(ie, out var gameObject))
            {
                if (!gameObject.Enabled)
                {
                    continue;
                }
            }
            
            if (ie.Current == null)
            {
                if (ie.MoveNext() == false)
                {
                    RemoveRoutine(ie);

                    //Happen when a IEnumerator is yield inside another IEnumerator (chained)
                    //if this ie has a parentIe, this ie will be removed from the loop and the parent re-added
                    if (routineWaitMap.TryGetValue(ie, out var parentIe))
                    {
                        if (parentIe.MoveNext() == false)
                        {
                            RemoveRoutine(parentIe);
                        }
                        else
                        {
                            routinesToAdd.Add(parentIe);
                        }

                        routineWaitMap.Remove(ie);
                    }
                }
            }
            else if (ie.Current is YieldInstruction yieldObj)
            {
                if (yieldObj.YieldAndEnd(delta) == false)
                {
                    continue;
                }

                if (ie.MoveNext() == false)
                {
                    RemoveRoutine(ie);
                    if (routineWaitMap.TryGetValue(ie, out var parentIe))
                    {
                        if (parentIe.MoveNext() == false)
                        {
                            RemoveRoutine(parentIe);
                        }
                        else
                        {
                            routinesToAdd.Add(parentIe);
                        }

                        routineWaitMap.Remove(ie);
                    }
                }
            }
            else
            {
                //Happen when a IEnumerator is yield inside another IEnumerator (chained)
                //Saves the parent ie and after the child ie ends, the parentIE is added to the loop
                IEnumerator childIe = (IEnumerator) ie.Current;
                StartCoroutine(childIe, routinesInvokerMap.ContainsKey(ie) ? routinesInvokerMap[ie] : null);
                routineWaitMap.Add(childIe, ie);
                RemoveRoutine(ie);
            }
        }

        //Console.WriteLine("=========================");
        //Console.WriteLine(string.Join(Environment.NewLine, routines.Where(ii => ii.ToString().Contains("Blink"))));

        routines.ExceptWith(routinesToRemove);
        routinesToRemove.Clear();

        _isIterating = false;
    }

    private static void RemoveRoutine(IEnumerator ie)
    {
        if (ie == null) return;
        
        routinesToRemove.Add(ie);
        
        if (routinesInvokerMap.ContainsKey(ie))
            routinesInvokerMap.Remove(ie);
    }

    public static string GetDiagnostics()
    {
        return "Total routines: " + routines.Count;
    }

    public static void ClearAllRoutines()
    {
        routines.Clear();
        routinesToAdd.Clear();
        routinesToRemove.Clear();
        routineWaitMap.Clear();
    }
}

/// <summary>
/// Base class for all types of YieldInstruction
/// </summary>
public abstract class YieldInstruction
{
    public abstract bool YieldAndEnd(int delta);
}

/// <summary>
/// 
/// </summary>
public class WaitForMilliSeconds : YieldInstruction
{
    public int duration;
    public int timeElapsed;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration">in milliseconds, ex: 1000 equal 1 second</param>
    public WaitForMilliSeconds(int duration)
    {
        this.duration = duration;
    }

    public override bool YieldAndEnd(int delta)
    {
        this.timeElapsed += delta;
        if (this.timeElapsed >= this.duration)
        {
            return true;
        }

        return false;
    }
}