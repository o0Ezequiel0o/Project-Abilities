using UnityEngine;

public class EntityAimPhysics : EntityAim
{
    [Header("Dependency")]
    [SerializeField] private Physics physics;

    public override Vector2 AimDirection => transform.up;
    
    private void Reset()
    {
        physics = GetComponentInChildren<Physics>();
    }

    protected override void UpdateRotationInternal(float desiredRotation, float step)
    {
        physics.Rotate(desiredRotation, step);
    }
}