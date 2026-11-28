using UnityEngine;

public class EntityAimPivot : EntityAim
{
    [Header("Dependency")]
    [SerializeField] private Transform pivot;

    public override Vector2 AimDirection => pivot.up;
    
    private void Reset()
    {
        pivot = GetComponentInChildren<Transform>();
    }

    protected override void UpdateRotationInternal(float desiredRotation, float step)
    {
        pivot.rotation = Quaternion.Euler(0f, 0f, Mathf.MoveTowardsAngle(pivot.eulerAngles.z, desiredRotation, step));
    }
}