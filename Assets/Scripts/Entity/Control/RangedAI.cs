using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class RangedAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RangedAISettings settings;
    [SerializeField] private Transform attackIndicatorSpawn;

    private RangedStateContext context;
    private RangedStateMachine stateMachine;

    public void SetTarget(Transform target)
    {
        context.Target = target;
    }

    private void Awake()
    {
        context = new RangedStateContext(settings, attackIndicatorSpawn);
        stateMachine = new RangedStateMachine(gameObject, context);

        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onTakenDamage.Subscribe(OnDamageTaken);
        }
    }

    private void Start()
    {
        stateMachine.ChangeState(stateMachine.idleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    private void OnDestroy()
    {
        stateMachine?.Destroy();
    }

    private void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        if (TeamManager.IsAlly(damageEvent.SourceUser, gameObject)) return;

        if (damageEvent.SourceUser == null) return;

        if (context.Target != null)
        {
            float currentTargetSqrDistance = (GetClosestTargetPoint() - (Vector2)transform.position).sqrMagnitude;
            float attackerTargetSqrDistance = (damageEvent.SourceUser.transform.position - transform.position).sqrMagnitude;

            if (attackerTargetSqrDistance > currentTargetSqrDistance) return;
        }

        SetTarget(damageEvent.SourceUser.transform);
    }

    private Vector2 GetClosestTargetPoint()
    {
        Vector2 closestPoint = context.Target.position;

        if (context.TargetCollider != null)
        {
            closestPoint = context.TargetCollider.ClosestPoint(transform.position);
        }

        return closestPoint;
    }
}

public class RangedStateContext
{
    public readonly RangedAISettings ai;
    public readonly Transform attackIndicatorSpawn;

    private Transform target;
    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;

            if (value != null)
            {
                TargetCollider = value.GetComponent<Collider2D>();
            }
        }
    }

    public Collider2D TargetCollider { get; private set; }

    public RangedStateContext(RangedAISettings ai, Transform attackIndicatorSpawn)
    {
        this.ai = ai;
        this.attackIndicatorSpawn = attackIndicatorSpawn;
    }
}

public class RangedStateMachine : StateMachine<RangedStateContext>
{
    public readonly RangedFollowState followState;
    public readonly RangedAttackState attackState;
    public readonly RangedIdleState idleState;

    private readonly RangedStateContext context;

    public RangedStateMachine(GameObject gameObject, RangedStateContext context)
    {
        this.context = context;

        followState = new RangedFollowState(gameObject, this);
        attackState = new RangedAttackState(gameObject, this, context);
        idleState = new RangedIdleState(gameObject, this);
    }

    public override void ChangeState(State<RangedStateContext> newState)
    {
        currentState?.ExitState(context);
        currentState = newState;
        currentState?.EnterState(context);
    }

    public override void Update()
    {
        currentState?.UpdateState(context);
    }

    public override void LateUpdate()
    {
        currentState?.LateUpdateState(context);
    }

    public override void Destroy()
    {
        followState.DestroyState(context);
        attackState.DestroyState(context);
        idleState.DestroyState(context);
    }
}

public abstract class RangedBaseState : State<RangedStateContext> { }

public class RangedIdleState : RangedBaseState
{
    private readonly GameObject gameObject;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly RangedStateMachine stateMachine;

    public RangedIdleState(GameObject gameObject, RangedStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(RangedStateContext context) { }

    public override void EnterState(RangedStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();
    }

    public override void ExitState(RangedStateContext context) { }

    public override void LateUpdateState(RangedStateContext context) { }

    public override void UpdateState(RangedStateContext context)
    {
        if (context.Target == null)
        {
            if (TryGetRandomTarget(out Transform target))
            {
                context.Target = target;
                stateMachine.ChangeState(stateMachine.followState);
            }
        }
        else
        {
            stateMachine.ChangeState(stateMachine.followState);
        }
    }

    private bool TryGetRandomTarget(out Transform target)
    {
        GameObject enemy = TeamManager.GetRandomEnemy(gameObject);
        target = null;

        if (enemy != null)
        {
            target = enemy.transform;
            return true;
        }

        return false;
    }
}

public class RangedFollowState : RangedBaseState
{
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;

    private readonly RangedStateMachine stateMachine;

    public RangedFollowState(GameObject gameObject, RangedStateMachine stateMachine)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();
    }

    public override void DestroyState(RangedStateContext context) { }

    public override void EnterState(RangedStateContext context) { }

    public override void ExitState(RangedStateContext context) { }

