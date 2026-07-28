using UnityEngine;

// CollisionDetection - Attached to the spear's hitbox collider.
// When the spear (while attacking) touches a fish, it destroys the fish
// and reports the kill to the QuestManager for quest progress tracking.
public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "Fish" while the spear attack animation is active
        if (other.tag == "Fish" && wc.IsAttacking)
        {
            Debug.Log("[CollisionDetection] Hit: " + other.name);

            // Notify the QuestManager that a fish was collected (if a quest is active)
            if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
            {
                QuestManager.Instance.FishCollected();
            }

            // Destroy the fish GameObject, removing it from the scene
            Object.Destroy(other.gameObject);
        }
    }
}
