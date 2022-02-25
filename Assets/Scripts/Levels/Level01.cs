using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level01 : Level
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

        UIController.CameraMove(cameraLocations[0]);

        DialogueController.StartConversation(introDialogue);
        onDialogueLineEnd = new UnityAction(FadeIn);
        EventController.StartListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
        dialogueWaitCounter = 0;
    }

    private void FadeIn()
    {
        dialogueWaitCounter++;
        switch (dialogueWaitCounter)
        {
            case 2://Doesn't execute until two dialogue lines have finished/
                StartCoroutine(UIController.CameraFade(false));
                EventController.StopListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
                break;
            case 3:
                EventController.TriggerEvent(EventController.EventType.PlayerUnlocked);//Unlock player control
                break;
        }
    }
}
