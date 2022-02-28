using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelData 
{
    public string levelName;
    public List<string> eventListeners;
    public List<string> eventDataListeners;

    public LevelData(string name, List<string> listeners, List<string> dataListeners)
    {
        levelName = name;
        eventListeners = listeners;
        eventDataListeners = dataListeners;
    }
}
