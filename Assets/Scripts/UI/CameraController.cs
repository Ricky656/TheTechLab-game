using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    public GameObject fadeBox; //Used to fade camera to/from black
    
    private const float defaultFadeSpeed = 1f;
    private const float defaultPanSpeed = 0.005f;
    private const float snapDistance = 0.02f;//distance before camera will snap to target and stop panning
    private static CameraController controller;

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
    }

    public static void CameraMove(GameObject cameraFlag, bool pan = false, float panSpeed = defaultPanSpeed)
    {
        if (cameraFlag == null) { Debug.Log($"<color=yellow>Cannot move camera: no camera flag!</color>"); return; }

        Vector3 location = new Vector3(cameraFlag.transform.position.x, cameraFlag.transform.position.y, controller.gameObject.transform.position.z);
        float zoomDistance = cameraFlag.GetComponent<CameraFlag>().distance; 
        if (!pan)
        {
            controller.SetPosition(location, zoomDistance);
        }
        else
        {
            //controller.PanCamera(location, defaultPanSpeed);
            controller.StartCoroutine(controller.PanTo(location,zoomDistance,panSpeed));
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

    private void SetPosition(Vector3 location, float zoomDistance)
    {
        gameObject.transform.position = location;
        gameObject.GetComponent<Camera>().orthographicSize = zoomDistance; 
    }

    private IEnumerator PanTo(Vector3 location, float zoomDistance, float panSpeed)
    {
        bool finishedPanning = false;
        while (!finishedPanning)
        {
            Vector3 distance = location - gameObject.transform.position;  //(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
            //Check if close enough to finish
            if (distance.magnitude < snapDistance)
            {
                gameObject.transform.position = location;
                gameObject.GetComponent<Camera>().orthographicSize = zoomDistance; 
                finishedPanning = true;
            }
            else
            {//Move Camera
                Vector3 direction = distance.normalized;
                Vector3 movement = direction * panSpeed;
                gameObject.transform.position += movement;

                float zoomSpeed = (zoomDistance - gameObject.GetComponent<Camera>().orthographicSize) / (distance.magnitude / movement.magnitude) ;
                gameObject.GetComponent<Camera>().orthographicSize += zoomSpeed;
            }


            yield return new WaitForSeconds(Time.deltaTime); //deltaTime is time between frames, ensuring consistant camera pan regardless of fps 
        }
    }
}
