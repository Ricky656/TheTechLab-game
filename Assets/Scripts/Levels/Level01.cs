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
        StartCoroutine(CameraController.CameraFade());
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence()
    {
        yield return new WaitForSeconds(3);

        CameraController.CameraMove(GetCameraFlag("start"));

        DialogueController.StartConversation(introDialogue);
        onDialogueLineEnd = new UnityAction(DialogueReactions);
        EventController.StartListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
        dialogueWaitCounter = 0;
    }

    private void DialogueReactions()
    {
        dialogueWaitCounter++;
        switch (dialogueWaitCounter)
        {
            case 2://Doesn't execute until two dialogue lines have finished/
                StartCoroutine(CameraController.CameraFade(false));
                break;
            case 4:
                CameraController.CameraMove(GetCameraFlag("01"), true);
                break;
            case 8:
                CameraController.CameraMove(GetCameraFlag("02"), true);
                break;
            case 9:
                EventController.TriggerEvent(EventController.EventType.PlayerUnlocked);//Unlock player control
                EventController.StopListening(EventController.EventType.DialogueLineFinish, onDialogueLineEnd);
                break;
        }
    }
}
