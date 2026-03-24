using UnityEngine;

public class EntityPhysics : Physics
{
    [SerializeField] private float moveForceDamping = 0.15f;

    public Vector2 MoveForces => moveForces;

    private Vector2 moveForces;

    public void AddMoveForce(Vector2 moveForces)
    {
        this.moveForces += moveForces;
    }

    protected override void UpdateForces()
    {
        base.UpdateForces();

        moveForces /= 1 + moveForceDamping;

        if (Mathf.Abs(moveForces.x) <= forceStopThreshold && Mathf.Abs(moveForces.y) <= forceStopThreshold)
        {
            moveForces = Vector2.zero;
        }
    }

    protected override void UpdateVelocity()
    {
        velocity = forces + moveForces;
        rigidBody.linearVelocity = velocity;
    }
}