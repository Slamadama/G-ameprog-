using UnityEngine;

public class SmoothMount : MonoBehaviour
{
    public Transform currentMount;
    public float speedFactor = 0.1f;

    void Update()
    {
        if (currentMount == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            currentMount.position,
            speedFactor);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            currentMount.rotation,
            speedFactor);
    }

    public void SetMount(Transform newMount)
    {
        currentMount = newMount;
    }
}