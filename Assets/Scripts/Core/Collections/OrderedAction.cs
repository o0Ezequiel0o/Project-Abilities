using System.Collections.Generic;
using System;

namespace Zeke.Collections
{
    public class OrderedAction
    {
        public int Callbacks => callbacks.Count;

        private readonly List<Callback> callbacks = new List<Callback>();

        public void Invoke()
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].action?.Invoke();
            }
        }

        public void Subscribe(Action callback)
        {
            Subscribe(callback, int.MaxValue);
        }

        public void Subscribe(Action callback, int order)
        {
            callbacks.Add(new Callback(callback, order));
            callbacks.Sort((callback1, callback2) => callback2.order.CompareTo(callback1.order));
        }

        public void Unsubscribe(Action callback)
        {
            RemoveCallback(callback);
        }

        private void RemoveCallback(Action action)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].action == action)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        private readonly struct Callback
        {
            public readonly Action action;
            public readonly int order;

            public Callback(Action action, int order)
            {
                this.action = action;
                this.order = order;
            }
        }
    }

    public class OrderedAction<T1>
    {
        public int Callbacks => callbacks.Count;

        private readonly List<Callback> callbacks = new List<Callback>();

        public void Invoke(T1 param1)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].action?.Invoke(param1);
            }
        }

        public void Subscribe(Action<T1> callback)
        {
            Subscribe(callback, int.MaxValue);
        }

        public void Subscribe(Action<T1> callback, int order)
        {
            callbacks.Add(new Callback(callback, order));
            callbacks.Sort((callback1, callback2) => callback1.order.CompareTo(callback2.order));
        }

        public void Unsubscribe(Action<T1> callback)
        {
            RemoveCallback(callback);
        }

        private void RemoveCallback(Action<T1> action)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].action == action)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        private readonly struct Callback
        {
            public readonly Action<T1> action;
            public readonly int order;

            public Callback(Action<T1> action, int order)
            {
                this.action = action;
                this.order = order;
            }
        }
    }

    public class OrderedAction<T1, T2>
    {
        public int Callbacks => callbacks.Count;

        private readonly List<Callback> callbacks = new List<Callback>();

        public void Invoke(T1 param1, T2 param2)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].action?.Invoke(param1, param2);
            }
        }

        public void Subscribe(Action<T1, T2> callback)
        {
            Subscribe(callback, int.MaxValue);
        }

        public void Subscribe(Action<T1, T2> callback, int order)
        {
            callbacks.Add(new Callback(callback, order));
            callbacks.Sort((callback1, callback2) => callback1.order.CompareTo(callback2.order));
        }

        public void Unsubscribe(Action<T1, T2> callback)
        {
            RemoveCallback(callback);
        }

        private void RemoveCallback(Action<T1, T2> action)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].action == action)
                {
                    callbacks.RemoveAt(i);
                    return;
                }
            }
        }

        private readonly struct Callback
        {
            public readonly Action<T1, T2> action;
            public readonly int order;

            public Callback(Action<T1, T2> action, int order)
            {
                this.action = action;
                this.order = order;
            }
        }
    }
}