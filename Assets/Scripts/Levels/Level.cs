using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject[] cameraLocations;
    public GameObject[] characters; //Characters in this level 

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
}
