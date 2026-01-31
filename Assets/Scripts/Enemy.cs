using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth  = 100f;
    public float moveSpeed  = 5f;

    [SerializeField] private Image bar;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        SetBarAmount();
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Update()
    {
    }

    private void SetBarAmount()
    {
        bar.fillAmount = currentHealth;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
