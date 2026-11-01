using System.Collections.Generic;
using System;

public static class GlobalEventBus
{
    private readonly static Dictionary<Type, Delegate> events = new Dictionary<Type, Delegate>();

    public static void Invoke<T>(T eventData) where T : IGlobalEvent
    {
        Type type = typeof(T);

        if (events.TryGetValue(type, out Delegate @delegate))
        {
            if (@delegate is Action<T> action)
            {
                action.Invoke(eventData);
            }
        }
    }

    public static void Subscribe<T>(Action<T> action) where T : IGlobalEvent
    {
        Type type = typeof(T);

        if (events.ContainsKey(type))
        {
            events[type] = Delegate.Combine(events[type], action);
        }
        else
        {
            events[type] = action;
        }
    }

    public static void Unsubscribe<T>(Action<T> action) where T : IGlobalEvent
    {
        Type type = typeof(T);

        if (events.ContainsKey(type))
        {
            events[type] = Delegate.Remove(events[type], action);

            if (events[type] == null)
            {
                events.Remove(type);
            }
        }
    }
}

public interface IGlobalEvent { }