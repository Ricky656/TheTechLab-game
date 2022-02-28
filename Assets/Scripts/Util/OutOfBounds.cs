using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OutOfBounds : MonoBehaviour
{


    public void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Player":
                EventController.TriggerEvent(EventController.EventType.PlayerDied);
                break;
            case "NPC":
                col.gameObject.GetComponent<NPC>().Die();
                break;
        }

    }
}
