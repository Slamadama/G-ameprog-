using UnityEngine;

// Controls fish movement: swims around randomly, flees when player gets close.
public class FishAI : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 2f;          // normal swim speed when wandering
    public float turnSpeed = 2f;          // how fast the fish rotates toward its target direction
    public float fleeSpeedMultiplier = 2f; // multiplies swimSpeed when fleeing from player

    [Header("Detection")]
    public float detectionRange = 10f;    // distance at which fish detects and flees from player

    [Header("Wandering")]
    public float directionChangeTime = 3f; // seconds between picking a new random direction

    private Vector3 targetDirection;  // direction the fish is trying to face/move toward
    private float timer;              // counts up, resets when it's time to change direction
    private Transform player;         // reference to the player's transform, found by tag

    void Start()
    {
        ChooseNewDirection();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        // Decide whether to flee or wander
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            FleeFromPlayer();
        }
        else
        {
            Wander();
        }

        // Smoothly rotate toward the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        // Move forward at the appropriate speed
        float currentSpeed = (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
            ? swimSpeed * fleeSpeedMultiplier
            : swimSpeed;

        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    // Slowly wanders by picking a new random direction every few seconds
    void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= directionChangeTime)
        {
            ChooseNewDirection();
            timer = 0f;
        }
    }

    // Sets movement direction away from player (with a slight bias to stay level, not just fly up)
    void FleeFromPlayer()
    {
        Vector3 awayDirection = transform.position - player.position;
        awayDirection.y *= 0.3f; // reduce vertical component so fish mostly swims horizontally away
        awayDirection.Normalize();
        targetDirection = awayDirection;
        timer = 0f;
    }

    // Picks a random direction, biasing horizontal movement (y is scaled down)
    void ChooseNewDirection()
    {
        targetDirection = Random.onUnitSphere;
        targetDirection.y *= 0.3f; // reduce vertical randomness to keep fish mostly horizontal
        targetDirection.Normalize();
    }

    // Draws the detection range sphere in the Scene view (yellow wireframe)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
