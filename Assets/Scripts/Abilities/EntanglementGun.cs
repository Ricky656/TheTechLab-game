using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntanglementGun : MonoBehaviour
{
    public Transform centrePoint; //central point around which the gun aim spins 
    public float aimLength;
    public GameObject projectilePrefab;
    public float projectileSpeed;

    private Vector3 mousePosition;
    private bool controlLocked;
    private LineRenderer aimLine;
    private bool fire;
    private EntangleProjectile projectile; //This gun will only be able to fire a single bullet at a time, so storing a single object is sufficient here. 
    private Vector2 aimDirection; 
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
        projectile.Initialize();
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
                Debug.Log("Gun firing");
                LaunchProjectile();
            }
        }
        
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
        if (Input.GetButtonDown("Fire1"))
        {
            fire = true;
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
    }

    public void Disable()
    {
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
