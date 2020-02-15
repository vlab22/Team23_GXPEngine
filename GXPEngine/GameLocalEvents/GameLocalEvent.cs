using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Use LocalEvents for AddListeners
/// </summary>
public class GameLocalEvent
{

}

public class LocalEvents
{
    private static LocalEvents _eventsInstance = null;

    public static LocalEvents Instance
    {
        get
        {
            if (_eventsInstance == null)
            {
                _eventsInstance = new LocalEvents ();
            }

            return _eventsInstance;
        }
    }

    public List<Delegate> Delegates => _delegates.Values.ToList();

    public delegate void EventDelegate<T> (T e) where T : GameLocalEvent;

    private Dictionary<System.Type, System.Delegate> _delegates = new Dictionary<System.Type, System.Delegate> ();

    public void AddListener<T> (EventDelegate<T> del) where T : GameLocalEvent
    {
        if (_delegates.ContainsKey (typeof (T)))
        {
            System.Delegate tempDel = _delegates[typeof (T)];

            _delegates[typeof (T)] = System.Delegate.Combine (tempDel, del);
        }
        else
        {
            _delegates[typeof (T)] = del;
        }
    }

    public void RemoveListener<T> (EventDelegate<T> del) where T : GameLocalEvent
    {
        if (_delegates.ContainsKey (typeof (T)))
        {
            var currentDel = System.Delegate.Remove (_delegates[typeof (T)], del);

            if (currentDel == null)
            {
                _delegates.Remove (typeof (T));
            }
            else
            {
                _delegates[typeof (T)] = currentDel;
            }
        }
    }

    public void Raise (GameLocalEvent e)
    {
        if (e == null)
        {
            return;
        }

        if (_delegates.ContainsKey (e.GetType ()))
        {
            _delegates[e.GetType ()].DynamicInvoke (e);
        }
    }
}