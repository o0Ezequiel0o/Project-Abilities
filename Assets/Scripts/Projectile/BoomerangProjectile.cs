using UnityEngine;
using Zeke.TeamSystem;

public class BoomerangProjectile : DamageProjectileBase
{
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
    private Vector3 lastSourcePosition = Vector3.zero;

    private float currentRotationSpeed = 0f;
    private float targetAngle = 0f;

    private float returnTimer = 0f;

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();

        state = BoomerangState.Normal;

        currentRotationSpeed = 0f;
        targetAngle = 0f;
        returnTimer = 0f;
    }

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange)
    {
        currentRotationSpeed = startRotationSpeed;
        if (SourceUser == null) lastSourcePosition = startPosition;
    }

    protected override void Update()
    {
        if (SourceUser != null)
        {
            lastSourcePosition = SourceUser.transform.position;
        }

        switch (state)
        {
            case BoomerangState.Slowing:
                UpdateSlowDownState();
                break;

            case BoomerangState.Returning:
                UpdateOwnerTrackingState(lastSourcePosition);
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

        Hit(hitObject);
    }

    protected override void OnMaxDistanceReached()
    {
        MaxRange = Mathf.Infinity;
        state = BoomerangState.Slowing;
    }

    private void Hit(GameObject receiver)
    {
        if (state == BoomerangState.Normal)
        {
            state = BoomerangState.Slowing;
        }

        if (TeamManager.IsAlly(Team, receiver)) return;

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        ApplyKnockback(receiver, Direction);
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

    private void UpdateOwnerTrackingState(Vector3 targetPos)
    {
        float accelerationStep = speedIncreaseRate * Time.deltaTime;

        Speed = Mathf.Clamp(Speed + accelerationStep, 0f, speedCap);
        currentRotationSpeed += rotationSpeedIncreaseRate * Time.deltaTime;

        targetAngle = GetRotation(targetPos - transform.position);

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