using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    public GameObject fadeBox; //Used to fade camera to/from black
    public GameObject followedObject;
    public Transform minPosition;
    public Transform maxPosition;
    public float moveDelay;
    public float moveSmoothness;
    //public float cameraHeight = 2; //How high is the camera raised above the object it's following
    

    private const float defaultFadeSpeed = 1f;
    private const float defaultPanSpeed = 0.005f;
    private const float snapDistance = 0.02f;//distance before camera will snap to target and stop panning
    private static CameraController controller;
    private Vector2 velocity;
    private CameraMode currentMode;

    public enum CameraMode
    {
        Cinematic,
        Normal,
        Paused,
    }

    public void Awake()
    {
        if (controller == null)
        {
            controller = this; 
        }
        currentMode = CameraMode.Cinematic;
        if (moveSmoothness <= 0) { moveSmoothness = defaultPanSpeed; }
    }

    public void Update()
    {
        if(GameController.GetGameState() == GameController.GameState.Paused) { return; }
        switch (currentMode)
        {
            case CameraMode.Normal:
                FollowTarget();
                break;
        }
    }

    public static void SetCameraMode(CameraMode mode)
    {
        controller.currentMode = mode;
    }

    public static CameraMode GetCameraMode()
    {
        return controller.currentMode; 
    }

    public static void CameraMove(GameObject cameraFlag, bool pan = false, float panSpeed = defaultPanSpeed)
    {
        controller.StopAllCoroutines();//If camera is already panning, we must stop that first
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

    public static void SetCameraFade(bool FadeOut = true, float speed = defaultFadeSpeed)
    {
        controller.StopCoroutine(controller.CameraFade());//stops any camera fades already running, then starts a new one.
        controller.StartCoroutine(controller.CameraFade(FadeOut, speed));
    }

    private IEnumerator CameraFade(bool FadeOut = true, float speed = defaultFadeSpeed)//Fade camera to/from black, lower speed = faster fade
    {
        while(GameController.GetGameState() == GameController.GameState.Paused)
        {
            yield return null;
        }
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
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (box.GetComponent<Image>().color.a > 0)
            {
                alpha = fadeColor.a - (speed * Time.deltaTime);
                fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                box.GetComponent<Image>().color = fadeColor;
                yield return new WaitForFixedUpdate();
            }
            box.SetActive(false);
        }
        EventController.TriggerEvent(EventController.EventType.CameraFadeComplete);
    }

    private void SetPosition(Vector3 location, float zoomDistance)
    {
        gameObject.transform.position = location;
        gameObject.GetComponent<Camera>().orthographicSize = zoomDistance; 
    }

    private IEnumerator PanTo(Vector3 location, float zoomDistance, float panSpeed)
    {
        while (GameController.GetGameState() == GameController.GameState.Paused)
        {
            yield return null;
        }
        bool finishedPanning = false;
        while (!finishedPanning)
        {
            Vector3 distance = location - gameObject.transform.position;
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

    private void FollowTarget()
    {
        Vector2 targetPosition = new Vector2(followedObject.transform.position.x, followedObject.transform.position.y);

        float xTarget = Mathf.Clamp(targetPosition.x, minPosition.position.x, maxPosition.position.x);
        float yTarget = Mathf.Clamp(targetPosition.y, minPosition.position.y, maxPosition.position.y);

        float xPos = Mathf.SmoothDamp(transform.position.x, xTarget, ref velocity.x, moveSmoothness);
        float yPos = Mathf.SmoothDamp(transform.position.y, yTarget, ref velocity.y, moveSmoothness);
        transform.position = new Vector3(xPos,yPos,transform.position.z); 
    }
}
