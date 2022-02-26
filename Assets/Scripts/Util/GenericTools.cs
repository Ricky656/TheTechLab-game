using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTools
{
    public static GameObject GetFromArray(GameObject[] list, string name)
    {
        foreach(GameObject item in list)
        {
            if(item.name == name)
            {  
                return item; 
            }
        }
        return null;
    }
}
