using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Level : MonoBehaviour, ISaveable<LevelData>
{
    public GameObject[] cameraLocations;
    public GameObject[] characters; //Characters in this level 

    private List<string> currentListeners; //stored as string for save/load purposes
    private List<string> currentDataListeners; //^for events with data

    protected void Awake()
    {
        currentListeners = new List<string>();
        currentDataListeners = new List<string>();
    }

    protected GameObject GetCameraFlag(string identifier)//Allows cameraflags to be referenced by names, rather than remembering exactly which element of array its stored in
    {
        
        string searchString = $"cameraFlag_{this.GetType()}_{identifier}";   
        GameObject flag = GenericTools.GetFromArray(cameraLocations, searchString   );
        if (!flag) { Debug.Log($"Could not find cameraFlag named: {searchString}, ensure flag names are formatted as 'cameraFlag_(LevelName)_(identifier)'"); }
        return flag; 
    }

    protected GameObject GetCharacter(string identifier)
    {
        GameObject chara = GenericTools.GetFromArray(characters, identifier);
        if (!chara) { Debug.Log($"<color=yellow>{this.GetType()} cannot find character: {identifier}</Color>"); }
        return chara;
    }



    #region Add/Remove listeners for level event scripting
    protected void AddListener(EventController.EventType eventType, string functionToCall)
    {
        UnityAction action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, functionToCall);
        string listenerString = $"{eventType.ToString()},{functionToCall}";
        Debug.Log($"Adding listener: {listenerString} ");
        currentListeners.Add(listenerString);
        EventController.StartListening(eventType, action);
        
    }

    protected void AddDataListener(EventController.EventType eventType, string functionToCall)// UnityAction<object> listener)
    {
        UnityAction<object> action = (UnityAction<object>)Delegate.CreateDelegate(typeof(UnityAction<object>), this, functionToCall);
        string listenerString = $"{eventType.ToString()},{functionToCall}";
        currentDataListeners.Add(listenerString);
        EventController.StartListening(eventType, action);
    }

    protected void RemoveListener(EventController.EventType eventType, string calledFunction)
    {
        foreach(string listenerString in currentListeners)
        {
            string[] current = listenerString.Split(',');
            if(current[0] == eventType.ToString()&& current[1] == calledFunction)
            {
                currentListeners.Remove(listenerString);
                UnityAction action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, calledFunction);
                EventController.StopListening(eventType, action);
                break;
            }
        }
    }

    protected void RemoveDataListener(EventController.EventType eventType, string calledFunction)
    {
        foreach (string listenerString in currentDataListeners)
        {
            string[] current = listenerString.Split(',');
            if (current[0] == eventType.ToString() && current[1] == calledFunction)
            {
                currentDataListeners.Remove(listenerString);
                UnityAction<object> action = (UnityAction<object>)Delegate.CreateDelegate(typeof(UnityAction<object>), this, calledFunction);
                EventController.StopListening(eventType, action);
                break;
            }
        }
    }
    #endregion

    public LevelData Save()
    {
        return new LevelData(gameObject.name, currentListeners, currentDataListeners);
    }

    public void Load(LevelData data)
    {
        currentDataListeners.Clear();
        currentListeners.Clear();
        foreach(string listenerString in data.eventListeners)
        {
            Debug.Log($"Loading listener: {listenerString}");
            string[] current = listenerString.Split(',');
            EventController.EventType eventType = (EventController.EventType)System.Enum.Parse(typeof(EventController.EventType), current[0]);//Gets the eventType enum from a string
            //UnityAction action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, current[1]); //Creates a UnityAction to call a function named by a string
            AddListener(eventType, current[1]);
        }
    }
}
