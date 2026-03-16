using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages WordQuake's custom events
/// </summary>
public static class WQEventSystem
{
    private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    public static void Subscribe(string eventName, Action<object> listener = null)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = delegate { }; // Initialize event slot
        }

        eventDictionary[eventName] += listener;
    }

    public static void Unsubscribe(string eventName, Action<object> listener = null)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    /// <summary>
    /// Triggers an event notifying all subscribed listeners
    /// </summary>
    public static void TriggerEvent(string wqEventName, object eventData = null)
    {
        MainThreadDispatcher.Instance.Run(() => {
					
            if (eventDictionary.ContainsKey(wqEventName))
            {
                eventDictionary[wqEventName]?.Invoke(eventData);
            }
        });
        
    }
}
