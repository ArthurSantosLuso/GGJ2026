using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float rotationSpeed = 10f;

    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationForce = 3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("UI")]
    [SerializeField] private Image bar;

    [Header("Attack")]
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float damageCooldown = 1f;


    private float lastDamageTime;
    private float currentHealth;
    private CharacterController controller;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        // Garante que o alvo seja o player da cena
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void Update()
    {
        if (target == null)
            return;

        Vector3 moveDirection = CalculateMovementDirection();

        // Movimento horizontal
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Gravidade
        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity.y = -2f;

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);

        // Rotação suave para a direção do movimento
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 CalculateMovementDirection()
    {
        // Direção para o player (plano XZ)
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0;
        toPlayer.Normalize();

        // Separação entre inimigos
        Vector3 separation = CalculateSeparation();

        // Combinação final
        Vector3 finalDirection = toPlayer + separation;
        finalDirection.y = 0;

        if (finalDirection.sqrMagnitude > 0.001f)
            finalDirection.Normalize();

        return finalDirection;
    }

    private Vector3 CalculateSeparation()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(
            transform.position,
            separationRadius,
            enemyLayer
        );

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider col in nearbyEnemies)
        {
            if (col.transform == transform)
                continue;

            Vector3 away = transform.position - col.transform.position;
            float distance = away.magnitude;

            if (distance > 0)
            {
                separation += away.normalized / distance;
                count++;
            }
        }

        if (count == 0)
            return Vector3.zero;

        separation /= count;
        return separation.normalized * separationForce;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (bar != null)
            bar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Player"))
            return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        PlayerController player = hit.collider.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.TakeDamage(contactDamage);
        lastDamageTime = Time.time;
    }
}

