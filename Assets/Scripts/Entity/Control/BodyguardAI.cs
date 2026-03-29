using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.Abilities;
using Zeke.TeamSystem;

public class BodyguardAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BodyguardAISettings settings;

    private BodyguardStateMachine stateMachine;
    private BodyguardStateContext context;

    public void SetProtectTarget(Transform target)
    {
        context.ProtectTarget = target;
    }

    private void Awake()
    {
        context = new BodyguardStateContext(settings);
        stateMachine = new BodyguardStateMachine(gameObject, context);

        stateMachine.ChangeState(stateMachine.idleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnDestroy()
    {
        stateMachine.Destroy();
    }
}

public class BodyguardStateContext
{
    public readonly BodyguardAISettings ai;

    private Transform protectTarget;
    private Transform engageTarget;

    public Transform ProtectTarget
    {
        get { return protectTarget; }
        set
        {
            protectTarget = value;

            if (value != null)
            {
                ProtectTargetCollider = value.GetComponent<Collider2D>();
            }
        }
    }

    public Transform EngageTarget
    {
        get { return engageTarget; }
        set
        {
            engageTarget = value;

            if (value != null)
            {
                EngageTargetCollider = value.GetComponent<Collider2D>();
            }
        }
    }

    public Collider2D ProtectTargetCollider { get; private set; }
    public Collider2D EngageTargetCollider { get; private set; }

    public BodyguardStateContext(BodyguardAISettings settings)
    {
        ai = settings;
    }
}

public class BodyguardStateMachine : StateMachine<BodyguardStateContext>
{
    public readonly BodyguardProtectState protectState;
    public readonly BodyguardAttackState attackState;
    public readonly BodyguardReturnState returnState;
    public readonly BodyguardFleeState fleeState;
    public readonly BodyguardIdleState idleState;

    private readonly BodyguardStateContext context;

    public BodyguardStateMachine(GameObject gameObject, BodyguardStateContext context)
    {
        this.context = context;

        protectState = new BodyguardProtectState(this, gameObject);
        attackState = new BodyguardAttackState(this, gameObject);
        returnState = new BodyguardReturnState(this, gameObject);
        fleeState = new BodyguardFleeState(this, gameObject);
        idleState = new BodyguardIdleState(this, gameObject);
    }

    public override void ChangeState(State<BodyguardStateContext> newState)
    {
        currentState?.ExitState(context);
        currentState = newState;
        currentState?.EnterState(context);
    }

    public override void Destroy()
    {
        protectState.DestroyState(context);
        attackState.DestroyState(context);
        returnState.DestroyState(context);
        fleeState.DestroyState(context);
        idleState.DestroyState(context);
    }

    public override void Update()
    {
        currentState?.UpdateState(context);
    }
}

public abstract class BodyguardBaseState : State<BodyguardStateContext>
{
    private static readonly List<Collider2D> hits = new List<Collider2D>(16);
    private static readonly List<Vector2> projectileDirections = new List<Vector2>(16);

    protected bool TargetsInFleetingRange(Vector2 position, float range, LayerMask targetLayers, Predicate<GameObject> filter) //or pass context?
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = targetLayers };
        Physics2D.OverlapCircle(position, range, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if ((Vector2)hits[i].transform.position == position) continue;
            if (filter(hits[i].gameObject)) return true;
        }

        return false;
    }

    protected bool TargetInRange(Vector2 position, Collider2D target, float range)
    {
        return TargetInRange(position, target.ClosestPoint(position), range);
    }

    protected bool TargetInRange(Vector2 position, Vector2 target, float range)
    {
        return (target - position).sqrMagnitude <= range * range;
    }

    protected Vector2 GetProjectileDodgeDirection(Vector3 position, ProjectileDetector projectileDetector, GameObject source)
    {
        projectileDirections.Clear();
        Vector2 evadeDirection = Vector2.zero;

        for (int i = 0; i < projectileDetector.Projectiles.Count; i++)
        {
            Projectile projectile = projectileDetector.Projectiles[i];

            if (projectile.TryGetComponent(out DamageProjectileBase damageProjectileBase))
            {
                if (damageProjectileBase.SourceUser == source) continue;
            }

            projectileDirections.Add((projectile.transform.position - position).normalized);
        }

        if (projectileDirections.Count == 0) return Vector2.zero;

        for (int i = 0; i < projectileDirections.Count; i++)
        {
            evadeDirection += projectileDirections[i];
        }

        return -(evadeDirection + Vector2.left).normalized;
    }

    protected bool ProtectTargetInRange(Vector2 position, BodyguardStateContext context)
    {
        Vector2 targetPosition = context.ProtectTarget.position;

        if (context.ProtectTargetCollider != null)
        {
            targetPosition = context.ProtectTargetCollider.ClosestPoint(position);
        }

        return (targetPosition - position).sqrMagnitude <= context.ai.MaxDistanceFromProtectTarget * context.ai.MaxDistanceFromProtectTarget;
    }

    protected bool ProtectTargetInMinFollowRange(Vector2 position, BodyguardStateContext context)
    {
        if (context.ProtectTargetCollider != null)
        {
            return TargetInRange(position, context.ProtectTargetCollider, context.ai.MinFollowDistance);
        }
        else
        {
            return TargetInRange(position, context.ProtectTarget.position, context.ai.MinFollowDistance);
        }
    }

    protected bool ProtectTargetOutOfRange(Vector3 position, BodyguardStateContext context)
    {
        if (context.ProtectTargetCollider != null)
        {
            return !TargetInRange(position, context.ProtectTargetCollider, context.ai.MaxDistanceFromProtectTarget);
        }
        else
        {
            return !TargetInRange(position, context.ProtectTarget.position, context.ai.MaxDistanceFromProtectTarget);
        }
    }
}

