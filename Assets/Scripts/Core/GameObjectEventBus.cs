using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameObjectEventBus
{
    private readonly static Dictionary<Type, Dictionary<GameObject, List<Callback>>> events = new Dictionary<Type, Dictionary<GameObject, List<Callback>>>();

    public static void Invoke<T>(GameObject ownerRef, T eventData)
    {
        Type type = eventData.GetType();

        if (events.TryGetValue(type, out Dictionary<GameObject, List<Callback>> inner))
        {
            if (inner.TryGetValue(ownerRef, out List<Callback> callbacks))
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i].@delegate?.DynamicInvoke(eventData);
                }
            }
        }
    }

    public static void Subscribe<T>(GameObject ownerRef, Action<T> action)
    {
        Subscribe(ownerRef, action, 0);
    }

    public static void Subscribe<T>(GameObject ownerRef, Action<T> action, int order)
    {
        Type type = typeof(T);

        if (events.ContainsKey(type))
        {
            events[type][ownerRef].Add(new Callback(action, order));
        }
        else
        {
            events.Add(type, new Dictionary<GameObject, List<Callback>>());
            events[type].Add(ownerRef, new List<Callback>());
            events[type][ownerRef].Add(new Callback(action, order));

            List<Callback> callbacks = events[typeof(T)][ownerRef];
            callbacks.Sort((callback1, callback2) => callback2.order.CompareTo(callback1.order));
        }
    }

    public static void Unsubscribe<T>(GameObject ownerRef, Action<T> action)
    {
        Type type = typeof(T);

        if (events.ContainsKey(type) && events[type].ContainsKey(ownerRef))
        {
            RemoveCallback(action, events[type][ownerRef]);

            if (events[type][ownerRef].Count <= 0)
            {
                events[type].Remove(ownerRef);
            }

            if (events[type].Count <= 0)
            {
                events.Remove(type);
            }
        }
    }

    private static void RemoveCallback<T>(Action<T> action, List<Callback> callbacks)
    {
        for (int i = 0; i < callbacks.Count; i++)
        {
            if (callbacks[i].@delegate == (MulticastDelegate)action)
            {
                callbacks.RemoveAt(i);
            }
        }
    }

    public struct Callback
    {
        public Delegate @delegate;
        public int order;

        public Callback(Delegate @delegate, int order)
        {
            this.@delegate = @delegate;
            this.order = order;
        }
    }
}