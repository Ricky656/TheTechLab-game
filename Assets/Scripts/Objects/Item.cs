using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //Using identifier and itemname because I want to be able to use textmesh formatting in the names - but still need a basic string to identify the item
    public string identifier; 
    public string itemName;
    [TextArea] public string pickupMessage = "You discovered the ";   

}
