using UnityEngine;

// CollisionDetection - Attached to the spear's hitbox collider.
// When the spear (while attacking) touches a fish, it destroys the fish
// and reports the kill to the QuestManager for quest progress tracking.
public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        // Only react to objects tagged "Fish" while the spear attack animation is active
        //if (other.tag == "Fish" && wc.IsAttacking)
        if (damageable != null && wc.IsAttacking)
        {
            Debug.Log("[CollisionDetection] Hit: " + other.name);

            // Destroy the fish GameObject, removing it from the scene
            //Object.Destroy(other.gameObject);
            damageable.TakeDamage(5);
        }
    }
}
