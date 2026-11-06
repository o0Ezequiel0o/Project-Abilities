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

        private struct PendingOperation
        {
            public Entry Entry;
            public bool IsSubscribe;

            public PendingOperation(Entry entry, bool isSubscribe)
            {
                Entry = entry;
                IsSubscribe = isSubscribe;
            }
        }

        private readonly List<Entry> callbacks = new List<Entry>();
        private readonly List<PendingOperation> pendingOperations = new List<PendingOperation>();

        private int invocationDepth;

        public int Count => callbacks.Count;
        public bool IsInvoking => invocationDepth > 0;

        public void Subscribe(Action<T> callback)
        {
            Subscribe(callback, 0);
        }

        public void Subscribe(Action<T> callback, int priority)
        {
            if (callback == null) return;

            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].Callback == callback) return;
            }

            Entry entry = new Entry(callback, priority);

            if (IsInvoking)
            {
                pendingOperations.Add(new PendingOperation(entry, true));
            }
            else
            {
                AddEntry(entry);
            }
        }

        public void Unsubscribe(Action<T> callback)
        {
            if (callback == null) return;

            if (IsInvoking)
            {
                pendingOperations.Add(new PendingOperation(new Entry(callback, 0), false));
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

            if (invocationDepth == 0 && pendingOperations.Count > 0)
            {
                ApplyPendingOperations();
            }
        }

        private void ApplyPendingOperations()
        {
            for (int i = 0; i < pendingOperations.Count; i++)
            {
                PendingOperation operation = pendingOperations[i];

                if (operation.IsSubscribe)
                {
                    AddEntry(operation.Entry);
                }
                else
                {
                    RemoveEntry(operation.Entry.Callback);
                }
            }
            pendingOperations.Clear();
        }

        public void Clear()
        {
            if (IsInvoking)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    pendingOperations.Add(new PendingOperation(new Entry(callbacks[i].Callback, 0), false));
                }
            }
            else
            {
                callbacks.Clear();
                pendingOperations.Clear();
            }
        }
    }

    public sealed class OrderedAction<T1, T2> : IOrderedActionSubscription<T1, T2>
    {
        private struct Entry
        {
            public Action<T1, T2> Callback;
            public int Priority;

            public Entry(Action<T1, T2> callback, int priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }

        private struct PendingOperation
        {
            public Entry Entry;
            public bool IsSubscribe;

            public PendingOperation(Entry entry, bool isSubscribe)
            {
                Entry = entry;
                IsSubscribe = isSubscribe;
            }
        }

        private readonly List<Entry> callbacks = new List<Entry>();
        private readonly List<PendingOperation> pendingOperations = new List<PendingOperation>();

        private int invocationDepth;

        public int Count => callbacks.Count;
        public bool IsInvoking => invocationDepth > 0;

        public void Subscribe(Action<T1, T2> callback)
        {
            Subscribe(callback, 0);
        }

        public void Subscribe(Action<T1, T2> callback, int priority)
        {
            if (callback == null) return;

            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].Callback == callback) return;
            }

            Entry entry = new Entry(callback, priority);

            if (IsInvoking)
            {
                pendingOperations.Add(new PendingOperation(entry, true));
            }
            else
            {
                AddEntry(entry);
            }
        }

        public void Unsubscribe(Action<T1, T2> callback)
        {
            if (callback == null) return;

            if (IsInvoking)
            {
                pendingOperations.Add(new PendingOperation(new Entry(callback, 0), false));
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

            if (invocationDepth == 0 && pendingOperations.Count > 0)
            {
                ApplyPendingOperations();
            }
        }

        private void ApplyPendingOperations()
        {
            for (int i = 0; i < pendingOperations.Count; i++)
            {
                PendingOperation operation = pendingOperations[i];

                if (operation.IsSubscribe)
                {
                    AddEntry(operation.Entry);
                }
                else
                {
                    RemoveEntry(operation.Entry.Callback);
                }
            }
            pendingOperations.Clear();
        }

        public void Clear()
        {
            if (IsInvoking)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    pendingOperations.Add(new PendingOperation(new Entry(callbacks[i].Callback, 0), false));
                }
            }
            else
            {
                callbacks.Clear();
                pendingOperations.Clear();
            }
        }
    }
}