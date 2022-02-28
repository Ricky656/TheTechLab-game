using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LevelObject, IInteractable
{
    public Item requiredKey;

    public void Interact(GameObject interactingObject)
    {
        if (!requiredKey) 
        {
            OpenDoor(); 
            return; 
        }
        List<Item> inventoryCheck = interactingObject.GetComponent<PlayerController>().GetInventory();
        if (inventoryCheck.Contains(requiredKey))
        {

            DisplayMessage($"You unlock the door using the {requiredKey.itemName}");
            OpenDoor();
        }
        else
        {
            DisplayMessage($"You need to find the {requiredKey.itemName} to open this door!");
        }
    }

    public void OpenDoor()
    {
        //TODO: Animations for door opening, sound effects etc...
        gameObject.SetActive(false);
        EventController.TriggerEvent(EventController.EventType.DoorOpen);
    }

    private void DisplayMessage(string text)
    {
        DialogueConversation convo = ScriptableObject.CreateInstance("DialogueConversation") as DialogueConversation;
        DialogueLine message = new DialogueLine();
        message.text = text;
        convo.gameHalt = false;
        convo.dialogueLines = new DialogueLine[] { message };
        DialogueController.StartConversation(convo);
    }
}