public class BodyguardProtectState : BodyguardBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly BodyguardStateMachine stateMachine;

    public BodyguardProtectState(BodyguardStateMachine stateMachine, GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(BodyguardStateContext context) { }

    public override void EnterState(BodyguardStateContext context) { }

    public override void ExitState(BodyguardStateContext context)
    {
        entityMove.StopMoving();
    }

    public override void UpdateState(BodyguardStateContext context)
    {
        if (TargetsInFleetingRange(transform.position, context.ai.FleetingRange, context.ai.TargetLayers, IsEnemy))
        {
            stateMachine.ChangeState(stateMachine.fleeState);
            return;
        }

        if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.EngageTarget = target;
            stateMachine.ChangeState(stateMachine.attackState);
            return;
        }

        if (context.ProtectTarget == null)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }

        bool targetInMinFollowRange = ProtectTargetInMinFollowRange(transform.position, context);

        if (!targetInMinFollowRange)
        {
            FollowProtectTarget(context);
        }
        else
        {
            entityMove.StopMoving();
        }
    }

    private void FollowProtectTarget(BodyguardStateContext context)
    {
        Vector2 direction = (context.ProtectTarget.position- transform.position).normalized;

        entityMove.MoveTowards(direction);
        entityAim.AimTowards(direction);
    }

    private bool IsEnemy(GameObject targetObject)
    {
        return TeamManager.IsEnemy(gameObject, targetObject);
    }
}

public class BodyguardAttackState : BodyguardBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly ProjectileDetector projectileDetector;

    private readonly BodyguardStateMachine stateMachine;

    public BodyguardAttackState(BodyguardStateMachine stateMachine, GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();
        projectileDetector = gameObject.GetComponentInChildren<ProjectileDetector>();
    }

    public override void DestroyState(BodyguardStateContext context) { }

    public override void EnterState(BodyguardStateContext context) { }

    public override void ExitState(BodyguardStateContext context) { }

    public override void UpdateState(BodyguardStateContext context)
    {
        if(TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.EngageTarget = target;
        }

        Vector2 direction = GetProjectileDodgeDirection(transform.position, projectileDetector, gameObject);

        entityMove.MoveTowards(direction);

        if (context.EngageTarget != null)
        {
            if (TargetAwareness.AnyTargetInLineOfSight(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, IsEnemy) && abilityController.CanUseAbility(context.ai.AttackType))
            {
                abilityController.TryUseAbility(context.ai.AttackType);
            }

            entityAim.AimTowards((target.position - transform.position).normalized);
        }

        if (context.ProtectTarget != null && ProtectTargetOutOfRange(transform.position, context))
        {
            stateMachine.ChangeState(stateMachine.returnState);
            return;
        }

        if (TargetsInFleetingRange(transform.position, context.ai.EngageRange, context.ai.TargetLayers, IsEnemy))
        {
            stateMachine.ChangeState(stateMachine.fleeState);
            return;
        }

        if (context.EngageTarget == null)
        {
            stateMachine.ChangeState(stateMachine.protectState);
            return;
        }
    }

    private bool IsEnemy(GameObject targetObject)
    {
        return TeamManager.IsEnemy(gameObject, targetObject);
    }
}

