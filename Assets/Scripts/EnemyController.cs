using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        Debug.Log("Killed " + gameObject.name);

        // Notify the QuestManager that a fish was collected (if a quest is active)
        if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
        {
            QuestManager.Instance.FishCollected();
        }

        Destroy(gameObject);
    }

    public float GetHealth()
    {
        return currentHealth;
    }
}
