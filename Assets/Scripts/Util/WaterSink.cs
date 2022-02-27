using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSink : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rigid = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigid) { rigid.gravityScale = 1.5f; }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D rigid = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigid) { rigid.gravityScale = 1f; }
    }

}
