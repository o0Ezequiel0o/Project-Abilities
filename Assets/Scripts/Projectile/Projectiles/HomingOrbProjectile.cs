using UnityEngine;
using Zeke.TeamSystem;

public class HomingOrbProjectile : PiercingProjectile
{
    [Header("Track settings")]
    [SerializeField] private float speedIncreaseRate;
    [SerializeField] private float speedCap;

    [Space]

    [SerializeField] private float startRotationSpeed;
    [SerializeField] private float rotationSpeedIncreaseRate;

    public bool ColliderEnabled { get; set; } = true;

    private Vector3 lastTargetPosition;
    private Transform target;

    private float currentRotationSpeed = 0f;
    private float targetAngle = 0f;

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        ColliderEnabled = true;
        target = null;

        currentRotationSpeed = 0f;
        targetAngle = 0f;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;

        if (target != null)
        {
            lastTargetPosition = target.transform.position;
        }
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, int pierce, Transform target, GameObject source, Teams team)
    {
        SetTarget(target);
        Launch(position, speed, direction, maxRange, damageData, knockback, pierce, source, team);
    }

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange)
    {
        base.OnLaunch(startPosition, speed, direction, maxRange);
        currentRotationSpeed = startRotationSpeed;
    }

    protected override void Update()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
            UpdateTrackingState(lastTargetPosition);
        }

        base.Update();
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (!ColliderEnabled) return;
        base.OnCollision(hit);
    }

    private void UpdateTrackingState(Vector3 targetPos)
    {
        float accelerationStep = speedIncreaseRate * Time.deltaTime;

        Speed = Mathf.Clamp(Speed + accelerationStep, 0f, speedCap);
        currentRotationSpeed += rotationSpeedIncreaseRate * Time.deltaTime;

        targetAngle = GetRotation(targetPos - transform.position);

        float rotationStep = currentRotationSpeed * Time.deltaTime;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationStep);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Direction = transform.up.normalized;
    }
}