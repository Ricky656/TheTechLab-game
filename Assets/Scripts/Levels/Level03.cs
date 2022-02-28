using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level03 : Level
{

    public DialogueConversation endDialogue;
    public NPC janitor;
    public GameObject player;
    void Start()
    {
        janitor.AddActiveConversation("01", this.GetType().ToString());
        janitor.AddDoneConversation("done01", this.GetType().ToString());
        janitor.AddDoneConversation("done02", this.GetType().ToString());
        janitor.AddAttackedConversation("attacked01", this.GetType().ToString());
        janitor.AddAttackedConversation("attacked02", this.GetType().ToString());
        janitor.AddDeathConversation("death01", this.GetType().ToString());
        AddListener(EventController.EventType.SwitchFlipped, "StartEndSequence");
    }

    private void StartEndSequence()
    {
        RemoveListener(EventController.EventType.SwitchFlipped, "StartEndSequence");
        StartCoroutine(CameraController.CameraFade());
        AddListener(EventController.EventType.CameraFadeComplete, "EndDialogue");
    }

    private void EndDialogue()
    {
        RemoveListener(EventController.EventType.CameraFadeComplete, "EndDialogue");
        DialogueController.StartConversation(endDialogue);
        AddListener(EventController.EventType.DialogueEnd, "ExitGame");
    }

    private void ExitGame()
    {
        RemoveListener(EventController.EventType.DialogueEnd, "ExitGame");
        Application.Quit();
    }

}
