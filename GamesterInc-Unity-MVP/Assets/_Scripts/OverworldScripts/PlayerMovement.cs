using UnityEngine;
using TMPro;
using System;

public class PlayerMovement : MonoBehaviour
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

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        moveToPos = transform.position;
        ChoiceDManager.OnDialogueStart += HandleDialogueStart;
        ChoiceDManager.OnDialogueEnd += HandleDialogueEnd;

        // Debug log to confirm Start is called
        Debug.Log("PlayerMovement Start called");
    }

    private void OnDestroy()
    {
        ChoiceDManager.OnDialogueStart -= HandleDialogueStart;
        ChoiceDManager.OnDialogueEnd -= HandleDialogueEnd;
    }

    private void HandleDialogueStart()
    {
        canMove = false;
        isMovingToClick = false;
        moveToPos = transform.position;  // Reset the move position to the current position
        UpdateAnimatorState(); // Call to update animator state when dialogue starts
    }

    private void HandleDialogueEnd()
    {
        canMove = true;
        UpdateAnimatorState(); // Call to update animator state when dialogue ends
    }

    void Update()
    {
        // Debug log to confirm Update is being called
        //Debug.Log("PlayerMovement Update called");

        bool cdmPlaying = ChoiceDManager.GetInstance() is not null && ChoiceDManager.GetInstance().dialogueIsPlaying;
        if (cdmPlaying || DialogueManager.isActive || QuestGiverScript.IsQuestWindowActive)
        {
            animator.SetFloat("PosX", 0);
            animator.SetFloat("PosY", 0);
            animator.SetBool("isWalking", false); // Set isWalking to false when not moving
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

        // Handle point-and-click movement
        if (Input.GetMouseButton(0) && canMove)
        {
            moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMovingToClick = true;
        }

        // Check if close enough to stop moving to the click position
        if (Vector2.Distance(transform.position, moveToPos) <= 0.1f)
        {
            isMovingToClick = false;
            moveToPos = transform.position;
        }

        // Set the horizontal and vertical input
        if (canMove)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horizontal = vertical = 0;
        }

        // Calculate movement direction
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

        // Update Animator parameters based on movement
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
        bool cdmPlaying = ChoiceDManager.GetInstance() is not null && ChoiceDManager.GetInstance().dialogueIsPlaying;
        if (cdmPlaying || DialogueManager.isActive || QuestGiverScript.IsQuestWindowActive)
        {
            body.velocity = Vector2.zero;
            return;
        }

        // For dashing
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
            // Point-and-click movement
            Vector2 position = transform.position;
            Vector2 moveDir = (moveToPos - position).normalized;
            body.velocity = moveDir * RunSpeed;
        }
        else
        {
            // Regular movement
            body.velocity = new Vector2(horizontal * RunSpeed, vertical * RunSpeed);
        }
    }

    // Helper method to update animator state based on movement/dialogue state
    private void UpdateAnimatorState()
    {
        if (canMove && !isMovingToClick && !DialogueManager.isActive && !QuestGiverScript.IsQuestWindowActive)
        {
            animator.SetBool("isWalking", (horizontal != 0 || vertical != 0));
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
