using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Dialogue/Create Conversation")]
public class DialogueConversation : ScriptableObject
{
    public bool gameHalt = true; //Should the game halt and let the player interact with dialogue, or is it just a message that displays during gameplay. 
    public bool repeatable = false;
    public DialogueLine[] dialogueLines; 
}
