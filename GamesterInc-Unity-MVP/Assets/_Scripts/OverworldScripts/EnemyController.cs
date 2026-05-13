using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float moveSpeed = 3f; // Speed at which the enemy moves towards the player

    private Rigidbody2D rb;
    public int followRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
{
    if (player != null)
    {
        // Calculate the distance between enemy and player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the distance is within the follow range
        if (distanceToPlayer < followRange)
        {
            // Calculate the direction towards the player
            Vector3 direction = (player.position - transform.position).normalized;

            // Move towards the player
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // If the player is out of range, stop moving
            rb.velocity = Vector2.zero;
        }
    }
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle damage to the player here
            Debug.Log("Player hit by enemy!");
            // For example, you can access the player's health script and decrease health
            // playerHealthScript.TakeDamage(damageAmount);
        }
    }
}