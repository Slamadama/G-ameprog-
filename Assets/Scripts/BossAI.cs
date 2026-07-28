using UnityEngine;

public class BossAI : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 2f;

    private Transform player;
    private float nextAttackTime;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }


    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face the player
        transform.LookAt(player);
    }


    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            Debug.Log("Boss attacked!");

            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
}