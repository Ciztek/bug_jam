using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private Camera mainCamera;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        mainCamera.GetComponent<AudioPlayerManager>().PlaySteveDamage(); 
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        mainCamera.GetComponent<AudioPlayerManager>().PlayScorpion();
        Destroy(gameObject);
    }
}