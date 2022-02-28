using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EntanglementGun))]
public class PlayerController : Character, ISaveable<PlayerData>
{
    public float moveSpeed;
    public float jumpStrength;
    public Transform feetLocation;
    public Transform interactLocation;
    public float colCheckRadius;
    public float interactCheckRadius;
    public LayerMask walkableObjects;
    public Animator animator;

    private bool controlLocked;
    private Rigidbody2D rigid;
    private float moveDirection;
    private bool jump;
    private bool onGround;
    private List<Item> inventory;
    private bool facingRight;

    private UnityAction onPlayerLock;
    private UnityAction onPlayerUnlock;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        controlLocked = true;
        inventory = new List<Item>();
        facingRight = true;

        onPlayerLock = new UnityAction(LockControls);
        onPlayerUnlock = new UnityAction(UnlockControls);
    }

    private void OnEnable()//Set up event listeners that can be used to lock player movement
    {
        EventController.StartListening(EventController.EventType.PlayerLocked, onPlayerLock);
        EventController.StartListening(EventController.EventType.PlayerUnlocked, onPlayerUnlock);
    }

    private void OnDisable()
    {
        EventController.StopListening(EventController.EventType.PlayerLocked, onPlayerLock);
        EventController.StopListening(EventController.EventType.PlayerUnlocked, onPlayerUnlock);
    }

    private void Update()
    {
        if (!controlLocked)
        {
            CheckInput();
        }
    }


    private void FixedUpdate()
    {
        Move();

        //Check if player is currently touching the ground and can therefore jump 
        onGround = Physics2D.OverlapCircle(feetLocation.position, colCheckRadius, walkableObjects);
    }

    public List<Item> GetInventory()
    {
        return inventory;
    }

    private void CheckInput() //Input buttons are defined in Unity Project Settings. E.G. Jump is defined as 'w' or 'up' keys
    {
        moveDirection = Input.GetAxis("Horizontal");
        if (moveDirection > 0 && !facingRight)
        {
            ChangeFacing();
        }//Don't want to use an 'else' here so that if movementDirection is 0 player keeps whatever the previous facing was 
        if (moveDirection < 0 && facingRight)
        {
            ChangeFacing();
        }

        if (Input.GetButtonDown("Jump") && onGround)
        {
            jump = true; 
        }
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }


    private void Move()
    {
        rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);
        if (jump)
        {
            animator.SetTrigger("Jump");//Play jump animation 
            rigid.AddForce(new Vector2(0f, jumpStrength));
            jump = false;
        }
    }

    private void ChangeFacing()
    {//Character changes direction, mainly used for the Interact functionality to work accurately
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f); 
    }

    private void Interact()//Attempt to interact with object in front of charcter
    {
        Debug.Log("Trying to interact");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactLocation.position, interactCheckRadius);
        IInteractable targetInteraction = null;
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.GetComponent<IInteractable>() !=null)
            {
                targetInteraction = col.gameObject.GetComponent<IInteractable>();
            }
            
        }
        if (targetInteraction !=null) { targetInteraction.Interact(gameObject); }
          
    }

    private void LockControls()
    {
        moveDirection = 0;
        controlLocked = true;
        GetComponent<EntanglementGun>().LockControl(true);
    }

    private void UnlockControls()
    {
        controlLocked = false;
        GetComponent<EntanglementGun>().LockControl(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        switch (obj.tag)
        {
            case "Pickup":
                Item item = obj.GetComponent<Item>();
                inventory.Add(obj.GetComponent<Item>()); //Add pickup to inventory, disable object and send event saying quest has been completed. (All pickups are 'quest objectives')
                obj.SetActive(false);
                EventController.TriggerEvent(EventController.EventType.QuestCompleted);
                SendPickupMessage(obj.GetComponent<Item>());
                break;
        }
    }

    private void SendPickupMessage(Item obj)//Build a new dialogue conversation object and send it to the respective controller 
    {
        DialogueConversation convo = ScriptableObject.CreateInstance("DialogueConversation") as DialogueConversation;
        DialogueLine message = new DialogueLine();
        message.text = obj.pickupMessage + obj.itemName + "!";    
        convo.gameHalt = false;
        convo.dialogueLines = new DialogueLine[] { message };

        DialogueController.StartConversation(convo);
    }


    public PlayerData Save()
    {
        PlayerData data = new PlayerData(transform.position, inventory);
        return data;
    }

    public void Load(PlayerData data)
    {
        transform.position = data.GetPosition();
        inventory = data.GetInventory();
        if (data.HasGun())
        {
            GetComponent<EntanglementGun>().Enable();
            GetComponent<EntanglementGun>().LockControl(false);
        }
        else
        {
            GetComponent<EntanglementGun>().Disable();
            GetComponent<EntanglementGun>().LockControl(true);
        }
    }
    
}
