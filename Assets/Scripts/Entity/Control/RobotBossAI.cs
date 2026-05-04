using System;
using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class RobotBossAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RobotBossAISettings settings;
    [SerializeField] private Transform attackIndicatorSpawn;

    private RobotBossStateContext context;
    private RobotBossStateMachine stateMachine;

    public void SetTarget(Transform target)
    {
        context.Target = target;
    }

    private void Awake()
    {
        context = new RobotBossStateContext(settings, attackIndicatorSpawn);
        stateMachine = new RobotBossStateMachine(gameObject, context);

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

public class RobotBossStateContext
{
    public readonly RobotBossAISettings ai;
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

    public RobotBossStateContext(RobotBossAISettings ai, Transform attackIndicatorSpawn)
    {
        this.ai = ai;
        this.attackIndicatorSpawn = attackIndicatorSpawn;
    }
}

public class RobotBossStateMachine : StateMachine<RobotBossStateContext>
{
    public readonly RobotBossFollowState followState;
    public readonly RobotBossPrimaryAttackState primaryAttackState;
    public readonly RobotBossSecondaryAttackState secondaryAttackState;
    public readonly RobotBossIdleState idleState;

    private readonly RobotBossStateContext context;

    public RobotBossStateMachine(GameObject gameObject, RobotBossStateContext context)
    {
        this.context = context;

        followState = new RobotBossFollowState(gameObject, this);
        primaryAttackState = new RobotBossPrimaryAttackState(gameObject, this, context);
        secondaryAttackState = new RobotBossSecondaryAttackState(gameObject, this, context);
        idleState = new RobotBossIdleState(gameObject, this);
    }

    public override void ChangeState(State<RobotBossStateContext> newState)
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
        primaryAttackState.DestroyState(context);
        secondaryAttackState.DestroyState(context);
        idleState.DestroyState(context);
    }
}

