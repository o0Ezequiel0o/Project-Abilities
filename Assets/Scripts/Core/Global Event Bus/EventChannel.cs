using System.Collections.Generic;
using System;

namespace Zeke.GlobalEventsBus
{
    internal static class EventChannel<T> where T : IGlobalEvent
    {
        private readonly struct Entry
        {
            public readonly Action<T> Callback;
            public readonly int Priority;

            public Entry(Action<T> callback, int priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }

        private readonly struct PendingOperation
        {
            public readonly Entry Entry;
            public readonly bool Subscribe;

            public PendingOperation(Entry entry, bool subscribe)
            {
                Entry = entry;
                Subscribe = subscribe;
            }
        }

        private static readonly List<Entry> callbacks = new(8);
        private static readonly HashSet<Action<T>> callbacksHash = new(8);
        private static readonly Queue<PendingOperation> pendingOperations = new(8);

        private static int invocationDepth;

        public static bool InvokingEvents => invocationDepth > 0;

        public static void Subscribe(Action<T> callback, int priority)
        {
            if (callback == null || !callbacksHash.Add(callback)) return;

            Entry entry = new Entry(callback, priority);

            if (InvokingEvents)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, true));
            }
            else
            {
                AddEntry(entry);
            }
        }

        public static void Unsubscribe(Action<T> callback)
        {
            if (callback == null || !callbacksHash.Remove(callback)) return;

            Entry entry = new Entry(callback, 0);

            if (InvokingEvents)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, false));
            }
            else
            {
                RemoveEntry(callback);
            }
        }

        private static void AddEntry(Entry entry)
        {
            int index = callbacks.Count;

            while (index > 0 && callbacks[index - 1].Priority < entry.Priority)
            {
                index -= 1;
            }

            callbacks.Insert(index, entry);
        }

        private static void RemoveEntry(Action<T> callback)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].Callback == callback)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        public static void Invoke(T eventData)
        {
            invocationDepth += 1;

            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Callback.Invoke(eventData);
            }

            invocationDepth -= 1;

            if (invocationDepth == 0)
            {
                ApplyPendingOperations();
            }
        }

        private static void ApplyPendingOperations()
        {
            while (pendingOperations.Count > 0)
            {
                PendingOperation operation = pendingOperations.Dequeue();

                if (operation.Subscribe)
                {
                    AddEntry(operation.Entry);
                }
                else
                {
                    RemoveEntry(operation.Entry.Callback);
                }
            }
        }
    }
}
