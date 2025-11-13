using System;
using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;


namespace Core.Events
{
    public class EventDispatcher : Singleton<EventDispatcher>
    {
        private Dictionary<EventType, Action<object>> _events = new();

        private void OnDestroy()
        {
            ClearListeners();
        }

        public void AddListener(EventType eventType, Action<object> callback)
        {
            if (_events.ContainsKey(eventType))
            {
                _events[eventType] += callback;
            }
            else
            {
                _events.Add(eventType, callback);
            }
        }

        public void RemoveListener(EventType eventType, Action<object> callback)
        {
            if (_events.TryGetValue(eventType, out var existing))
            {
                existing -= callback;
                if (existing == null)
                    _events.Remove(eventType);
                else
                    _events[eventType] = existing;
            }
        }

        public void FireEvent(EventType eventType, object param = null)
        {
            if (_events.ContainsKey(eventType))
            {
                var actions = _events[eventType];
                if (actions == null)
                {
                    Debug.Log($"Event {eventType} has not been registered");
                    _events.Remove(eventType);
                    return;
                }

                actions.Invoke(param);
            }
        }

        private void ClearListeners()
        {
            _events.Clear();
        }

        public bool HasListener(EventType eventType)
        {
            return _events.ContainsKey(eventType);
        }

        public void ClearEvent(EventType eventType)
        {
            if (_events.ContainsKey(eventType))
                _events.Remove(eventType);
        }
    }

// Extension Method
    public static class EventDispatcherExtensions
    {
        public static void AddListener(this MonoBehaviour instance, EventType eventType, Action<object> callback)
        {
            EventDispatcher.Instance.AddListener(eventType, callback);
        }

        public static void RemoveListener(this MonoBehaviour instance, EventType eventType, Action<object> callback)
        {
            if (EventDispatcher.Instance == null) return;

            EventDispatcher.Instance.RemoveListener(eventType, callback);
        }

        public static void FireEvent<T>(this MonoBehaviour instance, EventType eventType, T param)
        {
            EventDispatcher.Instance?.FireEvent(eventType, param);
        }
    }
}