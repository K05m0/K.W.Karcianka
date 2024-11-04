using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;

    public CustomGameEvent response;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }
    public void OnEventRaised(Component sender = null, object data = null)
    {
        if (response == null)
        {
            Debug.LogError($"Response in event listener {gameObject.name} is Null");
            return;
        }
        response.Invoke(sender, data);
    }

}
