using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private bool shouldFaceMoveDir;

    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;

    [SerializeField]
    private Shooting shootingScript;

    private CharacterController controller;
    private PlayerAnimHandler playerAnimHandler;
    private Transform cameraTransform;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float speed;
    private bool isRunning;

    private ThirdPersonCameraController cameraController;
    [SerializeField]
    private float aimRotationSpeed = 15f;

    [SerializeField]
    private CinemachineCamera cam;
    [SerializeField]
    private Transform aimFocusPoint;

    private Transform originalFocusPoint;

    private bool isAiming;

    void Start()
    {
        isRunning = false;
        speed = moveSpeed;
        controller = GetComponent<CharacterController>();
        playerAnimHandler = GetComponent<PlayerAnimHandler>();

        originalFocusPoint = cam.Follow;

        cameraController = cam.GetComponent<ThirdPersonCameraController>();
        cameraTransform = cam.transform;

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            playerAnimHandler.AttackAnim();
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            ChangeMoveSpeed();
            playerAnimHandler.SetRunAnim(isRunning);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {

            isAiming = !isAiming;
            shouldFaceMoveDir = !isAiming;

            cam.Follow = isAiming ? aimFocusPoint : originalFocusPoint;
            cameraController.SetAim(isAiming);

            playerAnimHandler.SetAimAnim(isAiming);
        }
    }

    void Update()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
        controller.Move(moveDir * speed * Time.deltaTime);

        playerAnimHandler.SetPlayerMoveValue(moveDir.sqrMagnitude);

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
        else if (shouldFaceMoveDir && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        // deals with jumping
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ChangeMoveSpeed()
    {
        if (isRunning)
        {
            speed = moveSpeed;
            isRunning = false;
        }
        else
        {
            speed = runSpeed;
            isRunning = true;
        }
    }
}
