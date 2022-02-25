using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Character
{
    public float moveSpeed;
    public float jumpStrength;
    public Transform feetLocation;
    public float colCheckRadius;
    public LayerMask walkableObjects;
    public Animator animator;

    private bool controlLocked;
    private Rigidbody2D rigid;
    private float moveDirection;
    private bool jump;
    private bool onGround;
    private List<Item> inventory; 

    private UnityAction onPlayerLock;
    private UnityAction onPlayerUnlock;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        controlLocked = true;
        inventory = new List<Item>();

        onPlayerLock = new UnityAction(LockControls);
        onPlayerUnlock = new UnityAction(UnlockControls);
    }

    private void OnEnable()//Set up event listeners that can be used to lock player movement from any relevent trigger
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
    private void CheckInput()
    {
        moveDirection = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump") && onGround)
        {
            jump = true; 
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

    private void LockControls()
    {
        controlLocked = true;
    }

    private void UnlockControls()
    {
        controlLocked = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        switch (obj.tag)
        {
            case "Pickup":
                inventory.Add(obj.GetComponent<Item>()); //Add pickup to inventory, disable object and send event saying quest has been completed. (All pickups are 'quest objectives')
                obj.SetActive(false);
                EventController.TriggerEvent(EventController.EventType.QuestCompleted);
                SendPickupMessage(obj.GetComponent<Item>());
                break;
            case "Water":

                break;
        }
    }

    private void SendPickupMessage(Item obj)//Build a new dialogue conversation object and send it to the respective controller 
    {
        DialogueConversation convo = ScriptableObject.CreateInstance("DialogueConversation") as DialogueConversation;
        DialogueLine message = new DialogueLine();
        message.text = obj.pickupMessage + obj.itemName;    
        convo.gameHalt = false;
        convo.dialogueLines = new DialogueLine[] { message };

        DialogueController.StartConversation(convo);
    }
}
