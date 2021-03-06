using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Character, IInteractable, ISaveable<NPCData>
{
    public GameObject interactableMarker;//Every NPC has their own exclamation mark, shown if they have an active conversation.
    public DialogueConversation[] dialogues; //All conversations this character can use in the game

    private List<DialogueConversation> activeConversations; //Current possible conversations split by type
    private List<DialogueConversation> doneConversations;  //^
    private List<DialogueConversation> attackedConversations; //^
    private List<DialogueConversation> deathConversations; //^
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
        deathConversations = new List<DialogueConversation>(); 
        if (!interactableMarker && dialogues.Length>0) { Debug.Log($"<color=yellow>{gameObject.name} missing dialogueMarker!</color>"); }
    }
    
    public void Interact(GameObject interactingObject)
    {
        if(interactingObject.tag == "Player")
        {
            Debug.Log($"{gameObject.name} is trying to talk");
            Talk();
        }
    }

    public void Die()
    {
        if(deathConversations.Count > 0)
        {
            int rnd = Random.Range(0, attackedConversations.Count);
            currentConversation = deathConversations[rnd];
            DialogueController.StartConversation(currentConversation);
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.GetComponent<EntangleProjectile>())
        {
            Attacked();
        }
    }


    #region  Alter current conversations of NPC    
    public void AddDoneConversation(DialogueConversation convo)//Conversations can be added either directly, or by giving the name and level of conversation
    {
        doneConversations.Add(convo);
    }

    public void AddDoneConversation(string conversationName, string levelName)
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

    public void AddDeathConversation(string conversationName, string levelName)
    {
        deathConversations.Add(GetDialogue(levelName, conversationName));
    }

    public void AddDeathConversation(DialogueConversation convo)
    {
        deathConversations.Add(convo);
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
        attackedConvoIndex = 0; 
    }

    public void ClearDeathConversations()
    {
        deathConversations.Clear();
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
    #endregion

    #region save/load code
    public NPCData Save()
    {
        NPCData data = new NPCData(gameObject.name, transform.position, gameObject.activeSelf);
        data.activeConvoPointers = FindConvoPointers(activeConversations);
        data.doneConvoPointers = FindConvoPointers(doneConversations);
        data.attackedConvoPointers = FindConvoPointers(attackedConversations);
        data.deathConvoPointers = FindConvoPointers(deathConversations);
        return data;
    }

    public void Load(NPCData data)
    {
        transform.position = data.GetPosition();
        gameObject.SetActive(data.GetActive());
        activeConversations.Clear();
        doneConversations.Clear();
        attackedConversations.Clear();
        deathConversations.Clear();
        SetConvosFromPointers(activeConversations, data.activeConvoPointers);
        SetConvosFromPointers(doneConversations, data.doneConvoPointers);
        SetConvosFromPointers(attackedConversations, data.attackedConvoPointers);
        SetConvosFromPointers(deathConversations, data.deathConvoPointers);
    }

    //Creates int 'pointers' that store the 'dialogues' index of conversations currently in use by NPC.
    //This can be serialized and therefore used to save/load this aspect of the game between sessions
    private int[] FindConvoPointers(List<DialogueConversation> list)
    {
        int[] pointers = new int[list.Count];
        for(int x =0; x < list.Count; x++)
        {
            for(int y=0; y < dialogues.Length; y++)
            {
                if(dialogues[y] == list[x])
                {
                    pointers[x] = y;
                    break;
                }
            }
        }

        return pointers;
    }

    private void SetConvosFromPointers(List<DialogueConversation> list, int[] pointers)
    {
        foreach(int index in pointers)
        {
            list.Add(dialogues[index]);
        } 
    }
    #endregion
}

