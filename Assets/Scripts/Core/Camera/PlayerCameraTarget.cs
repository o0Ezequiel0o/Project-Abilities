using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour
{
    private void Awake()
    {
        PlayerCameraController cameraController = FindFirstObjectByType<PlayerCameraController>();

        if (cameraController != null )
        {
            cameraController.SetTarget(transform);
        }
    }
}