    public override void UpdateState(RangedStateContext context)
    {
        if (context.Target == null)
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }
        else
        {
            Update(context);
        }
    }

    public override void LateUpdateState(RangedStateContext context) { }

    private void Update(RangedStateContext context)
    {
        Vector2 targetDirection = (context.Target.position - transform.position).normalized;

        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle(context) && TargetInRange(transform, context))
        {
            if (TargetAwareness.HasLineOfSight(transform.position, context.TargetCollider, context.ai.BlockLayers))
            {
                if (abilityController.CanUseAbility(context.ai.AttackType))
                {
                    stateMachine.ChangeState(stateMachine.attackState);
                    return;
                }
            }
        }

        if (Vector2.Distance(transform.position, context.Target.position) > context.ai.MinChaseRange)
        {
            entityMove.MoveTowards(targetDirection);
        }
        else
        {
            entityMove.StopMoving();
        }
    }

    private bool InStartAttackAngle(RangedStateContext context)
    {
        float angleDifference = Vector2.Angle(transform.up, context.Target.position - transform.position);
        return angleDifference < context.ai.MinStartAttackAngle;
    }

    private bool TargetInRange(Transform transform, RangedStateContext context)
    {
        Vector3 targetPosition = context.Target.position;

        if (context.TargetCollider != null)
        {
            targetPosition = context.TargetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude <= context.ai.AttackRange * context.ai.AttackRange;
    }
}

public class RangedAttackState : RangedBaseState
{
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly AbilityIndicator abilityIndicator;

    private readonly RangedStateMachine stateMachine;

    private readonly Stat.Multiplier aimSpeedMultiplier;

    private float attackTimer = 0f;
    private float recoverTimer = 0f;

    private AttackPhase attackPhase = AttackPhase.WindUp;

    private enum AttackPhase
    {
        WindUp,
        Attack,
        Recover
    }

    public RangedAttackState(GameObject gameObject, RangedStateMachine stateMachine, RangedStateContext context)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();

        aimSpeedMultiplier = new Stat.Multiplier(1f);
        abilityIndicator = CreateAbilityIndicator(context.ai.AttackType, gameObject);
    }

    public override void DestroyState(RangedStateContext context)
    {
        if (abilityIndicator == null) return;
        abilityIndicator.Destroy();
    }

    public override void EnterState(RangedStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();

        attackPhase = AttackPhase.WindUp;

        attackTimer = 0f;
        recoverTimer = 0f;

        abilityIndicator?.Reset();

        entityAim.RotationSpeed.AddMultiplier(aimSpeedMultiplier);
    }

    public override void ExitState(RangedStateContext context)
    {
        abilityIndicator?.Disable();
        entityAim.RotationSpeed.RemoveMultiplier(aimSpeedMultiplier);
    }

    public override void UpdateState(RangedStateContext context)
    {
        switch (attackPhase)
        {
            case AttackPhase.WindUp:
                UpdateWindUp(context);
                break;

            case AttackPhase.Attack:
                UpdateAttack(context);
                break;

            case AttackPhase.Recover:
                UpdateRecover(context);
                break;
        }
    }

    public override void LateUpdateState(RangedStateContext context)
    {
        abilityIndicator?.LateUpdate();
    }

    private void UpdateWindUp(RangedStateContext context)
    {
        if (abilityIndicator == null)
        {
            PerformAttack(context);
            attackPhase = AttackPhase.Attack;
            aimSpeedMultiplier.UpdateMultiplier(1f);
        }
        else
        {
            attackTimer += Time.deltaTime;

            abilityIndicator.Update();
            aimSpeedMultiplier.UpdateMultiplier(context.ai.AimingSpeedMultiplier);

            if (context.Target != null)
            {
                Vector2 direction = (context.Target.position - transform.position).normalized;
                entityAim.AimTowards(direction);
            }
            else
            {
                entityAim.StopAiming();
            }

            if (attackTimer > abilityIndicator.FirstHideTime)
            {
                PerformAttack(context);
                attackPhase = AttackPhase.Attack;
                aimSpeedMultiplier.UpdateMultiplier(1f);
            }
        }
    }

    private void UpdateAttack(RangedStateContext context)
    {
        if (abilityIndicator != null)
        {
            attackTimer += Time.deltaTime;

            abilityIndicator.Update();

            if (attackTimer > abilityIndicator.LastHideTime)
            {
                attackPhase = AttackPhase.Recover;
            }
        }
        else
        {
            attackPhase = AttackPhase.Recover;
        }
    }

    private void UpdateRecover(RangedStateContext context)
    {
        recoverTimer += Time.deltaTime;

        if (recoverTimer > context.ai.AttackRecover)
        {
            FinishAttack(context);
        }
    }

    private void PerformAttack(RangedStateContext context)
    {
        abilityController.TryUseAbility(context.ai.AttackType);
    }

    private void FinishAttack(RangedStateContext context)
    {
        if (context.Target == null)
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.followState);
        }
    }

    private AbilityIndicator CreateAbilityIndicator(AbilityType abilityType, GameObject gameObject)
    {
        if (abilityController.TryGetAbility(abilityType, out IAbility ability))
        {
            if (ability.IndicatorData != null)
            {
                return ability.IndicatorData.CreateAbilityIndicator(gameObject, abilityController.Spawn);
            }
        }

        return null;
    }
}