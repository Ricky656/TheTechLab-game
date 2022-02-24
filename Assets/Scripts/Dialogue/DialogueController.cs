using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public TextMeshProUGUI nextIndicator; 

    private bool busy; 
    private DialogueConversation currentConversation;
    private int currentLineIndex; 
    private static DialogueController controller; //Allows other scripts to access the dialogue controller without having to give every class a specific gameobject reference
    private Dictionary<char, float> charSpeedDictionary; 

    private void Awake()
    {
        if (controller == null)
        {
            controller = this; //Set static reference to this, there should only be 1 instance of DialogueController 
        }

        Initialize();
    }

    private void Initialize()
    {
        charSpeedDictionary = new Dictionary<char, float>//This allows particular chars to have a modified typing speed, allowing more varied articulation in dialogue
        {
            { ',', 1.5f },//I.E. 1.2 * text speed, higher is slower
            { '.', 2f },
            { '!', 1.3f }
        };
        nextIndicator.gameObject.SetActive(false);
        gameObject.SetActive(false);
        busy = false;
    }

    public void Update()
    {
        if (busy && currentConversation.gameHalt)
        {
            if (CheckInput())//Lets player skip typing animation, or go to next line 
            {
                if (textBox.text == currentConversation.dialogueLines[currentLineIndex].text)
                {
                    nextIndicator.gameObject.SetActive(false);
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textBox.text = currentConversation.dialogueLines[currentLineIndex].text;
                    nextIndicator.gameObject.SetActive(true);
                }
            }
        }
    }


    public static bool IsBusy()
    {
        return controller.busy;
    }
    public static void StartConversation(DialogueConversation conversation)
    {
        if (controller == null) { Debug.Log("No dialogue controller!"); return; }

        controller.busy = true;
        controller.gameObject.SetActive(true);
        //TODO: animation for dialoguebox appearing?
        //TODO: React if already in a conversation? 

        controller.currentLineIndex = 0;
        controller.currentConversation = conversation;
        controller.textBox.text = "";

        //TODO: Lock player if conversation.halt == true 
        controller.StartCoroutine(controller.TypeLine());
    }

    public static void EndConversation()//Immediately ends current conversation
    {
        if (controller == null) { Debug.Log("No dialogue controller!"); return; }

        EventController.TriggerEvent(EventController.EventType.DialogueEnd);
        //TODO: animation for dialoguebox disappearing? 
        controller.busy = false;
        controller.gameObject.SetActive(false);
    }

    private IEnumerator TypeLine()//Displays current dialogue line using a typing effect, speed is set and can be customized per line in dialogueline objects
    {
        foreach(char letter in currentConversation.dialogueLines[currentLineIndex].text.ToCharArray())
        {
            textBox.text += letter;
            float waitTime = currentConversation.dialogueLines[currentLineIndex].textSpeed;
            float timeModifier;
            if(charSpeedDictionary.TryGetValue(letter, out timeModifier))//Typing speed is modified if current char has an entry in the dictionary, creating pauses for '.'s for example
            {
                waitTime *= timeModifier;
            }
            //TODO: Play sound for each letter? (Very Undertale) 
            yield return new WaitForSeconds(waitTime);
        }
        if (!currentConversation.gameHalt)//Automatically go to next line after short delay if this isn't an interactive dialogue 
        {
            yield return new WaitForSeconds(4);
            NextLine();
        }
        else
        {
            nextIndicator.gameObject.SetActive(true); 
        }
    }

    private void NextLine()
    {
        EventController.TriggerEvent(EventController.EventType.DialogueLineFinish);
        currentLineIndex++;
        if(currentLineIndex < currentConversation.dialogueLines.Length)
        {
            textBox.text = "";
            StartCoroutine(TypeLine());
        }
        else
        {
            EndConversation();
        }
    }

    private bool CheckInput()
    {
        if (Input.GetKeyDown("space"))//TODO: Enable mouse click/Enter key as well 
        {
            return true;
        }
        return false;
    }
}
