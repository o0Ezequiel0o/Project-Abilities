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

    public virtual void Upgrade(int levels)
    {
        MoveSpeed.Upgrade(levels);
    }

    protected virtual void Reset()
    {
        physics = GetComponentInChildren<EntityPhysics>();
    }

    private Vector2 GetForceForDesiredMoveSpeed(Vector2 desiredMoveSpeed)
    {
        Vector2 forceToReachDesiredSpeed = Vector2.zero;

        forceToReachDesiredSpeed.x = GetForceForDesiredMoveSpeed(desiredMoveSpeed.x, physics.MoveForces.x, physics.MovingDirection.x);
        forceToReachDesiredSpeed.y = GetForceForDesiredMoveSpeed(desiredMoveSpeed.y, physics.MoveForces.y, physics.MovingDirection.y);

        return forceToReachDesiredSpeed;
    }

    private float GetForceForDesiredMoveSpeed(float desiredForcesAxis, float currentForcesAxis, float currentDirection)
    {
        if (desiredForcesAxis != 0f)
        {
            float maxForceRequired = desiredForcesAxis - currentForcesAxis;       
            float accelerationStep = MoveSpeed.Value * accelerationRatio * Time.deltaTime;
            accelerationStep += GetOverflowSpeedStep(currentForcesAxis, currentDirection);

            return Mathf.MoveTowards(0f, maxForceRequired, accelerationStep);
        }
        else
        {
            float brakingStep = MoveSpeed.Value * brakingRatio * Time.deltaTime;
            brakingStep += GetOverflowSpeedStep(currentForcesAxis, currentDirection);

            float maxForceRequired = -currentForcesAxis;
            return Mathf.MoveTowards(0f, maxForceRequired, brakingStep);
        }
    }

    private float GetOverflowSpeedStep(float currentForce, float currentDirection)
    {
        float step = 0f;

        if ((currentForce < 0 && MoveSpeed.Value * currentDirection > currentForce) ||
            currentForce > 0 && MoveSpeed.Value * currentDirection < currentForce)
        {
            step = Mathf.Abs(currentForce - (MoveSpeed.Value * currentDirection));
        }

        return step;
    }
}