using UnityEngine;

public class ennemy : MonoBehaviour
{
    [Header("Stats de l'ennemi")]
    [Tooltip("Points de vie de l'ennemi")]
    public int health = 100;
    [Tooltip("Dégâts infligés par l'ennemi")]
    public int damage = 10;
    [Tooltip("Vitesse de déplacement de l'ennemi")]
    public float moveSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer < 200f)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private float damageTimer = 0f;
    private float damageCooldown = 1f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth is not null)
                {
                    playerHealth.TakeDamage(damage);
                    damageTimer = damageCooldown;
                }
            }
        }
    }
}
