using UnityEngine;

public class EntityMoveSnap : EntityMove
{
    public override Vector2 MoveDirection => moveDirection;
    private Vector2 moveDirection = Vector2.zero;

    [SerializeField] private float accelerationRatio = 1f;
    [SerializeField] private float brakingRatio = 2f;

    protected override void UpdateMovementInternal(Vector2 desiredMoveDirection)
    {
        physics.AddMoveForce(GetForceForDesiredMoveSpeed(MoveSpeed.Value * desiredMoveDirection));
        moveDirection = desiredMoveDirection;
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