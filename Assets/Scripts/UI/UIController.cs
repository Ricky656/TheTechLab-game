using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    private static UIController controller;

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

  
}
