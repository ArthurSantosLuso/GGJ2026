using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] 
    private float aimZoom = 3f;
    [SerializeField] 
    private float mouseSensitivity = 3f;

    [SerializeField]
    private float zoomSpeed = 2f;
    [SerializeField]
    private float zoomLerpSpeed = 10f;
    [SerializeField]
    private float minDistante = 3f;
    [SerializeField]
    private float maxDistante = 3f;

    private PlayerControls control;

    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    private bool isAiming;
    private float yaw;


    private void Start()
    {
        control = new PlayerControls();
        control.Enable();
        control.CameraControls.MouseZoom.performed += HandleMouseScroll;

        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Mouse is scrolling. Value: {scrollDelta}");
    }

    private void Update()
    {
        if (!isAiming)
        {
            if (scrollDelta.y != 0)
            {
                if (orbital != null)
                {
                    targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistante, maxDistante);
                    scrollDelta = Vector2.zero;
                }
            }
        }
        else
        {
            targetZoom = aimZoom;
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }

    public void SetAim(bool aiming)
    {
        isAiming = aiming;

        if (isAiming)
        {
            targetZoom = aimZoom;
            currentZoom = aimZoom;
            orbital.Radius = aimZoom;
        }
    }
}

