using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour
{
    private void Awake()
    {
        PlayerCameraController cameraController = FindAnyObjectByType<PlayerCameraController>();

        if (cameraController != null)
        {
            cameraController.SetTarget(transform);
        }
    }
}