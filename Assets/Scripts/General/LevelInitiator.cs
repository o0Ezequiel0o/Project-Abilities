using UnityEngine;

public class LevelInitiator : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private CameraController cameraController;

    void Awake()
    {
        cameraController.SetTarget(player.transform);
    }
}