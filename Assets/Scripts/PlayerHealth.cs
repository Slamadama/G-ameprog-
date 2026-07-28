using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;


    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log("Player HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        Debug.Log("Player died!");
    }
}