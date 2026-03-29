using UnityEngine;
using Zeke.TeamSystem;

public class HomingOrbProjectile : DamageProjectileBase
{
    [Header("Track settings")]
    [SerializeField] private float speedIncreaseRate;
    [SerializeField] private float speedCap;

    [Space]

    [SerializeField] private float startRotationSpeed;
    [SerializeField] private float rotationSpeedIncreaseRate;

    [Space]

    [SerializeField] private int maxHits = -1;

    public bool ColliderEnabled { get; set; } = true;

    private Vector3 lastTargetPosition;
    private Transform target;

    private float currentRotationSpeed = 0f;
    private float targetAngle = 0f;

    private int currentHits = 0;

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

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, int pierce, GameObject source, Teams team)
    {
        Launch(position, speed, direction, maxRange, damage, pierce, null, source, team);
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, int pierce, Transform target, GameObject source, Teams team)
    {
        maxHits = pierce;
        SetTarget(target);
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange)
    {
        currentHits = 0;
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

        if (hit.collider.gameObject == SourceUser) return;
        if (objectsNotExited.Contains(hit.collider.gameObject)) return;

        Hit(hit.transform.gameObject);
    }

    protected void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        currentHits += 1;

        if (maxHits >= 0 && currentHits >= maxHits)
        {
            Despawn();
        }

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        ApplyKnockback(receiver, Direction);
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