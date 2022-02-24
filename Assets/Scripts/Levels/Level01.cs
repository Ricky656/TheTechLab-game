using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level01 : MonoBehaviour
{

    public DialogueConversation introDialogue;

    private UnityAction onDialogueLineEnd;
    private int dialogueWaitCounter;

    public void StartLevel()
    {
        StartCoroutine(UIController.CameraFade());
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence()
    {
        yield return new WaitForSeconds(3);
        DialogueController.StartConversation(introDialogue);
        onDialogueLineEnd = new UnityAction(FadeIn);
        EventController.StartListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
        dialogueWaitCounter = 0;
    }

    private void FadeIn()
    {
        dialogueWaitCounter++;
        if (dialogueWaitCounter == 2)
        {
            StartCoroutine(UIController.CameraFade(false));
            EventController.StopListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
        }
    }
}
