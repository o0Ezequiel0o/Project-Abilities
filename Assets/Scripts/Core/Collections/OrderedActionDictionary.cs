using System.Collections.Generic;
using System;

namespace Zeke.Collections
{
    public class OrderedActionDictionary<TSubKey, TCallBack>
    {
        private readonly Dictionary<TSubKey, OrderedAction<TCallBack>> subscribers;

        public OrderedActionDictionary()
        {
            subscribers = new Dictionary<TSubKey, OrderedAction<TCallBack>>();
        }

        public void Invoke(TSubKey key, TCallBack output)
        {
            if (key == null) return;

            if (subscribers.TryGetValue(key, out OrderedAction<TCallBack> callBack))
            {
                callBack?.Invoke(output);
            }
        }

        public void Subscribe(TSubKey key, Action<TCallBack> callBack)
        {
            Subscribe(key, callBack, int.MaxValue);
        }

        public void Subscribe(TSubKey key, Action<TCallBack> callBack, int order)
        {
            if (subscribers.ContainsKey(key))
            {
                subscribers[key].Subscribe(callBack, order);
            }
            else
            {
                OrderedAction<TCallBack> orderedAction = new OrderedAction<TCallBack>();

                subscribers.Add(key, orderedAction);
                orderedAction.Subscribe(callBack, order);
            }
        }

        public void Unsubscribe(TSubKey source, Action<TCallBack> callBack)
        {
            if (subscribers.ContainsKey(source))
            {
                subscribers[source].Unsubscribe(callBack);

                if (subscribers[source].Callbacks <= 0)
                {
                    subscribers.Remove(source);
                }
            }
        }
    }
}