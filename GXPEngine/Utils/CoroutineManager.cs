using System.Collections;
using System.Collections.Generic;
using GXPEngine;

/// <summary>
/// 
/// </summary>
public static class CoroutineManager
{
    static HashSet<YieldInstruction> waitSecondsRoutine = new HashSet<YieldInstruction>();
    static HashSet<IEnumerator> routines = new HashSet<System.Collections.IEnumerator>();
    static HashSet<IEnumerator> routinesToAdd = new HashSet<System.Collections.IEnumerator>();
    static HashSet<IEnumerator> routinesToRemove = new HashSet<System.Collections.IEnumerator>();

    static Dictionary<IEnumerator, IEnumerator> routineWaitMap = new Dictionary<IEnumerator, IEnumerator>();

    private static bool _isIterating;

    public static IEnumerator StartCoroutine(IEnumerator ie)
    {
        if (_isIterating)
        {
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
        routinesToRemove.Add(ie);
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
            if (ie.Current == null)
            {
                if (ie.MoveNext() == false)
                {
                    routinesToRemove.Add(ie);
                    
                    //Happen when a IEnumerator is yield inside another IEnumerator (chained)
                    //if this ie has a parentIe, this ie will be removed from the loop and the parent re-added
                    if (routineWaitMap.TryGetValue(ie, out var parentIe))
                    {
                        if (parentIe.MoveNext() == false)
                        {
                            routinesToRemove.Add(parentIe);
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
                    routinesToRemove.Add(ie);
                    if (routineWaitMap.TryGetValue(ie, out var parentIe))
                    {
                        if (parentIe.MoveNext() == false)
                        {
                            routinesToRemove.Add(parentIe);
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
                StartCoroutine(childIe);
                routineWaitMap.Add(childIe, ie);
                routinesToRemove.Add(ie);
            }
        }

        routines.ExceptWith(routinesToRemove);
        routinesToRemove.Clear();

        _isIterating = false;
    }

    public static string GetDiagnostics()
    {
        return "Total routines: " + routines.Count;
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