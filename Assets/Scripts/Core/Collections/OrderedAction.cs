using System.Collections.Generic;
using System;

namespace Zeke.Collections
{
    public interface IOrderedActionSubscription<T>
    {
        public void Subscribe(Action<T> callback);

        public void Subscribe(Action<T> callback, int priority);

        public void Unsubscribe(Action<T> callback);
    }

    public interface IOrderedActionSubscription<T1, T2>
    {
        public void Subscribe(Action<T1, T2> callback);

        public void Subscribe(Action<T1, T2> callback, int priority);

        public void Unsubscribe(Action<T1, T2> callback);
    }

    public sealed class OrderedAction<T> : IOrderedActionSubscription<T>
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

        private readonly List<Entry> callbacks = new List<Entry>();
        private readonly HashSet<Action<T>> callbacksHash = new HashSet<Action<T>>();
        private readonly Queue<PendingOperation> pendingOperations = new Queue<PendingOperation>();

        private int invocationDepth;

        public int Count => callbacksHash.Count;

        public bool IsInvoking => invocationDepth > 0;

        public void Subscribe(Action<T> callback)
        {
            Subscribe(callback, 0);
        }

        public void Subscribe(Action<T> callback, int priority)
        {
            if (callback == null || !callbacksHash.Add(callback)) return;

            Entry entry = new Entry(callback, priority);

            if (IsInvoking)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, true));
            }
            else
            {
                AddEntry(entry);
            }
        }

        public void Unsubscribe(Action<T> callback)
        {
            if (callback == null || !callbacksHash.Remove(callback)) return;

            Entry entry = new Entry(callback, 0);

            if (IsInvoking)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, false));
            }
            else
            {
                RemoveEntry(callback);
            }
        }

        private void AddEntry(Entry entry)
        {
            int index = callbacks.Count;

            while (index > 0 && callbacks[index - 1].Priority < entry.Priority)
            {
                index -= 1;
            }

            callbacks.Insert(index, entry);
        }

        private void RemoveEntry(Action<T> callback)
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

        public void Invoke(T data)
        {
            invocationDepth += 1;

            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Callback.Invoke(data);
            }

            invocationDepth -= 1;

            if (invocationDepth == 0)
            {
                ApplyPendingOperations();
            }
        }

        private void ApplyPendingOperations()
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

        public void Clear()
        {
            if (IsInvoking)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    Action<T> callback = callbacks[i].Callback;

                    if (callbacksHash.Remove(callback))
                    {
                        pendingOperations.Enqueue(new PendingOperation(new Entry(callback, 0), false));
                    }
                }
            }
            else
            {
                callbacks.Clear();
                callbacksHash.Clear();
                pendingOperations.Clear();
            }
        }
    }

    public sealed class OrderedAction<T1, T2> : IOrderedActionSubscription<T1, T2>
    {
        private readonly struct Entry
        {
            public readonly Action<T1, T2> Callback;
            public readonly int Priority;

            public Entry(Action<T1, T2> callback, int priority)
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

        private readonly List<Entry> callbacks = new(8);
        private readonly HashSet<Action<T1, T2>> callbacksHash = new(8);
        private readonly Queue<PendingOperation> pendingOperations = new(4);

        private int invocationDepth;

        public int Count => callbacksHash.Count;
        public bool IsInvoking => invocationDepth > 0;

        public void Subscribe(Action<T1, T2> callback)
        {
            Subscribe(callback, 0);
        }

        public void Subscribe(Action<T1, T2> callback, int priority)
        {
            if (callback == null || !callbacksHash.Add(callback)) return;

            Entry entry = new(callback, priority);

            if (IsInvoking)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, true));
            }
            else
            {
                AddEntry(entry);
            }
        }

        public void Unsubscribe(Action<T1, T2> callback)
        {
            if (callback == null || !callbacksHash.Remove(callback)) return;

            Entry entry = new(callback, 0);

            if (IsInvoking)
            {
                pendingOperations.Enqueue(new PendingOperation(entry, false));
            }
            else
            {
                RemoveEntry(callback);
            }
        }

        private void AddEntry(Entry entry)
        {
            int index = callbacks.Count;

            while (index > 0 && callbacks[index - 1].Priority < entry.Priority)
            {
                index -= 1;
            }

            callbacks.Insert(index, entry);
        }

        private void RemoveEntry(Action<T1, T2> callback)
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

        public void Invoke(T1 data1, T2 data2)
        {
            invocationDepth += 1;

            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Callback.Invoke(data1, data2);
            }

            invocationDepth -= 1;

            if (invocationDepth == 0)
            {
                ApplyPendingOperations();
            }
        }

        private void ApplyPendingOperations()
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

        public void Clear()
        {
            if (IsInvoking)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    Action<T1, T2> callback = callbacks[i].Callback;

                    if (callbacksHash.Remove(callback))
                    {
                        pendingOperations.Enqueue(new PendingOperation(new Entry(callback, 0), false));
                    }
                }
            }
            else
            {
                callbacks.Clear();
                callbacksHash.Clear();
                pendingOperations.Clear();
            }
        }
    }
}