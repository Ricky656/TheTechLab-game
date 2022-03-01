using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level01 : Level
{

    public DialogueConversation introDialogue;
    public DialogueConversation tutorialDialogue; 

    private int dialogueWaitCounter;
    public NPC professor;
    public GameObject player;

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
        AddListener(EventController.EventType.DialogueLineFinish, "DialogueReactions");
        AddListener(EventController.EventType.DialogueEnd, "EndOfOpeningDialogue");
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
                RemoveListener(EventController.EventType.DialogueLineFinish, "DialogueReactions");

                AddListener(EventController.EventType.QuestCompleted, "PickupGun");
                CameraController.SetCameraMode(CameraController.CameraMode.Normal);
                professor.AddDoneConversation("done01", this.GetType().ToString());
                professor.AddAttackedConversation("Attacked01", this.GetType().ToString());
                professor.AddAttackedConversation("Attacked02", this.GetType().ToString());
                GameController.SaveGame();
                break;
        }
    }

    private void EndOfOpeningDialogue()
    {
        RemoveListener(EventController.EventType.DialogueEnd, "EndOfOpeningDialogue");
        DialogueController.StartConversation(tutorialDialogue);
    }

    private void PickupGun()
    {
        Debug.Log("Pickup gun");
        professor.ClearDoneConversations();
        professor.AddActiveConversation("01",this.GetType().ToString());
        player.GetComponent<EntanglementGun>().Enable();
        player.GetComponent<EntanglementGun>().LockControl(false);  
        RemoveListener(EventController.EventType.QuestCompleted, "PickupGun");
        AddDataListener(EventController.EventType.BulletHit, "EntangleBox");
    }

    private void EntangleBox(object data)
    {
        GameObject[] objs = (GameObject[])data;
        if(objs[1].name == "level01_box")
        {
            professor.ClearActiveConversations();
            professor.AddDoneConversation("done02", this.GetType().ToString());
            professor.AddDoneConversation("done03", this.GetType().ToString());
            RemoveDataListener(EventController.EventType.BulletHit, "EntangleBox");
        }
    }
}
