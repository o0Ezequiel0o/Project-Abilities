using UnityEngine;

public abstract class EntityAim : MonoBehaviour, IUpgradable
{
    [Header("Settings")]
    [field: SerializeField] public Stat RotationSpeed { get; private set; }

    public abstract Vector2 AimDirection { get; }

    private Vector2 desiredAimDirection = Vector2.up;

    public void AimTowards(Vector2 desiredDirection)
    {
        desiredAimDirection = desiredDirection;
    }

    public void StopAiming()
    {
        desiredAimDirection = AimDirection;
    }

    public virtual void Upgrade()
    {
        RotationSpeed.Upgrade();
    }

    private void FixedUpdate()
    {
        float desiredRotation = (Mathf.Atan2(desiredAimDirection.y, desiredAimDirection.x) * Mathf.Rad2Deg) - 90f;
        UpdateRotationInternal(desiredRotation, RotationSpeed.Value * Time.fixedDeltaTime);
    }

    protected abstract void UpdateRotationInternal(float desiredRotation, float step);
}