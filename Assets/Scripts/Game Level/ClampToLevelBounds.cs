using UnityEngine;
using Zeke.GlobalEventsBus;

[DefaultExecutionOrder(100), RequireComponent(typeof(Rigidbody2D))]
public class ClampToLevelBounds : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;

    private GameLevelInstance levelInstance;

    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        GlobalEventBus.Subscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoaded);
    }

    private void Start()
    {
        if (levelInstance == null)
        {
            levelInstance = RunLevelHandler.Instance.CurrentLevelInstance;
        }
    }

    private void OnDestroy()
    {
        GlobalEventBus.Unsubscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoaded);
    }

    private void FixedUpdate()
    {
        if (levelInstance == null || levelInstance.BoundsCollider == null) return;

        Vector2 position = rigidBody.position;
        Bounds bounds = levelInstance.BoundsCollider.bounds;

        float clampedX = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);

        bool isAtXBoundary = IsAtBoundary(position.x, rigidBody.linearVelocity.x, bounds.min.x, bounds.max.x);
        bool isAtYBoundary = IsAtBoundary(position.y, rigidBody.linearVelocity.y, bounds.min.y, bounds.max.y);

        if (position.x != clampedX || position.y != clampedY)
        {
            rigidBody.MovePosition(new Vector2(clampedX, clampedY));
        }

        Vector2 linearVelocity = rigidBody.linearVelocity;

        if (isAtXBoundary) linearVelocity.x = 0;
        if (isAtYBoundary) linearVelocity.y = 0;

        rigidBody.linearVelocity = linearVelocity;
    }

    private bool IsAtBoundary(float position, float velocity, float boundsMin, float boundsMax)
    {
        return (position <= boundsMin && velocity < 0) || (position >= boundsMax && velocity > 0);
    }

    private void OnLevelLoaded(RunLevelHandler.LevelLoadedEvent @event)
    {
        levelInstance = @event.level;
    }
}