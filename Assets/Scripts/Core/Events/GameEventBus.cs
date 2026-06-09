using System;
using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Core.Events
{
    public sealed class GameEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            if (handler == null)
            {
                Debug.LogError("[EventBus] Subscribe failed: handler is null.");
                return;
            }

            var eventType = typeof(TEvent);

            if (_handlers.TryGetValue(eventType, out var existing))
                _handlers[eventType] = Delegate.Combine(existing, handler);
            else
                _handlers[eventType] = handler;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            if (handler == null)
                return;

            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var existing))
                return;

            var updated = Delegate.Remove(existing, handler);

            if (updated == null)
                _handlers.Remove(eventType);
            else
                _handlers[eventType] = updated;
        }

        public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            if (gameEvent == null)
            {
                Debug.LogError("[EventBus] Publish failed: event is null.");
                return;
            }

            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var existing))
                return;

            if (existing is Action<TEvent> action)
            {
                try
                {
                    action.Invoke(gameEvent);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}