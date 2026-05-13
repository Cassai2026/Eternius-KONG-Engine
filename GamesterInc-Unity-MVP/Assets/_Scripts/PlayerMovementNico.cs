using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementNico : MonoBehaviour
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

    private Vector2 movement;

    public ProgressBar progressBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;

    private int xp = 0;
    private int level = 0;
    private int xpToNextLevel = 100;

    private Animator animator;

    [NonSerialized] public Vector2 moveToPos;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        moveToPos = transform.position;
    }

    void Update()
    {
        if (DialogueManager.isActive == true)
            return;

        if (isCooldown)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0)
            {
                isCooldown = false;
            }
        }

        //// Set the horizontal and vertical input
        //horizontal = Input.GetAxisRaw("Horizontal");
        //vertical = Input.GetAxisRaw("Vertical");

        //// Check if moving left or right to flip sprite
        //if (horizontal > 0)
        //{
        //    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Face right
        //}
        //else if (horizontal < 0)
        //{
        //    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Face left
        //}

        // Set the Speed parameter for horizontal movement
        // animator.SetFloat("Speed", Mathf.Abs(horizontal));

        //// Check if moving down to trigger walking down animation
        //if (vertical < 0)
        //{
        //    // animator.SetFloat("VerticalSpeed", Mathf.Abs(vertical)); // Set the vertical speed parameter
        //}
        //else
        //{
        //    // animator.SetFloat("VerticalSpeed", 0); // Reset vertical speed if not moving down
        //}

        //if (Input.GetKeyDown(KeyCode.LeftShift) && !isCooldown)
        //{
        //    isDashing = true;
        //    currentDashTime = DashDuration;
        //    isCooldown = true;
        //    currentCooldownTime = DashCooldown;
        //}


        //POINT AND CLICK
        if (Input.GetMouseButton(0))
        {
            moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (moveToPos - (Vector2)transform.position).normalized;
            animator.SetFloat("X", direction.x);
            animator.SetFloat("Y", direction.y);
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
        else
        {
            body.velocity = new Vector2(horizontal * RunSpeed, vertical * RunSpeed);
        }


        //POINT N CLICK  
        if (Vector2.Distance(transform.position, moveToPos) > 0.1f && !DialogueManager.isActive)
        {
            body.velocity = new Vector2(moveToPos.x - transform.position.x, moveToPos.y - transform.position.y) * RunSpeed;
            //animator.SetFloat("X", movement.x);
            //animator.SetFloat("Y", movement.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            progressBar.current += 25;
            xp += 25;

            xpText.text = xp.ToString() + " /100";

            if (progressBar.current >= progressBar.maximum)
            {
                progressBar.current = 0;
                xp = 0;
                level++;
                levelText.text = "Level: " + level.ToString();
            }

            Destroy(other.gameObject);
        }
    }
}