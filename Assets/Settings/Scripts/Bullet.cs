using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float damage;

    [SerializeField]
    private bool fire;
    [SerializeField]
    private bool poison;
    [SerializeField]
    private bool slow;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        StatusEffectManager effects = FindAnyObjectByType<StatusEffectManager>();

        enemy.TakeDamage(damage);

        if (fire)
            effects.ApplyFire(enemy, 5f, 3f);

        if (poison)
            effects.ApplyPoison(enemy, 2f, 5f);

        if (slow)
            effects.ApplySlow(enemy, 0.5f, 2f);

        Destroy(gameObject);
    }
}