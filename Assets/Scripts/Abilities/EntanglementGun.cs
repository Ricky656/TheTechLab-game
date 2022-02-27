using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class EntanglementGun : MonoBehaviour
{
    public Transform centrePoint; //central point around which the gun aim spins 
    public float aimLength;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public GameObject particleEffectPrefab;

    private GameObject particleEffect;
    private Vector3 mousePosition;
    private bool controlLocked;
    private LineRenderer aimLine;
    private bool fire;
    private EntangleProjectile projectile; //This gun will only be able to fire a single bullet at a time, so storing a single object is sufficient here. 
    private Vector2 aimDirection;
    private GameObject entangledObject;
 
    private void Awake()
    {
        aimLine = centrePoint.gameObject.AddComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = Color.white;
        aimLine.endColor = Color.white;
        aimLine.startWidth = 0.05f;
        aimLine.endWidth = 0.05f;
        projectile = Instantiate(projectilePrefab).GetComponent<EntangleProjectile>();
        projectile.Initialize(this);
        particleEffect = Instantiate(particleEffectPrefab);
        particleEffect.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - (GetComponent<BoxCollider2D>().bounds.extents.y));
        particleEffect.transform.parent = gameObject.transform;
        particleEffect.SetActive(false);
        Disable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlLocked)
        {
            CheckInput();
        }
    }

    
    private void FixedUpdate()
    {
        if (!controlLocked)
        {
            DrawAimLine();
            if (fire && !projectile.IsActive())
            {
                LaunchProjectile();
            }
        }
        
    }

    private void BulletHit(object data)//Data should be formatted as GameObject[]
    {
        GameObject[] objects = (GameObject[])data;
        if(objects[0] == gameObject)
        {
            if (objects[1].gameObject.GetComponent<Entangleable>())
            {
                Debug.Log($"{gameObject.ToString()} is now entangled with: {objects[1].ToString()}");
                entangledObject = objects[1];
                entangledObject.GetComponent<Entangleable>().Entangle(gameObject);
                particleEffect.SetActive(true);
            }
        }
    }

    private void Disentangle()
    {
        entangledObject.GetComponent<Entangleable>().Disentangle();
        entangledObject = null;
        particleEffect.SetActive(false);
    }
    private void LaunchProjectile()
    {
        Vector2 startPos = (Vector2)centrePoint.transform.position + (aimDirection * aimLength);
        projectile.GetComponent<EntangleProjectile>().Fire(startPos, aimDirection * projectileSpeed);
        fire = false;
    }

    private void CheckInput()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButtonDown("Fire1") && entangledObject == null)
        {
            fire = true;
        }
        if (Input.GetButtonDown("Disentangle"))
        {
            Disentangle();
        }
    }

    private void DrawAimLine()
    {
        aimDirection = ((Vector2)mousePosition - (Vector2)centrePoint.transform.position).normalized;
        aimLine.SetPosition(0, centrePoint.transform.position);
        aimLine.SetPosition(1, (Vector2)centrePoint.transform.position + (aimDirection * aimLength));
    }

    public void Enable()
    {
        this.enabled = true;
        EventController.StartListening(EventController.EventType.BulletHit, BulletHit);
    }

    public void Disable()
    {
        EventController.StopListening(EventController.EventType.BulletHit, BulletHit);
        this.enabled = false;
    }

    public void LockControl(bool locked)
    {
        controlLocked = locked;
        if (locked) 
        { 
            fire = false;
            aimLine.enabled = false;
        }
        else
        {
            aimLine.enabled = true; 
        }
    }
}
