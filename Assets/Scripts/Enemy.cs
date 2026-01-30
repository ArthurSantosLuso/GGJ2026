using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth  = 100f;
    public float moveSpeed  = 5f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
