using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;
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
    private Vector2 moveInput;
    private Vector3 velocity;
    private float speed;
    private bool isRunning;

    void Start()
    {
        isRunning = false;
        speed = moveSpeed;
        controller = GetComponent<CharacterController>();
        playerAnimHandler = GetComponent<PlayerAnimHandler>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            shootingScript.Shoot();
            Debug.Log($"{Time.time}: Player Attacked {context.phase}");
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
            // Implement
            Debug.Log("Aiming");
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

        if (shouldFaceMoveDir && moveDir.sqrMagnitude > 0.001f)
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
            isRunning= true;
        }
    }
}
