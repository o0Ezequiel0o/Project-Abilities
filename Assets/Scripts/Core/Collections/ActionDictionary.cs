using System.Collections.Generic;
using System;

namespace Zeke.Collections
{
    public class ActionDictionary<TSubKey, TCallBack>
    {
        private readonly Dictionary<TSubKey, Action<TCallBack>> subscribers;

        public ActionDictionary()
        {
            subscribers = new Dictionary<TSubKey, Action<TCallBack>>();
        }

        public void Invoke(TSubKey key, TCallBack output)
        {
            if (key == null) return;

            if (subscribers.TryGetValue(key, out Action<TCallBack> callBack))
            {
                callBack?.Invoke(output);
            }
        }

        public void Subscribe(TSubKey key, Action<TCallBack> callBack)
        {
            if (subscribers.ContainsKey(key))
            {
                subscribers[key] += callBack;
            }
            else
            {
                subscribers.Add(key, callBack);
            }
        }

        public void Unsubscribe(TSubKey source, Action<TCallBack> callBack)
        {
            if (subscribers.ContainsKey(source))
            {
                subscribers[source] -= callBack;

                if (subscribers[source] == null)
                {
                    subscribers.Remove(source);
                }
            }
        }
    }
}