using UnityEngine;

public class BoomerangProjectile : Projectile
{
    [Header("Boomerang Settings")]
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 1f;
    [SerializeField] private float knockback = 1f;

    [Header("Return settings")]
    [SerializeField] private float returnDeceleration;
    [SerializeField] private float speedIncreaseRate;
    [SerializeField] private float speedCap;

    [Space]

    [SerializeField] private float startRotationSpeed;
    [SerializeField] private float rotationSpeedIncreaseRate;

    [Space]

    [SerializeField] private float maxReturnTime = 10f;

    private BoomerangState state = BoomerangState.Normal;

    private float currentRotationSpeed = 0f;
    private float targetAngle = 0f;

    private float returnTimer = 0f;

    public override void OnPoolableGet()
    {
        base.OnPoolableGet();

        state = BoomerangState.Normal;

        currentRotationSpeed = 0f;
        targetAngle = 0f;
        returnTimer = 0f;
    }

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange, float damage, GameObject source)
    {
        currentRotationSpeed = startRotationSpeed;
    }

    protected override void Update()
    {
        switch (state)
        {
            case BoomerangState.Slowing:
                UpdateSlowDownState();
                break;

            case BoomerangState.Returning:
                if (SourceUser != null)
                {
                    UpdateOwnerTrackingState(SourceUser.transform);
                }
                break;
        }

        base.Update();
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        if (hitObject == SourceUser)
        {
            if (state == BoomerangState.Returning)
            {
                StopLoopingHits();
                Despawn();
            }
            else
            {
                return;
            }
        }

        if (objectsNotExited.Contains(hitObject))
        {
            return;
        }

        OnImpact(hit);
    }

    protected override void OnMaxDistanceReached()
    {
        MaxRange = Mathf.Infinity;
        state = BoomerangState.Slowing;
    }

    protected virtual void OnHitInternal(GameObject receiver, bool damageRejected) { }

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

        OnHitInternal(receiver, damageRejected);
    }

    private void OnImpact(RaycastHit2D collision)
    {
        GameObject hitObject = collision.collider.gameObject;

        if (TeamManager.IsEnemy(SourceUser, hitObject))
        {
            Hit(hitObject);
        }

        if (state == BoomerangState.Normal)
        {
            state = BoomerangState.Slowing;
        }
    }

    private void UpdateSlowDownState()
    {
        float decelerationStep = returnDeceleration * Time.deltaTime;
        Speed = Mathf.Max(0f, Speed - decelerationStep);

        if (Speed <= 0f)
        {
            InvertDirectionAndRotation();
            state = BoomerangState.Returning;
        }
    }

    private void UpdateOwnerTrackingState(Transform target)
    {
        float accelerationStep = speedIncreaseRate * Time.deltaTime;

        Speed = Mathf.Clamp(Speed + accelerationStep, 0f, speedCap);
        currentRotationSpeed += rotationSpeedIncreaseRate * Time.deltaTime;

        targetAngle = GetRotation(target.position - transform.position);

        float rotationStep = currentRotationSpeed * Time.deltaTime;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationStep);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Direction = transform.up.normalized;

        returnTimer += Time.deltaTime;

        if (returnTimer > maxReturnTime)
        {
            Despawn();
        }
    }

    private void InvertDirectionAndRotation()
    {
        float angle = GetRotation(Direction * -1f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Direction = transform.up.normalized;
    }

    private enum BoomerangState
    {
        Normal,
        Slowing,
        Returning
    }
}