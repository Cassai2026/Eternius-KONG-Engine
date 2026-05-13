using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthAttackSystem : MonoBehaviour
{
    public int Health;
    public int NumOfHearts;

    public Image[] Hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    public Transform respawnPoint; // Reference to the respawn point

    public int attackDamage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    void Update()          // Attack system, attacks when clicking on space
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastAttackTime > attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyHealthScript enemyHealth = enemy.GetComponent<EnemyHealthScript>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            RespawnPlayer();
        }
        UpdateHeartsDisplay();
    }

    void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Health = 3; // Reset health to 3 filled hearts
        }
        else
        {
            Debug.LogError("Respawn point is not set!");
        }

        UpdateHeartsDisplay();
    }

    void UpdateHeartsDisplay()
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i < Health)
            {
                Hearts[i].sprite = FullHeart;
            }
            else
            {
                Hearts[i].sprite = EmptyHeart;
            }

            if (i < NumOfHearts)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }
        }
    }
}