using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject continueButton;
    public GameObject pauseMenu;


    private static UIController controller;

    public enum menus
    {
        None,
        Main,
        Pause
    }

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

    public static void SetMenu(menus menu)
    {
        switch (menu)
        {
            case menus.None:
                controller.SetMainMenu(false);
                controller.SetPauseMenu(false);
                break;
            case menus.Main:
                controller.SetMainMenu(true);
                controller.SetPauseMenu(false);
                break;
            case menus.Pause:
                controller.SetMainMenu(false);
                controller.SetPauseMenu(true);
                break;

        }
    }

    private void SetMainMenu(bool visible)//Animations and such could go here
    {
        controller.mainMenu.SetActive(visible);
    }

    private void SetPauseMenu(bool visible)
    {
        controller.pauseMenu.SetActive(visible);
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
