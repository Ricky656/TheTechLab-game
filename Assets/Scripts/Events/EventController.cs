using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    /* EventController stores events in a dictionary and allows any other script to use it to listen for or trigger events,
    this allows for much cleaner scripting of sequential events that wait for particular triggers such as the player completing a task in game */

    private Dictionary<EventType, UnityEvent> events; //stores the events triggered and listened for
    private static EventController controller;

    public enum EventType//Event types that can be triggered
    {
        DialogueLineFinish,
        DialogueEnd,
        CameraFadeComplete,
        PlayerLocked,
        PlayerUnlocked,
        QuestCompleted

    }

    public static EventController instance
    {
        get
        {
            if (!controller)
            {
                controller = FindObjectOfType(typeof(EventController)) as EventController;
                controller.Initialize();
            }
            return controller;
        }
    }

    private void Initialize()
    {
        events = new Dictionary<EventType, UnityEvent>();
    }

    public static void StartListening(EventType eventName, UnityAction listener)//Adds listener to an event
    {
        //if (controller == null) { Debug.Log("No event controller!"); return; }

        UnityEvent currentEvent; 
        if(!instance.events.TryGetValue(eventName, out currentEvent)) //Check if event already exists in dictionary
        {
            currentEvent = new UnityEvent();
            instance.events.Add(eventName, currentEvent);//TODO: refactor
        }
        currentEvent.AddListener(listener);

        //Debug.Log($"Started Listening for { eventName } ");
    }

    public static void StopListening(EventType eventName, UnityAction listener)//Removes listener from an event, if it exists in dictionary
    {
        if (controller == null) { Debug.Log($"<color=yellow>No event controller!</color>"); return; }

        UnityEvent currentEvent; 
        if(controller.events.TryGetValue(eventName, out currentEvent))
        {
            currentEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(EventType eventName) //Triggers specified event, if it exists in dictionary
    {
        //Debug.Log($"Trying to trigger{ eventName } ");
        UnityEvent currentEvent;
        if(instance.events.TryGetValue(eventName, out currentEvent))
        {
            Debug.Log($"Successfully triggered { eventName } ");
            currentEvent.Invoke();
        }
    }
}
