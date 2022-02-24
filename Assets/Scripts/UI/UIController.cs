using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{

    public GameObject fadeBox; //Used to fade camera to/from black
    public const float defaultFadeSpeed = 1f;

    private static UIController controller;

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

    //public DialogueConversation testConversation;


    //private UnityAction onDialogueEnd; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   /* public void StartGame()
    {
        GameObject.Find("StartMenu").gameObject.SetActive(false);
        Debug.Log("Starting game");

        StartCoroutine(CameraFade());
        StartCoroutine(openingSequence());
        
        
    }*/

    /*private void startEventTest()
    {
        EventController.StopListening(EventController.EventType.DialogueEnd, onDialogueEnd);
        StartCoroutine(CameraFade(false));
    }

    private IEnumerator openingSequence()
    {
        yield return new WaitForSeconds(3);
        DialogueController.StartConversation(testConversation);

        onDialogueEnd = new UnityAction(startEventTest);
        EventController.StartListening(EventController.EventType.DialogueEnd, onDialogueEnd);
    }*/


    public static IEnumerator CameraFade(bool FadeOut = true, float speed = defaultFadeSpeed)//Fade camera to/from black, lower speed = faster fade
    {
        Debug.Log("fading");
        GameObject box = controller.fadeBox;
        box.SetActive(true);
        Color fadeColor = box.GetComponent<Image>().color;
        float alpha;

        if (FadeOut)
        {
            while (box.GetComponent<Image>().color.a < 1)
            {
                alpha = fadeColor.a + (speed * Time.deltaTime);
                fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                box.GetComponent<Image>().color = fadeColor;
                yield return null;
            }
        }
        else
        {
            while (box.GetComponent<Image>().color.a > 0)
            {
                alpha = fadeColor.a - (speed * Time.deltaTime);
                fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                box.GetComponent<Image>().color = fadeColor;
                yield return null;
            }
            box.SetActive(false);
        }
    }
}
