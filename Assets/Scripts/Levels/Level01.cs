using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level01 : Level
{

    public DialogueConversation introDialogue;

    private UnityAction onDialogueLineEnd;
    private UnityAction onItemPickup;
    private int dialogueWaitCounter;
    private NPC professor;
    private GameObject player;

    public void StartLevel()
    {
        professor = GetCharacter("professor").GetComponent<NPC>();
        player = GetCharacter("player");
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

                onItemPickup = new UnityAction(PickupGun);
                EventController.StartListening(EventController.EventType.QuestCompleted, onItemPickup);
                CameraController.SetCameraMode(CameraController.CameraMode.Normal);
                professor.AddDoneConversation("done01", this.GetType().ToString());
                break;
        }
    }

    private void PickupGun()
    {
        professor.ClearDoneConversations();
        professor.AddActiveConversation("01",this.GetType().ToString());
        player.GetComponent<EntanglementGun>().Enable();
        player.GetComponent<EntanglementGun>().LockControl(false);
        EventController.StopListening(EventController.EventType.QuestCompleted, onItemPickup);
    }
}
