using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CantPush : MonoBehaviour //Stops the player from being able to push these objects
{
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.gameObject.tag == "Player")
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            rigid.isKinematic = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.gameObject.tag == "Player")
        {
            rigid.isKinematic = false;
        }
    }
}
