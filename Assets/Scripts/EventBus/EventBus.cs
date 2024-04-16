using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : IGameService
{
    private readonly Dictionary<Type, List<object>> _eventDictionary;
    private readonly Dictionary<Type, List<Action>> _noArgsEventDictionary;

    public EventBus()
    {
        _eventDictionary = new Dictionary<Type, List<object>>();
        _noArgsEventDictionary = new Dictionary<Type, List<Action>>();
    }

    public void Initialize()
    {
    }

    public void Subscribe<T>(Action<T> action) where T : IEvent
    {
        var type = typeof(T);
        if (!_eventDictionary.ContainsKey(type)) _eventDictionary[type] = new List<object>();
        _eventDictionary[type].Add(action);
        // Debug.Log("Subscribed to " + type);
    }

    public void Subscribe<T>(Action action) where T : IEvent
    {
        var type = typeof(T);
        if (!_noArgsEventDictionary.ContainsKey(type)) _noArgsEventDictionary[type] = new List<Action>();
        _noArgsEventDictionary[type].Add(action);
        // Debug.Log("Subscribed to " + type);
    }

    public void Unsubscribe<T>(Action<T> action) where T : IEvent
    {
        // Debug.Log("Unsubscribed from " + typeof(T));
        var type = typeof(T);
        if (_eventDictionary.ContainsKey(type))
        {
            var list = _eventDictionary[type];
            if (list.Contains(action)) list.Remove(action);

            if (list.Count == 0) _eventDictionary.Remove(type);
        }
        else
        {
            Debug.LogWarning("EventBus: Unsubscribe failed, no event of type " + type);
        }
    }

    public void Unsubscribe<T>(Action action) where T : IEvent
    {
        // Debug.Log("Unsubscribed from " + typeof(T));
        var type = typeof(T);
        if (_noArgsEventDictionary.ContainsKey(type))
        {
            var list = _noArgsEventDictionary[type];
            if (list.Contains(action)) list.Remove(action);

            if (list.Count == 0) _noArgsEventDictionary.Remove(type);
        }
        else
        {
            Debug.LogWarning("EventBus: Unsubscribe failed, no event of type " + type);
        }
    }

    public void Fire<T>(T payload) where T : IEvent
    {
        var type = typeof(T);
        if (_eventDictionary.ContainsKey(type))
        {
            object[] registereds = new object[_eventDictionary[type].Count];
            for (int i = 0; i < registereds.Length; i++)
            {
                registereds[i] = _eventDictionary[type][i];
            }

            for (int index = 0; index < registereds.Length; index++)
            {
                Action<T> action = (Action<T>)registereds[index];

                try
                {
                    action?.Invoke(payload);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        if (_noArgsEventDictionary.ContainsKey(type))
        {
            for (int index = 0; index < _noArgsEventDictionary[type].Count; index++)
            {
                Action action = _noArgsEventDictionary[type][index];

                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                }
            }
        }
    }
}