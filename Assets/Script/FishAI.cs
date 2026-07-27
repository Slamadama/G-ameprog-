using UnityEngine;

public class FishAI : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 2f;
    public float turnSpeed = 2f;

    [Header("Wandering")]
    public float directionChangeTime = 3f;

    private Vector3 targetDirection;
    private float timer;

    void Start()
    {
        ChooseNewDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= directionChangeTime)
        {
            ChooseNewDirection();
            timer = 0f;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position += transform.forward * swimSpeed * Time.deltaTime;
    }

    void ChooseNewDirection()
    {
        targetDirection = Random.onUnitSphere;

        // Don't let the fish swim straight up or down
        targetDirection.y *= 0.3f;

        targetDirection.Normalize();
    }
}