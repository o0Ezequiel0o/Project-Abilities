using UnityEngine;

public class EntityMove : MonoBehaviour, IUpgradable
{
    [Header("Dependency")]
    [SerializeField] protected EntityPhysics physics;

    [Header("Settings")]
    [field: SerializeField] public Stat MoveSpeed { get; private set; }
    [SerializeField] private float accelerationRatio = 3f;
    [SerializeField] private float brakingRatio = 2f;

    public Vector2 MoveDirection => moveDirection;
    public Vector2 Velocity => physics.Velocity;

    private Vector2 desiredMoveDirection = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;

    public void MoveTowards(Vector2 desiredDirection)
    {
        desiredMoveDirection = desiredDirection.normalized;
    }

    public void StopMoving()
    {
        desiredMoveDirection = Vector2.zero;
    }

    public virtual void FixedUpdate()
    {
        physics.AddMoveForce(GetForceForDesiredMoveSpeed(MoveSpeed.Value * desiredMoveDirection));
        moveDirection = desiredMoveDirection;
    }

    public virtual void Upgrade()
    {
        MoveSpeed.Upgrade();
    }

    protected virtual void Reset()
    {
        physics = GetComponentInChildren<EntityPhysics>();
    }

    private Vector2 GetForceForDesiredMoveSpeed(Vector2 desiredMoveSpeed)
    {
        Vector2 forceToReachDesiredSpeed = Vector2.zero;

        forceToReachDesiredSpeed.x = GetForceForDesiredMoveSpeed(desiredMoveSpeed.x, physics.MoveForces.x);
        forceToReachDesiredSpeed.y = GetForceForDesiredMoveSpeed(desiredMoveSpeed.y, physics.MoveForces.y);

        return forceToReachDesiredSpeed;
    }

    private float GetForceForDesiredMoveSpeed(float desiredForcesAxis, float currentForcesAxis)
    {
        if (desiredForcesAxis != 0f)
        {
            float maxForceRequired = Mathf.Sign(desiredForcesAxis) == 1
                ? Mathf.Max(0, desiredForcesAxis - currentForcesAxis)
                : Mathf.Min(0, desiredForcesAxis - currentForcesAxis);

            float accelerationStep = MoveSpeed.Value * accelerationRatio * Time.deltaTime;
            return MoveTowards(maxForceRequired, accelerationStep);
        }
        else
        {
            float brakingStep = MoveSpeed.Value * brakingRatio * Time.deltaTime;

            float maxForceRequired = -currentForcesAxis;
            return MoveTowards(maxForceRequired, brakingStep);
        }
    }

    private float MoveTowards(float target, float step)
    {
        float stepRequired = Mathf.Sign(target) == 1
            ? Mathf.Min(target, step)
            : Mathf.Max(target, -step);

        return stepRequired;
    }
}