using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject[] cameraLocations;

    protected GameObject GetCameraFlag(string identifier)//Allows cameraflags to be referenced by names, rather than remembering exactly which element of array its stored in
    {
        string searchString = $"cameraFlag_{this.GetType()}_{identifier}";
        foreach(GameObject flag in cameraLocations)
        {
            if(flag.name == searchString)
            {
                return flag;
            }
        }
        Debug.Log($"Could not find cameraFlag named: {searchString}, ensure flag names are formatted as 'cameraFlag_(LevelName)_(identifier)'");
        return null; 
    }
}
