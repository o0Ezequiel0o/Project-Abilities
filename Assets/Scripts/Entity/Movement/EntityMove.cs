using UnityEngine;

public abstract class EntityMove : MonoBehaviour, IUpgradable
{
    [Header("Dependency")]
    [SerializeField] protected EntityPhysics physics;

    [Header("Settings")]
    [field: SerializeField] public Stat MoveSpeed { get; private set; }

    public abstract Vector2 MoveDirection { get; }
    public Vector2 Velocity => physics.Velocity;

    private Vector2 desiredMoveDirection = Vector2.zero;

    public void MoveTowards(Vector2 desiredDirection)
    {
        desiredMoveDirection = desiredDirection;
    }

    public void StopMoving()
    {
        desiredMoveDirection = Vector2.zero;
    }

    public virtual void FixedUpdate()
    {
        UpdateMovementInternal(desiredMoveDirection);
    }

    public virtual void Upgrade()
    {
        MoveSpeed.Upgrade();
    }

    protected virtual void Reset()
    {
        physics = GetComponentInChildren<EntityPhysics>();
    }

    protected abstract void UpdateMovementInternal(Vector2 desiredMoveDirection);
}