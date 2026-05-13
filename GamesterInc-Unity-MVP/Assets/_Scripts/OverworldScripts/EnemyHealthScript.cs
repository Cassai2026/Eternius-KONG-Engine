using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthScript : MonoBehaviour
{
    public int Health;
    public int NumOfHearts;
    public Image[] Hearts; // If we ever want to display enemy health visually with hearts

    public int damageAmount = 1; // Amount of damage inflicted on the enemy

    // Call this method when the enemy takes damage
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die(); // Enemy dies when no more health
            FindObjectOfType<Player>().EnemyKilled(); // Call EnemyKilled method of the Player object
        }
        UpdateHeartsDisplay(); // Update health display if needed
    }

    // Update the hearts display based on current health
    void UpdateHeartsDisplay()
    {
        // Implement based on your visual representation of enemy health
    }

    // Optional: Implement Die() method if you want special behavior when the enemy dies
    void Die()
    {
        // Implement behavior for when the enemy dies (e.g., play death animation, drop items, etc.)
        Destroy(gameObject); // Destroy the enemy GameObject
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the HealthAttackSystem component of the player
            HealthAttackSystem playerHealth = collision.gameObject.GetComponent<HealthAttackSystem>();

            // If the player has a HealthAttackSystem component, apply damage
            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}