using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntangleProjectile : MonoBehaviour
{

    public ParticleSystem particleTrail;

    private const float lifeTime = 2f;
    private EntanglementGun gun;

    public void Initialize(EntanglementGun owner = null)
    {
        gameObject.SetActive(false);
        gun = owner;
    }

    public void Fire(Vector2 startPos, Vector2 velocity)
    {
        gameObject.transform.position = startPos;
        gameObject.SetActive(true);
        GetComponent<Rigidbody2D>().velocity = velocity;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.rotation = Vector2.Angle(startPos, startPos - velocity);
        particleTrail.Emit(emitParams, 10);
        StartCoroutine(timedLife());
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject[] entangledObjects = new GameObject[] {gun.gameObject, collision.collider.gameObject };
        EventController.TriggerEvent(EventController.EventType.BulletHit, entangledObjects);
        Dissapate();
    }

    private IEnumerator timedLife()
    {
        yield return new WaitForSeconds(lifeTime);
        Dissapate();
    }

    private void Dissapate()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
    }
}
