using UnityEngine;

public abstract class BulletSandBox : MonoBehaviour
{
    private float speed = 20f;
    private float baseDamage = 10f;
    private float lifeTime = 5f;

    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
      ApplyDamage(collision);
      OnHit(collision);
      Destroy(gameObject);
    }

    protected abstract void ApplyDamage(Collision collision);
    protected virtual void OnHit(Collision collision) { }   
}
