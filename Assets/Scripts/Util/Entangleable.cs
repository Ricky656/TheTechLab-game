using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Entangleable : MonoBehaviour
{
    private GameObject entangledPair;
    private bool entangled;

    public void Entangle(GameObject entangledBy)
    {
        entangledPair = entangledBy;
        entangled = true;
    }

    public void Disentangle()
    {
        entangled = false;
        entangledPair = null;
    }

    private void FixedUpdate()
    {
        if (entangled)
        {
            Vector3 velocity = entangledPair.GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }
}
