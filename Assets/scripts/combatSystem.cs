using UnityEngine;
using UnityEngine.InputSystem;

public class combatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    [Tooltip("Damage dealt to enemies per attack")]
    public int attackDamage = 25;
    
    [Tooltip("Range within which enemies can be hit")]
    public float attackRange = 3f;
    
    [Tooltip("Time between attacks (seconds)")]
    public float attackCooldown = 0.5f;
    
    [Header("Detection Settings")]
    [Tooltip("Layer mask for detecting enemies")]
    public LayerMask enemyLayer;
    
    [Header("Visual Feedback")]
    [Tooltip("Show attack range in editor")]
    public bool showAttackRange = true;

    // Private variables
    private float nextAttackTime = 0f;
    private Animator animator;
    private bool attackInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // If no layer mask is set, try to detect all colliders
        if (enemyLayer == 0)
        {
            enemyLayer = ~0; // All layers
        }
    }

    // Input callback for attack action
    void OnAttack(InputValue value)
    {
        attackInput = value.isPressed;
    }

    void Update()
    {
        // Check if player wants to attack and cooldown is ready
        if (attackInput && Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void PerformAttack()
    {
        // Play attack animation if animator exists
        if (animator != null)
        {
           // animator.SetTrigger("Attack");
        }

        // Find all colliders within attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        // Deal damage to all enemies in range
        foreach (Collider collider in hitColliders)
        {
            ennemy enemy = collider.GetComponent<ennemy>();
            
            if (enemy != null)
            {
                // Deal damage to the enemy
                enemy.health -= attackDamage;
                
                Debug.Log($"Hit {collider.gameObject.name} for {attackDamage} damage! Remaining health: {enemy.health}");
                
                // Destroy enemy if health reaches 0
                if (enemy.health <= 0)
                {
                    Destroy(collider.gameObject);
                    Debug.Log($"{collider.gameObject.name} has been defeated!");
                }
            }
        }

        // Debug feedback
        if (hitColliders.Length > 0)
        {
            Debug.Log($"Attack hit {hitColliders.Length} enemy(ies)!");
        }
        else
        {
            Debug.Log("Attack missed - no enemies in range!");
        }
    }

    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        if (showAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
