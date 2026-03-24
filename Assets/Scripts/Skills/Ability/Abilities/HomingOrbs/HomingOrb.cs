using UnityEngine;

public class HomingOrb : Projectile
{
    [Header("Basic Projectile Settings")]
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 1f;
    [SerializeField] private float knockback = 1f;

    [Header("Track settings")]
    [SerializeField] private float speedIncreaseRate;
    [SerializeField] private float speedCap;

    [Space]

    [SerializeField] private float startRotationSpeed;
    [SerializeField] private float rotationSpeedIncreaseRate;

    [Header("Spinner Projectile Settings")]
    [SerializeField] private int maxHits = -1;

    private int currentHits = 0;
    private bool colliderEnabled = false;

    private Transform target;

    private float currentRotationSpeed = 0f;
    private float targetAngle = 0f;

    public override void OnPoolableGet()
    {
        base.OnPoolableGet();
        colliderEnabled = false;
        target = null;

        currentRotationSpeed = 0f;
        targetAngle = 0f;
    }

    public void EnableCollider()
    {
        colliderEnabled = true;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange, float damage, GameObject source)
    {
        currentHits = 0;
        currentRotationSpeed = startRotationSpeed;
    }

    protected override void Update()
    {
        if (target != null)
        {
            UpdateTrackingState(target);
        }

        base.Update();
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (!colliderEnabled) return;

        if (hit.collider.gameObject == SourceUser) return;
        if (objectsNotExited.Contains(hit.collider.gameObject)) return;

        OnImpact(hit);
    }

    protected void Hit(GameObject receiver)
    {
        bool damageRejected = false;

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, procCoefficient, armorPenetration), SourceUser, gameObject);

            damageRejected = damageEvent.damageRejected;
        }

        if (!damageRejected && knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, Direction);
        }
    }

    protected override void OnMaxDistanceReached()
    {
        Despawn();
    }

    private void OnImpact(RaycastHit2D collision)
    {
        HitIfEnemy(collision.collider.gameObject);
    }

    private void HitIfEnemy(GameObject receiver)
    {
        if (TeamManager.IsEnemy(SourceUser, receiver))
        {
            Hit(receiver);
            currentHits += 1;

            if (maxHits < 0) return;

            if (currentHits >= maxHits)
            {
                Despawn();
            }
        }
    }

    private void UpdateTrackingState(Transform target)
    {
        float accelerationStep = speedIncreaseRate * Time.deltaTime;

        Speed = Mathf.Clamp(Speed + accelerationStep, 0f, speedCap);
        currentRotationSpeed += rotationSpeedIncreaseRate * Time.deltaTime;

        targetAngle = GetRotation(target.position - transform.position);

        float rotationStep = currentRotationSpeed * Time.deltaTime;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationStep);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Direction = transform.up.normalized;
    }
}