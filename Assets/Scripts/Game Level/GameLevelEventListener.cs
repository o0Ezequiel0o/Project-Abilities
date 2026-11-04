using UnityEngine.Events;
using UnityEngine;
using Zeke.GlobalEventsBus;

public class GameLevelEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent onLevelLoaded;
    [SerializeField] private UnityEvent onLevelUnloaded;

    [Space]

    [SerializeField] private UnityEvent onLevelStarted;
    [SerializeField] private UnityEvent onLevelEnded;

    private void OnEnable()
    {
        GlobalEventBus.Subscribe<GameLevelHandler.LevelLoadedEvent>(OnLevelLoadEvent);
        GlobalEventBus.Subscribe<GameLevelHandler.LevelUnloadedEvent>(OnLevelUnloadEvent);

        GlobalEventBus.Subscribe<GameLevelHandler.LevelStartEvent>(OnLevelStartEvent);
        GlobalEventBus.Subscribe<GameLevelHandler.LevelEndEvent>(OnLevelEndEvent);
    }

    private void OnDisable()
    {
        GlobalEventBus.Unsubscribe<GameLevelHandler.LevelLoadedEvent>(OnLevelLoadEvent);
        GlobalEventBus.Subscribe<GameLevelHandler.LevelUnloadedEvent>(OnLevelUnloadEvent);

        GlobalEventBus.Unsubscribe<GameLevelHandler.LevelStartEvent>(OnLevelStartEvent);
        GlobalEventBus.Unsubscribe<GameLevelHandler.LevelEndEvent>(OnLevelEndEvent);
    }

    private void OnLevelLoadEvent(GameLevelHandler.LevelLoadedEvent _) => onLevelLoaded?.Invoke();
    private void OnLevelUnloadEvent(GameLevelHandler.LevelUnloadedEvent _) => onLevelUnloaded?.Invoke();

    private void OnLevelStartEvent(GameLevelHandler.LevelStartEvent _) => onLevelStarted?.Invoke();
    private void OnLevelEndEvent(GameLevelHandler.LevelEndEvent _) => onLevelEnded?.Invoke();
}