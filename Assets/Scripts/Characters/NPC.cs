using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Character, IInteractable
{
    public GameObject interactableMarker;//Every NPC has their own exclamation mark, shown if they have an active conversation.
    public DialogueConversation[] dialogues; //All conversations this character can use in the game

    private List<DialogueConversation> activeConversations; //Currently used active conversations
    private List<DialogueConversation> doneConversations;  //Currently used 'done' messages
    private List<DialogueConversation> attackedConversations; //Currently used messages for when an NPC is hit by the entanglement gun 
    private DialogueConversation currentConversation;
    private UnityAction onDialogueEnd;
    private Rigidbody2D rigid;
    private int doneConvoIndex;
    private int attackedConvoIndex;

    public void Awake()
    {
        doneConvoIndex = 0;
        attackedConvoIndex = 0;
        rigid = GetComponent<Rigidbody2D>();
        activeConversations = new List<DialogueConversation>();
        doneConversations = new List<DialogueConversation>();
        attackedConversations = new List<DialogueConversation>();
        if (!interactableMarker && dialogues.Length>0) { Debug.Log($"<color=yellow>{gameObject.name} missing dialogueMarker!</color>"); }
    }
    
    public void Interact(GameObject interactingObject)
    {
        if(interactingObject.tag == "Player")
        {
            Talk();
        }
    }
    private void Talk()
    {
        if (activeConversations.Count > 0)
        {
            currentConversation = activeConversations[0]; //plays the 1st conversation in list
            if (!currentConversation.repeatable)
            {
                activeConversations.Remove(currentConversation);
            }
        }else if (doneConversations.Count > 0)
        {
            currentConversation = doneConversations[doneConvoIndex];
            if (!currentConversation.repeatable)
            {
                doneConversations.Remove(currentConversation);
            }
            else
            {
                doneConvoIndex++;
                if (doneConvoIndex > doneConversations.Count-1) { doneConvoIndex = 0; }
            }
        }
        PlayConversation(currentConversation);
    }

    private void Attacked()
    {
        if(attackedConvoIndex < attackedConversations.Count)
        {
            currentConversation = attackedConversations[attackedConvoIndex];
            if (!currentConversation.repeatable)
            {
                attackedConversations.Remove(currentConversation);
            }
            else
            {
                attackedConvoIndex++;
                if (attackedConvoIndex > attackedConversations.Count - 1) { attackedConvoIndex = 0; }
            }
        }
        PlayConversation(currentConversation);
    }

    private void PlayConversation(DialogueConversation convo)
    {
        if (convo)
        {
            HideInteractable();
            onDialogueEnd = new UnityAction(FinishTalking);
            EventController.StartListening(EventController.EventType.DialogueEnd, onDialogueEnd);
            //TODO: Play talking animation 
            DialogueController.StartConversation(currentConversation);
        }
    }

    private void FinishTalking()
    {
        EventController.StopListening(EventController.EventType.DialogueEnd, onDialogueEnd);
        //Show exclamation mark if another active conversation available, unless it's the same one that can be repeated.
        if (activeConversations.Count > 0 && activeConversations[0] != currentConversation)
        {
            ShowInteractable();
        }
        currentConversation = null;
    }


    private void HideInteractable()
    {
        interactableMarker.SetActive(false);
    }
    private void ShowInteractable()
    {
        interactableMarker.SetActive(true);
        interactableMarker.GetComponent<Animator>().SetTrigger("Appear");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.GetComponent<EntangleProjectile>())
        {
            Attacked();
        }
    }

    //Functions to change what conversations player can currently have with this character         
    public void AddDoneConversation(DialogueConversation convo)
    {
        doneConversations.Add(convo);
    }

    public void AddDoneConversation(string conversationName, string levelName)//Conversations can be added either directly, or by giving the name and level of conversation
    {
        doneConversations.Add(GetDialogue(levelName,conversationName));
    }

    public void AddActiveConversation(DialogueConversation convo)
    {
        activeConversations.Add(convo);
        ShowInteractable();
    }
    public void AddActiveConversation(string conversationName, string levelName)
    {
        activeConversations.Add(GetDialogue(levelName,conversationName));
        ShowInteractable();
    }
    public void AddAttackedConversation(DialogueConversation convo)
    {
        attackedConversations.Add(convo);
    }

    public void AddAttackedConversation(string conversationName, string levelName)
    {
        attackedConversations.Add(GetDialogue(levelName, conversationName));
    }

    public void ClearActiveConversations()
    {
        activeConversations.Clear();
        HideInteractable();

    }
    public void ClearDoneConversations()
    {
        doneConversations.Clear(); 
    }

    public void ClearAttackedConversations()
    {
        attackedConversations.Clear();
    }


    public DialogueConversation GetDialogue(string levelName, string identifier)
    {
        string searchString = $"{levelName}_{gameObject.name}_{identifier}";
        foreach(DialogueConversation convo in dialogues)
        {
            if (convo.name == searchString)
            {
                return convo;
            }
        }
        Debug.Log($"<color=yellow>Cannot find dialogue of name {searchString}, please ensure all Dialogues are placed on the correct character with the name format of: '(LevelName)_(characterName)_(identifier)'</color>");
        return null;
    }
    
}
