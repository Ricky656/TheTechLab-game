using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private const float snapDistance = 2f; 

    public void SetPosition(Vector2 location)
    {
        gameObject.transform.position = new Vector3(location.x, location.y, gameObject.transform.position.z);
    }

    public IEnumerator PanCamera(Vector2 location, float panSpeed)
    {
        bool finishedPanning = false;
        while (!finishedPanning)
        {
            Vector3 distance = location - (new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
            //Check if close enough to finish
            if(distance.magnitude < snapDistance)
            {
                gameObject.transform.position = location;
                finishedPanning = true;
            }
            else
            {//Move Camera
                Vector2 direction = distance.normalized;
                Vector2 movement = direction * panSpeed;
                gameObject.transform.position += new Vector3(movement.x, movement.y, 0);
            }
            
            
            yield return null;
        }
    }
}
