using System.Collections.Generic;
using System;

namespace Zeke.GlobalEventsBus
{
    internal static class EventChannel<T> where T : IGlobalEvent
    {
        private struct Entry
        {
            public Action<T> Callback;
            public int Priority;

            public Entry(Action<T> callback, int priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }

        private static readonly List<Entry> callbacks = new(8);
        private static readonly HashSet<Action<T>> callbacksHash = new(8);

        public static void Subscribe(Action<T> callback, int priority)
        {
            if (callback == null) return;
            if (callbacksHash.Contains(callback)) return;

            int index = callbacks.Count;

            while (index > 0 && callbacks[index - 1].Priority < priority)
            {
                index--;
            }

            callbacksHash.Add(callback);
            callbacks.Insert(index, new Entry(callback, priority));
        }

        public static void Unsubscribe(Action<T> callback)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].Callback == callback)
                {
                    callbacksHash.Remove(callback);
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        public static void Invoke(T eventData)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Callback.Invoke(eventData);
            }
        }
    }
}