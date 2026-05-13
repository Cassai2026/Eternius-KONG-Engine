using UnityEngine;
using TMPro;
using System;

public class PlayerController2D : MonoBehaviour
{
    Rigidbody2D body;
    float horizontal;
    float vertical;

    public float RunSpeed = 20.0f;
    public float DashSpeedMultiplier = 2.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1.0f;

    private bool isDashing = false;
    private bool isCooldown = false;
    private float currentDashTime = 0.0f;
    private float currentCooldownTime = 0.0f;

    public ProgressBar progressBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;

    private int xp = 0;
    private int level = 0;
    private int xpToNextLevel = 100;

    public Animator animator;

    [NonSerialized] public Vector2 moveToPos;
    [NonSerialized] public bool canMove = true;
    [NonSerialized] public bool isMovingToClick = false;

    private float minDistanceToStop = 0.2f; // Distance threshold to stop the player

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        moveToPos = transform.position;
    }

    void Update()
    {
        if (QuestGiverScript.IsQuestWindowActive)
        {
            StopMovement(); // Ensure the player stops immediately
            return;
        }

        if (isCooldown)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0)
            {
                isCooldown = false;
            }
        }

        if (Input.GetMouseButton(0) && canMove)
        {
            moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMovingToClick = true;
        }

        if (Vector2.Distance(transform.position, moveToPos) <= minDistanceToStop)
        {
            isMovingToClick = false;
            moveToPos = transform.position;
        }

        if (canMove)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontal = vertical = 0;
        }

        Vector2 moveDirection = Vector2.zero;

        if (isMovingToClick)
        {
            Vector2 position = transform.position;
            moveDirection = (moveToPos - position).normalized;
        }
        else
        {
            moveDirection = new Vector2(horizontal, vertical).normalized;
        }

        if (moveDirection.magnitude > 0.1f)
        {
            animator.SetFloat("PosX", moveDirection.x);
            animator.SetFloat("PosY", moveDirection.y);
            animator.SetBool("isWalking", true); // Set isWalking to true when moving
        }
        else
        {
            animator.SetFloat("PosX", 0);
            animator.SetFloat("PosY", 0);
            animator.SetBool("isWalking", false); // Set isWalking to false when not moving
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCooldown)
        {
            isDashing = true;
            currentDashTime = DashDuration;
            isCooldown = true;
            currentCooldownTime = DashCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            body.velocity = new Vector2(horizontal * RunSpeed * DashSpeedMultiplier, vertical * RunSpeed * DashSpeedMultiplier);

            currentDashTime -= Time.fixedDeltaTime;
            if (currentDashTime <= 0)
            {
                isDashing = false;
            }
        }
        else if (isMovingToClick)
        {
            Vector2 position = transform.position;
            Vector2 moveDir = (moveToPos - position).normalized;
            body.velocity = moveDir * RunSpeed;
        }
        else
        {
            body.velocity = new Vector2(horizontal * RunSpeed, vertical * RunSpeed);
        }
    }

    // Method to immediately stop the player's movement
    public void StopMovement()
    {
        body.velocity = Vector2.zero; // Immediately stop the player
        isMovingToClick = false;
        horizontal = 0;
        vertical = 0;
        moveToPos = transform.position;

        // Ensure the player is not moving
        animator.SetBool("isWalking", false);
    }

    // Method to freeze the player's position
    public void FreezePosition()
    {
        canMove = false;
        StopMovement(); // Call to stop the player immediately
    }

    // Method to unfreeze the player's position
    public void UnfreezePosition()
    {
        canMove = true;
    }

    private void UpdateAnimatorState()
    {
        if (canMove && !isMovingToClick && !QuestGiverScript.IsQuestWindowActive)
        {
            animator.SetBool("isWalking", (horizontal != 0 || vertical != 0));
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    // Handle collision with NPCs
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SmokerNPC"))
        {
            // Check if the player is starting dialogue with the NPC
            NPCController npc = collision.gameObject.GetComponent<NPCController>();
            if (npc != null)
            {
                FreezePosition(); // Stop the player immediately upon interaction
                // Start the dialogue or interaction logic here...
            }
        }
        
        if (collision.CompareTag("HardHatNPC"))
        {
            // Check if the player is starting dialogue with the NPC
            NPCController npc = collision.gameObject.GetComponent<NPCController>();
            if (npc != null)
            {
                FreezePosition(); // Stop the player immediately upon interaction
                // Start the dialogue or interaction logic here...
            }
        }
    }
}
