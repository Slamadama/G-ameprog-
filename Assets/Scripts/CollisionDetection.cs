using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;
    // public GameObject HitParticle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Fish" && wc.IsAttacking)
        {
            Debug.Log(other.name);
            Object.Destroy(other.gameObject);
            // other.GetComponent<Animator>().SetTrigger("Hit");
            // Instantiate(HitParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.postiion.z), other.transform.rotation);
        }
    }
}
