using UnityEngine;

public class EntityMoveSnap : EntityMove
{
    public override Vector2 MoveDirection => moveDirection;
    private Vector2 moveDirection = Vector2.zero;

    protected override void UpdateMovementInternal(Vector2 desiredMoveDirection)
    {
        physics.AddMoveForce(GetForceForDesiredMoveSpeed(MoveSpeed.Value * desiredMoveDirection.normalized));
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
        if (currentForcesAxis == 0f || desiredForcesAxis == 0f) return desiredForcesAxis;
        if (Mathf.Sign(currentForcesAxis) != Mathf.Sign(desiredForcesAxis)) return desiredForcesAxis;

        if (Mathf.Sign(desiredForcesAxis) == 1)
        {
            return Mathf.Max(0, desiredForcesAxis - currentForcesAxis);
        }
        else
        {
            return Mathf.Min(0, desiredForcesAxis - currentForcesAxis);
        }
    }
}