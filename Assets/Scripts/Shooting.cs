using System.Collections.Generic;
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

    [Header("Vision Settings")]
    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private float viewDistance = 30f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstructionLayer;

    private float nextFireTime;

    [SerializeField] private Transform playerTransform;

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
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        Transform target = GetNearestVisibleEnemy();
        if (target == null)
            return;

        GameObject bulletPrefab = GetCurrentBulletPrefab();
        if (bulletPrefab == null)
            return;

        Vector3 direction =
            (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed;
    }


    public GameObject GetCurrentBulletPrefab()
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

    private Transform GetNearestVisibleEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            playerTransform.position,
            viewDistance,
            enemyLayer
        );

        Transform closest = null;
        float bestScore = -Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            Vector3 toEnemy =
                (enemy.transform.position - playerTransform.position);

            float distance = toEnemy.magnitude;
            Vector3 direction = toEnemy.normalized;

            float angle = Vector3.Angle(playerTransform.forward, direction);
            if (angle > viewAngle)
                continue;

            if (Physics.Raycast(
                    playerTransform.position + Vector3.up * 1.5f,
                    direction,
                    distance,
                    obstructionLayer))
                continue;

            float dot = Vector3.Dot(playerTransform.forward, direction);

            if (dot > bestScore)
            {
                bestScore = dot;
                closest = enemy.transform;
            }
        }

        return closest;
    }



    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, viewDistance);

        Vector3 left =
            Quaternion.Euler(0, -viewAngle, 0) * playerTransform.forward;
        Vector3 right =
            Quaternion.Euler(0, viewAngle, 0) * playerTransform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerTransform.position, left * viewDistance);
        Gizmos.DrawRay(playerTransform.position, right * viewDistance);
    }

}