public abstract class RobotBossBaseState : State<RobotBossStateContext>
{
    protected bool TargetInPrimaryRange(Transform transform, RobotBossStateContext context)
    {
        Vector3 targetPosition = context.Target.position;

        if (context.TargetCollider != null)
        {
            targetPosition = context.TargetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude <= context.ai.PrimaryStartRange * context.ai.PrimaryStartRange;
    }

    protected bool TargetInSecondaryRange(Transform transform, RobotBossStateContext context)
    {
        Vector3 targetPosition = context.Target.position;

        if (context.TargetCollider != null)
        {
            targetPosition = context.TargetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude <= context.ai.SecondaryStartRange * context.ai.SecondaryStartRange;
    }

    protected bool InStartAttackAngle(Transform transform, RobotBossStateContext context)
    {
        float angleDifference = Vector2.Angle(transform.up, context.Target.position - transform.position);
        return angleDifference < context.ai.MinStartAttackAngle;
    }

    protected AbilityIndicator CreateAbilityIndicator(AbilityController abilityController, AbilityType abilityType, GameObject gameObject)
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

public class RobotBossIdleState : RobotBossBaseState
{
    private readonly GameObject gameObject;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly RobotBossStateMachine stateMachine;

    public RobotBossIdleState(GameObject gameObject, RobotBossStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(RobotBossStateContext context) { }

    public override void EnterState(RobotBossStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();
    }

    public override void ExitState(RobotBossStateContext context) { }

    public override void LateUpdateState(RobotBossStateContext context) { }

    public override void UpdateState(RobotBossStateContext context)
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

public class RobotBossFollowState : RobotBossBaseState
{
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;

    private readonly RobotBossStateMachine stateMachine;

    public RobotBossFollowState(GameObject gameObject, RobotBossStateMachine stateMachine)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();
    }

    public override void DestroyState(RobotBossStateContext context) { }

    public override void EnterState(RobotBossStateContext context) { }

    public override void ExitState(RobotBossStateContext context) { }

    public override void UpdateState(RobotBossStateContext context)
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

    public override void LateUpdateState(RobotBossStateContext context) { }

    private void Update(RobotBossStateContext context)
    {
        Vector2 targetDirection = (context.Target.position - transform.position).normalized;

        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle(transform, context) && TargetInSecondaryRange(transform, context))
        {
            if (TargetAwareness.HasLineOfSight(transform.position, context.TargetCollider, context.ai.BlockLayers | context.ai.TargetLayers))
            {
                if (abilityController.CanUseAbility(context.ai.SecondaryAttackType))
                {
                    stateMachine.ChangeState(stateMachine.secondaryAttackState);
                    return;
                }
            }
        }

        if (InStartAttackAngle(transform, context) && TargetInPrimaryRange(transform, context))
        {
            if (TargetAwareness.HasLineOfSight(transform.position, context.TargetCollider, context.ai.BlockLayers | context.ai.TargetLayers))
            {
                if (abilityController.CanUseAbility(context.ai.PrimaryAttackType))
                {
                    stateMachine.ChangeState(stateMachine.primaryAttackState);
                    return;
                }
            }
        }
        
        entityMove.MoveTowards(targetDirection);
    }
}

public class RobotBossPrimaryAttackState : RobotBossBaseState
{
    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly AbilityIndicator abilityIndicator;

    private readonly RobotBossStateMachine stateMachine;

    private float attackTimer = 0f;
    private float recoverTimer = 0f;

    private AttackPhase attackPhase = AttackPhase.WindUp;

    private enum AttackPhase
    {
        WindUp,
        Attack,
        Recover
    }

    public RobotBossPrimaryAttackState(GameObject gameObject, RobotBossStateMachine stateMachine, RobotBossStateContext context)
    {
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();

        abilityIndicator = CreateAbilityIndicator(abilityController, context.ai.PrimaryAttackType, gameObject);
    }

    public override void DestroyState(RobotBossStateContext context)
    {
        if (abilityIndicator == null) return;
        abilityIndicator.Destroy();
    }

    public override void EnterState(RobotBossStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();

        attackPhase = AttackPhase.WindUp;

        attackTimer = 0f;
        recoverTimer = 0f;

        abilityIndicator?.Reset();
    }

    public override void ExitState(RobotBossStateContext context)
    {
        abilityIndicator?.Disable();
    }

    public override void UpdateState(RobotBossStateContext context)
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

    public override void LateUpdateState(RobotBossStateContext context)
    {
        abilityIndicator?.LateUpdate();
    }

    private void UpdateWindUp(RobotBossStateContext context)
    {
        if (abilityIndicator == null)
        {
            PerformAttack(context);
            attackPhase = AttackPhase.Attack;
        }
        else
        {
            attackTimer += Time.deltaTime;

            abilityIndicator.Update();

            if (attackTimer > abilityIndicator.FirstHideTime)
            {
                PerformAttack(context);
                attackPhase = AttackPhase.Attack;
            }
        }
    }

    private void UpdateAttack(RobotBossStateContext context)
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

    private void UpdateRecover(RobotBossStateContext context)
    {
        recoverTimer += Time.deltaTime;

        if (recoverTimer > context.ai.PrimaryAttackRecover)
        {
            FinishAttack(context);
        }
    }

    private void PerformAttack(RobotBossStateContext context)
    {
        abilityController.TryUseAbility(context.ai.PrimaryAttackType);
    }

    private void FinishAttack(RobotBossStateContext context)
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
}

public class RobotBossSecondaryAttackState : RobotBossBaseState
{
    private readonly GameObject gameObject;
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly AbilityIndicator abilityIndicator;

    private readonly RobotBossStateMachine stateMachine;

    private float attackTimer = 0f;

    private readonly Predicate<GameObject> targetFilter;
    private AttackPhase attackPhase = AttackPhase.WindUp;

    private enum AttackPhase
    {
        WindUp,
        Track
    }

    public RobotBossSecondaryAttackState(GameObject gameObject, RobotBossStateMachine stateMachine, RobotBossStateContext context)
    {
        this.stateMachine = stateMachine;
        this.gameObject = gameObject;
        transform = gameObject.transform;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();

        abilityIndicator = CreateAbilityIndicator(abilityController, context.ai.SecondaryAttackType, gameObject);
        targetFilter = TargetFilter;
    }

    public override void DestroyState(RobotBossStateContext context)
    {
        if (abilityIndicator == null) return;
        abilityIndicator.Destroy();
    }

    public override void EnterState(RobotBossStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();

        attackPhase = AttackPhase.WindUp;

        attackTimer = 0f;

        abilityIndicator?.Reset();
    }

    public override void ExitState(RobotBossStateContext context)
    {
        abilityIndicator?.Disable();
    }

    public override void UpdateState(RobotBossStateContext context)
    {
        switch (attackPhase)
        {
            case AttackPhase.WindUp:
                UpdateWindUp(context);
                break;

            case AttackPhase.Track:
                UpdateTracking(context);
                break;
        }
    }

    public override void LateUpdateState(RobotBossStateContext context)
    {
        abilityIndicator?.LateUpdate();
    }

    private void UpdateWindUp(RobotBossStateContext context)
    {
        if (abilityIndicator == null)
        {
            PerformAttack(context);
            attackPhase = AttackPhase.Track;
        }
        else
        {
            attackTimer += Time.deltaTime;

            abilityIndicator.Update();

            if (attackTimer > abilityIndicator.FirstHideTime)
            {
                PerformAttack(context);
                attackPhase = AttackPhase.Track;
            }
        }
    }

    private void UpdateTracking(RobotBossStateContext context)
    {
        abilityIndicator?.Update();

        if (context.Target == null)
        {
            context.Target = TargetAwareness.GetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.SecondaryAttackRange, context.ai.TargetLayers, context.ai.BlockLayers, targetFilter);
        }

        if (context.Target != null)
        {
            Vector2 targetDirection = (context.Target.transform.position - transform.position).normalized;
            entityAim.AimTowards(targetDirection);
        }

        if (abilityController.TryGetAbility(context.ai.SecondaryAttackType, out IAbility ability))
        {
            if (!ability.DurationActive)
            {
                FinishAttack(context);
            }
        }
        else
        {
            FinishAttack(context);
        }
    }

    private void PerformAttack(RobotBossStateContext context)
    {
        abilityController.TryUseAbility(context.ai.SecondaryAttackType);
    }

    private void FinishAttack(RobotBossStateContext context)
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

    private bool TargetFilter(GameObject target)
    {
        if (target == gameObject) return false;
        if (TeamManager.IsAlly(gameObject, target)) return false;

        return true;
    }
}