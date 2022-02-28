using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level03 : MonoBehaviour
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

        EventController.StartListening(EventController.EventType.SwitchFlipped, StartEndSequence);
    }

    private void StartEndSequence()
    {
        EventController.StopListening(EventController.EventType.SwitchFlipped, StartEndSequence);
        StartCoroutine(CameraController.CameraFade());
        EventController.StartListening(EventController.EventType.CameraFadeComplete, EndDialogue);
    }

    private void EndDialogue()
    {
        EventController.StopListening(EventController.EventType.CameraFadeComplete, EndDialogue);
        DialogueController.StartConversation(endDialogue);
    }

}
