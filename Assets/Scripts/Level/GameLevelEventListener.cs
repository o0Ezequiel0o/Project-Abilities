using UnityEngine.Events;
using UnityEngine;

public class GameLevelEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent onLevelLoaded;
    [SerializeField] private UnityEvent onLevelUnloaded;

    [Space]

    [SerializeField] private UnityEvent onLevelStarted;
    [SerializeField] private UnityEvent onLevelEnded;

    private void Awake()
    {
        GlobalEventBus.Subscribe<GameLevelHandler.LevelLoadedEvent>(_ => onLevelLoaded?.Invoke());
        GlobalEventBus.Subscribe<GameLevelHandler.LevelUnloadedEvent>(_ => onLevelUnloaded?.Invoke());

        GlobalEventBus.Subscribe<GameLevelHandler.LevelStartEvent>(_ => onLevelStarted?.Invoke());
        GlobalEventBus.Subscribe<GameLevelHandler.LevelEndEvent>(_ => onLevelEnded?.Invoke());
    }
}