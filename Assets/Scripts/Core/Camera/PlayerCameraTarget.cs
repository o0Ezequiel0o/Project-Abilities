using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraTarget : MonoBehaviour
{
    [SerializeField] private InputActionReference zoomInput;
    [SerializeField] private float zoomAmount = 0.1f;

    private PlayerCameraController cameraController;

    private void Awake()
    {
        cameraController = FindAnyObjectByType<PlayerCameraController>();

        if (cameraController != null)
        {
            cameraController.SetTarget(transform);
        }
    }

    private void OnEnable()
    {
        if (cameraController != null)
        {
            zoomInput.action.performed += OnZoomInput;
        }
    }

    private void OnDisable()
    {
        if (cameraController != null)
        {
            zoomInput.action.performed -= OnZoomInput;
        }
    }

    private void OnZoomInput(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        cameraController.Zoom += -value.y * zoomAmount;
    }
}