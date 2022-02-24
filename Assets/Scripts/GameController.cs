using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public void StartGame()
    {
        Debug.Log("Start Game");
        GameObject.Find("StartMenu").gameObject.SetActive(false);
        GameObject.Find("Level01").gameObject.GetComponent<Level01>().StartLevel();
    }

    public void ExitGame()
    {

    }
}
