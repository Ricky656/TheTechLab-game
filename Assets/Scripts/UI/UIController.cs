using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public CameraController mainCamera; 
    public GameObject fadeBox; //Used to fade camera to/from black
    public const float defaultFadeSpeed = 1f;
    public const float defaultPanSpeed = 0.5f; 

    private static UIController controller;

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

    public static IEnumerator CameraFade(bool FadeOut = true, float speed = defaultFadeSpeed)//Fade camera to/from black, lower speed = faster fade
    {
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

    public static void CameraMove(GameObject cameraFlag, bool pan = false, float panSpeed = defaultPanSpeed)
    {
        Vector2 location = new Vector2(cameraFlag.transform.position.x, cameraFlag.transform.position.y);
        if (!pan)
        {
            controller.mainCamera.SetPosition(location);
        }
        else
        {
            controller.mainCamera.PanCamera(location, defaultPanSpeed);
        }
    }
}