public class BodyguardFleeState : BodyguardBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;

    private readonly BodyguardStateMachine stateMachine;

    public BodyguardFleeState(BodyguardStateMachine stateMachine, GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();
    }

    public override void DestroyState(BodyguardStateContext context) { }

    public override void EnterState(BodyguardStateContext context) { }

    public override void ExitState(BodyguardStateContext context)
    {
        entityMove.StopMoving();
    }

    public override void UpdateState(BodyguardStateContext context)
    {
        if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.EngageTarget = target;
        }

        if (context.EngageTarget != null)
        {
            if (TargetAwareness.AnyTargetInLineOfSight(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, IsEnemy) && abilityController.CanUseAbility(context.ai.AttackType))
            {
                abilityController.TryUseAbility(context.ai.AttackType);
            }

            entityAim.AimTowards((target.position - transform.position).normalized);
        }

        if (context.ProtectTarget != null && ProtectTargetOutOfRange(transform.position, context))
        {
            stateMachine.ChangeState(stateMachine.returnState);
            return;
        }

        if (Avoidance.TryGetAvoidanceDirection(transform.position, context.ai.FleetingRange, context.ai.TargetLayers, out Vector2 direction))
        {
            entityMove.MoveTowards(direction);
        }
        else
        {
            if (context.EngageTarget != null)
            {
                stateMachine.ChangeState(stateMachine.attackState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }
    }

    private bool IsEnemy(GameObject targetObject)
    {
        return TeamManager.IsEnemy(gameObject, targetObject);
    }
}

public class BodyguardIdleState : BodyguardBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityAim entityAim;

    private readonly BodyguardStateMachine stateMachine;

    public BodyguardIdleState(BodyguardStateMachine stateMachine, GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(BodyguardStateContext context) { }

    public override void EnterState(BodyguardStateContext context) { }

    public override void ExitState(BodyguardStateContext context) { }

    public override void UpdateState(BodyguardStateContext context)
    {
        if (context.ProtectTarget != null && ProtectTargetOutOfRange(transform.position, context))
        {
            stateMachine.ChangeState(stateMachine.returnState);
            return;
        }

        if (TargetsInFleetingRange(transform.position, context.ai.FleetingRange, context.ai.TargetLayers, IsEnemy))
        {
            stateMachine.ChangeState(stateMachine.fleeState);
            return;
        }

        if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.EngageTarget = target;
            stateMachine.ChangeState(stateMachine.attackState);
            return;
        }

        if (context.ProtectTarget != null)
        {
            stateMachine.ChangeState(stateMachine.protectState);
            return;
        }
    }

    private bool IsEnemy(GameObject targetObject)
    {
        return TeamManager.IsEnemy(gameObject, targetObject);
    }
}

public class BodyguardReturnState : BodyguardBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly BodyguardStateMachine stateMachine;

    private bool reachedTargetOnce = false;
    private float timer = 0f;

    public BodyguardReturnState(BodyguardStateMachine stateMachine, GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(BodyguardStateContext context) { }

    public override void EnterState(BodyguardStateContext context)
    {
        reachedTargetOnce = false;
        timer = 0f;
    }

    public override void ExitState(BodyguardStateContext context) { }

    public override void UpdateState(BodyguardStateContext context)
    {
        if (context.ProtectTarget == null)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }

        if ((timer >= context.ai.ReturnLockStateTime || reachedTargetOnce) && ProtectTargetOutOfRange(transform.position, context))
        {
            if (TargetsInFleetingRange(transform.position, context.ai.FleetingRange, context.ai.TargetLayers, IsEnemy))
            {
                stateMachine.ChangeState(stateMachine.fleeState);
                return;
            }

            if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
            {
                context.EngageTarget = target;
                stateMachine.ChangeState(stateMachine.attackState);
                return;
            }
        }

        Update(context);
    }

    private void Update(BodyguardStateContext context)
    {
        Vector2 desiredDirection = (context.ProtectTarget.position - transform.position).normalized;

        if (Avoidance.TryGetAvoidanceDirection(transform.position, context.ai.FleetingRange, context.ai.TargetLayers, IsEnemy, out Vector2 fleeDirection))
        {
            Vector2 direction = (fleeDirection + desiredDirection).normalized;
            entityMove.MoveTowards(direction);
        }
        else
        {
            if (!ProtectTargetInMinFollowRange(transform.position, context))
            {
                entityMove.MoveTowards(desiredDirection);
            }
            else
            {
                reachedTargetOnce = true;
                entityMove.StopMoving();
            }
        }

        if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.EngageRange, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.EngageTarget = target;
            stateMachine.ChangeState(stateMachine.attackState);
            return;
        }
        else
        {
            entityAim.AimTowards(desiredDirection);
        }

        timer += Time.deltaTime;
    }

    protected bool IsEnemy(GameObject targetObject)
    {
        return TeamManager.IsEnemy(gameObject, targetObject);
    }
}