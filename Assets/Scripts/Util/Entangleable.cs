using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Entangleable : MonoBehaviour
{
    public GameObject particleEffectPrefab;

    private GameObject entangledPair;
    private GameObject particleEffect;
    private bool entangled;

    public void Awake()
    {
        gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        particleEffect = Instantiate(particleEffectPrefab);
        particleEffect.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - (GetComponent<BoxCollider2D>().bounds.extents.y));
        particleEffect.SetActive(false);//TODO: Refactor this block, code also appears in EntanglementGun! 
        particleEffect.transform.parent = gameObject.transform;
    }
    public void Entangle(GameObject entangledBy)
    {
        entangledPair = entangledBy;
        entangled = true;
        particleEffect.SetActive(true);
    }

    public void Disentangle()
    {
        entangled = false;
        entangledPair = null;
        particleEffect.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (entangled)
        {
            Vector2 entangledVelocity = entangledPair.GetComponent<Rigidbody2D>().velocity;
            Vector2 newVelocity = new Vector2(entangledVelocity.x, entangledVelocity.y / 1.5f);

             GetComponent<Rigidbody2D>().velocity = newVelocity; //Entangled objects only get reduced y velocity to avoid infinite jumping

            //GetComponent<Rigidbody2D>().AddForce(new Vector2(velocity.x, velocity.y / 1.5f)*5);
        }
    }

    public bool IsEntangled()
    {
        return entangled;
    }
}
