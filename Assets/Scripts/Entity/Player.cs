using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        GameInstance.PlayerSpawned(this);
    }

    private void OnEnable()
    {
        GameInstance.AddPlayer(this);
    }

    private void OnDisable()
    {
        GameInstance.RemovePlayer(this);
    }
}