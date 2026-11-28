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
        GlobalEventBus.Subscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoadEvent);
        GlobalEventBus.Subscribe<RunLevelHandler.LevelUnloadedEvent>(OnLevelUnloadEvent);

        GlobalEventBus.Subscribe<RunLevelHandler.LevelStartEvent>(OnLevelStartEvent);
        GlobalEventBus.Subscribe<RunLevelHandler.LevelEndEvent>(OnLevelEndEvent);
    }

    private void OnDisable()
    {
        GlobalEventBus.Unsubscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoadEvent);
        GlobalEventBus.Subscribe<RunLevelHandler.LevelUnloadedEvent>(OnLevelUnloadEvent);

        GlobalEventBus.Unsubscribe<RunLevelHandler.LevelStartEvent>(OnLevelStartEvent);
        GlobalEventBus.Unsubscribe<RunLevelHandler.LevelEndEvent>(OnLevelEndEvent);
    }

    private void OnLevelLoadEvent(RunLevelHandler.LevelLoadedEvent _) => onLevelLoaded?.Invoke();
    private void OnLevelUnloadEvent(RunLevelHandler.LevelUnloadedEvent _) => onLevelUnloaded?.Invoke();

    private void OnLevelStartEvent(RunLevelHandler.LevelStartEvent _) => onLevelStarted?.Invoke();
    private void OnLevelEndEvent(RunLevelHandler.LevelEndEvent _) => onLevelEnded?.Invoke();
}