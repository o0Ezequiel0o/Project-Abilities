using System;

namespace Zeke.GlobalEventsBus
{
    public interface IGlobalEvent { }

    public static class GlobalEventBus
    {
        public static void Subscribe<T>(Action<T> callback) where T : IGlobalEvent
        {
            EventChannel<T>.Subscribe(callback, 0);
        }

        public static void Subscribe<T>(Action<T> callback, int priority) where T : IGlobalEvent
        {
            EventChannel<T>.Subscribe(callback, priority);
        }

        public static void Unsubscribe<T>(Action<T> callback) where T : IGlobalEvent
        {
            EventChannel<T>.Unsubscribe(callback);
        }

        public static void Invoke<T>(T eventData) where T : IGlobalEvent
        {
            EventChannel<T>.Invoke(eventData);
        }
    }
}