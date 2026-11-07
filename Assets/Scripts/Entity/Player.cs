using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }

    private void Awake()
    {
        Run.Instance.PlayerSpawned(this);
    }

    private void OnEnable()
    {
        Run.Instance.AddPlayer(this);
    }

    private void OnDisable()
    {
        Run.Instance.RemovePlayer(this);
    }
}