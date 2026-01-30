using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet_Prefab;
    [SerializeField] private Transform ponta;
    [SerializeField] private float speed;

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        GameObject projectile = Instantiate(bullet_Prefab, ponta.position, ponta.rotation);

        projectile.GetComponent<Rigidbody>().linearVelocity = ponta.forward * speed;
    }
}
