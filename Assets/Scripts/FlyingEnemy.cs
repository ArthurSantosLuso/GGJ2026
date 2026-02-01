using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class FlyingEnemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image bar;
    private float currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackDistance = 10f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float projectileSpeed = 12f;

    private Transform player;
    private float nextFireTime;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false; // Important for MovePosition
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentHealth = maxHealth;
        SetBarAmount();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            FollowPlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Move the enemy towards the player using Rigidbody
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Make the enemy look at the player
        transform.LookAt(player);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody projRb = projectile.GetComponent<Rigidbody>();

        if (projRb != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            projRb.linearVelocity = direction * projectileSpeed;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        SetBarAmount();

        if (currentHealth <= 0f)
            Die();
    }

    private void SetBarAmount()
    {
        if (bar != null)
            bar.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
