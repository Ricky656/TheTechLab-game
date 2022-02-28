using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSink : MonoBehaviour
{
    public float deathTimer = 2f;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rigid = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigid) { rigid.gravityScale = 1.5f; }

        switch (col.gameObject.tag)
        {
            case "Player":
                StartCoroutine(DeathTimer());
                break;
            case "NPC":
                col.gameObject.GetComponent<NPC>().Die();
                break;

        }   
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D rigid = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigid) { rigid.gravityScale = 1f; }
        if (col.gameObject.tag == "Player")
        {
            StopAllCoroutines();
        }
    }


    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(deathTimer);
        EventController.TriggerEvent(EventController.EventType.PlayerDied);
    }

}
