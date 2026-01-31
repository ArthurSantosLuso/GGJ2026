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
    [SerializeField] private Transform cameraTransform;


    private float nextFireTime;

    public GameObject Player;

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

        Vector3 direction = (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;

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

    Transform GetNearestVisibleEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            viewDistance,
            enemyLayer
        );

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy =
                (enemy.transform.position - cameraTransform.position).normalized;

            
            float angle = Vector3.Angle(cameraTransform.forward, directionToEnemy);
            if (angle > viewAngle)
                continue;

            float distance = Vector3.Distance(
                cameraTransform.position,
                enemy.transform.position
            );

            
            if (Physics.Raycast(
                    cameraTransform.position,
                    directionToEnemy,
                    distance,
                    obstructionLayer))
                continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }

        return closest;
    }


    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftLimit =
            Quaternion.Euler(0, -viewAngle, 0) * cameraTransform.forward;
        Vector3 rightLimit =
            Quaternion.Euler(0, viewAngle, 0) * cameraTransform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.position, leftLimit * viewDistance);
        Gizmos.DrawRay(cameraTransform.position, rightLimit * viewDistance);
    }

}
