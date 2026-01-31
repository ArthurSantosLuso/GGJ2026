using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform firePoint;

    [Header("Bullet Prefabs")]
    [SerializeField]
    private GameObject fireBullet;
    [SerializeField]
    private GameObject poisonBullet;
    [SerializeField]
    private GameObject slowBullet;

    [Header("Shooting Settings")]
    [SerializeField]
    private float bulletSpeed = 40f;
    [SerializeField]
    private float fireRate = 0.2f;

    private float nextFireTime;

    private enum BulletType
    {
        Fire,
        Poison,
        Slow
    }

    [SerializeField]
    private BulletType currentBullet = BulletType.Fire;

    public void Shoot()
    {
        GameObject bulletPrefab = GetCurrentBulletPrefab();
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;
    }

    GameObject GetCurrentBulletPrefab()
    {
        switch (currentBullet)
        {
            case BulletType.Fire:
                return fireBullet;
            case BulletType.Poison:
                return poisonBullet;
            case BulletType.Slow:
                return slowBullet;
        }

        return null;
    }
}
