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


    private void Awake()//Set static reference to this, there should only be 1 instance of DialogueController 
    {
        if (controller == null)
        {
            controller = this; 
        }
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
        //TODO: animation for dialoguebox disappearing? 
        controller.busy = false;
        controller.gameObject.SetActive(false);
    }

    private IEnumerator TypeLine()//Displays current dialogue line using a typing effect, speed is set and can be customized per line in dialogueline objects
    {
        foreach(char letter in currentConversation.dialogueLines[currentLineIndex].text.ToCharArray())
        {
            textBox.text += letter;
            yield return new WaitForSeconds(currentConversation.dialogueLines[currentLineIndex].textSpeed);
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
