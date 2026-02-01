using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool shouldFaceMoveDir = true;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat")]
    [SerializeField] private Shooting shootingScript;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Aim")]
    [SerializeField] private float aimRotationSpeed = 15f;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform aimFocusPoint;

    private CharacterController controller;
    private PlayerAnimHandler playerAnimHandler;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector3 velocity;
    private Vector3 lastMoveDirection;

    private float speed;
    private bool isRunning;
    private bool isAiming;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerAnimHandler = GetComponent<PlayerAnimHandler>();

        currentHealth = maxHealth;
        speed = moveSpeed;
    }

    private void Start()
    {
        cameraTransform = cam.transform;
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (shootingScript == null || !shootingScript.isReloading)
                playerAnimHandler.AttackAnim();
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isRunning = !isRunning;
            speed = isRunning ? runSpeed : moveSpeed;
            playerAnimHandler.SetRunAnim(isRunning);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isAiming = !isAiming;
            shouldFaceMoveDir = !isAiming;
            playerAnimHandler.SetAimAnim(isAiming);
            cam.GetComponent<ThirdPersonCameraController>().SetAim(isAiming);
        }
    }

    #endregion

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;

        if (moveDir.sqrMagnitude > 0.001f)
            lastMoveDirection = moveDir.normalized;

        controller.Move(moveDir * speed * Time.deltaTime);
        playerAnimHandler.SetPlayerMoveValue(moveDir.sqrMagnitude);
    }

    private void HandleRotation()
    {
        if (isAiming)
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;

            if (camForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    aimRotationSpeed * Time.deltaTime
                );
            }
        }
        else if (shouldFaceMoveDir && lastMoveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(lastMoveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                toRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #region Damage System

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player tomou {amount} de dano. Vida atual: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player morreu");
        Destroy(gameObject);
    }

    #endregion
}
