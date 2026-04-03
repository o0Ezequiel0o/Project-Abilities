using UnityEngine;
using System;

public class Missile : BasicProjectile
{
    [Header("Missile Settings")]
    [SerializeField] protected float turnSpeed;

    [Space]

    [SerializeField] protected LayerMask seekTargetLayer;
    [SerializeField] protected float findTargetRange;

    private Transform target;

    private Predicate<GameObject> filter;

    public override Vector3 Direction
    {
        get
        {
            return transform.up;
        }

        protected set
        {
            transform.rotation = GetRotationQuaternion(value);
        }
    }

    protected void Awake()
    {
        filter = target => TargetAwareness.HasLineOfSight(transform.position, target.transform, blockLayer);
    }

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        target = null;
    }

    private void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override void Update()
    {
        UpdateMissileSeeking();
        base.Update();
    }

    protected override void UpdateMovement()
    {
        UpdateMissileSteering();
        base.UpdateMovement();
    }

    private void UpdateMissileSeeking()
    {
        if (target != null) return;
        SetTarget(TargetAwareness.GetClosestTargetToDirection(TipPosition, Direction, findTargetRange, seekTargetLayer, blockLayer, filter));
    }

    private void UpdateMissileSteering()
    {
        if (target == null) return;

        float angularStep = turnSpeed * Time.deltaTime;
        float targetAngle = GetRotation((target.position - transform.position).normalized);

        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, angularStep);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
