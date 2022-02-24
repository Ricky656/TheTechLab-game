using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

[Serializable]
public class DialogueLine
{
    public GameObject character;//Reference to speaking character, used to create identifying UI in-game 
    public float textSpeed = 0.05f; //Time between each letter being typed out, lower = faster dialogue 

    [TextArea]public string text; 
}
