using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }

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