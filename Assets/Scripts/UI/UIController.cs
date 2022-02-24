using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIController : MonoBehaviour
{

    public GameObject fadeBox; //Used to fade camera to/from black
    public const float defaultFadeSpeed = 1f;

    public DialogueConversation testConversation; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameObject.Find("StartMenu").gameObject.SetActive(false);
        Debug.Log("Starting game");
        StartCoroutine(CameraFade());
        StartCoroutine(openingSequence());
        
        
    }

    private IEnumerator openingSequence()
    {
        yield return new WaitForSeconds(3);
        DialogueController.StartConversation(testConversation);
    }


    public IEnumerator CameraFade(bool FadeOut = true, float speed = defaultFadeSpeed)//Fade camera to/from black, lower speed = faster fade
    {
        fadeBox.SetActive(true);
        Color fadeColor = fadeBox.GetComponent<Image>().color;
        float alpha;

        if (FadeOut)
        {
            while (fadeBox.GetComponent<Image>().color.a < 1)
            {
                alpha = fadeColor.a + (speed * Time.deltaTime);
                fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                fadeBox.GetComponent<Image>().color = fadeColor;
                yield return null;
            }
        }
        else
        {
            while (fadeBox.GetComponent<Image>().color.a > 0)
            {
                alpha = fadeColor.a - (speed * Time.deltaTime);
                fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                fadeBox.GetComponent<Image>().color = fadeColor;
                yield return null;
            }
            fadeBox.SetActive(false);
        }
    }
}
