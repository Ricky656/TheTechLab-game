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
                else if(textBox.text.Length > 3)//Stops people from accidentally skipping dialogue with a small grace period
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
        if (IsBusy())
        {
            if (controller.currentConversation.gameHalt)//Don't allow conversations that halt the game to be interrupted by other messages
            {
                return;
            }
            else
            {
                controller.StopAllCoroutines();//Interrupt current conversation and then new one 
            }
        }

        controller.busy = true;
        controller.gameObject.SetActive(true);
        //TODO: React if already in a conversation? 
        controller.currentLineIndex = 0;
        controller.currentConversation = conversation;
        controller.textBox.text = "";
        if (conversation.gameHalt)
        {
            EventController.TriggerEvent(EventController.EventType.PlayerLocked);
        }
        controller.gameObject.GetComponent<Animator>().SetBool("show", true);
        controller.StartCoroutine(controller.TypeLine());
    }

    public static void EndConversation()//Immediately ends current conversation
    {
        if (controller == null) { Debug.Log("No dialogue controller!"); return; }
        if(controller.currentConversation == null) { return; }
        controller.busy = false;
        if (controller.currentConversation.gameHalt)
        {
            EventController.TriggerEvent(EventController.EventType.PlayerUnlocked);
        }
        //controller.gameObject.SetActive(false);
        controller.gameObject.GetComponent<Animator>().SetBool("show", false);
        EventController.TriggerEvent(EventController.EventType.DialogueEnd);
    }

    private IEnumerator TypeLine()//Displays current dialogue line using a typing effect, speed is set and can be customized per line in dialogueline objects
    {
        bool usingMarkup = false;
        foreach(char letter in currentConversation.dialogueLines[currentLineIndex].text.ToCharArray())
        {
            textBox.text += letter;
            //These checks allow for markup to be used in strings displayed, skipping the wait time so that E.G.'<color=yellow>' isn't written out 1 characters at a time like the rest of the text
            if (letter == '<' || usingMarkup)
            {
                usingMarkup = true;
                if(letter == '>')
                {
                    usingMarkup = false;
                }
                continue;
            }
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
            yield return new WaitForSeconds(2.5f);
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
        if (Input.GetButtonDown("DialogueNext"))
        {
            return true;
        }
        return false;
    }
}
