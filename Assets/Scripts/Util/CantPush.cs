using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CantPush : MonoBehaviour //Stops the player from being able to push these objects
{
    private Rigidbody2D rigid;
    private Collider2D col;
    private GameObject blocker; 

    private void Awake()//Creates a 'blocker' that surrounds the gameObject and prevents it from being pushed by anything else
    {
        blocker = Instantiate(Resources.Load("Markers/collisionBlocker")) as GameObject;
        blocker.transform.localScale = gameObject.transform.localScale * 1.01f; //10% larger
        blocker.transform.parent = transform;
        blocker.transform.localPosition = Vector2.zero;
        rigid = blocker.AddComponent<Rigidbody2D>();
        col = blocker.AddComponent<BoxCollider2D>();
        rigid.isKinematic = true;
        Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());


    }
    
}
