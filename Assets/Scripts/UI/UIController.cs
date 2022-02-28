using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public GameObject mainmenu;
    public GameObject continueButton;

    private static UIController controller;

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

    public static void SetMainMenu(bool visible)
    {
        controller.mainmenu.SetActive(visible);
    }

    public static void InitializeMainMenu(GameSaveData data)
    {
        if(data != null)
        {
            controller.continueButton.SetActive(true);
        }
        else
        {
            controller.continueButton.SetActive(false);
        }
    }

  
}
