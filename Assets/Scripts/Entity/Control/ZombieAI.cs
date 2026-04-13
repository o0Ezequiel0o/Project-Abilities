using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class ZombieAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ZombieAISettings settings;
    [SerializeField] private Transform attackIndicatorSpawn;

    private ZombieStateContext context;
    private ZombieStateMachine stateMachine;

    public void SetTarget(Transform target)
    {
        context.Target = target;
    }

    private void Awake()
    {
        context = new ZombieStateContext(settings, attackIndicatorSpawn);
        stateMachine = new ZombieStateMachine(gameObject, context);

        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onDamageTaken += OnDamageTaken;
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

public class ZombieStateContext
{
    public readonly ZombieAISettings ai;
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

    public ZombieStateContext(ZombieAISettings ai, Transform attackIndicatorSpawn)
    {
        this.ai = ai;
        this.attackIndicatorSpawn = attackIndicatorSpawn;
    }
}

public class ZombieStateMachine : StateMachine<ZombieStateContext>
{
    public readonly ZombieFollowState followState;
    public readonly ZombieAttackState attackState;
    public readonly ZombieIdleState idleState;

    private readonly ZombieStateContext context;

    public ZombieStateMachine(GameObject gameObject, ZombieStateContext context)
    {
        this.context = context;

        followState = new ZombieFollowState(gameObject, this);
        attackState = new ZombieAttackState(gameObject, this, context);
        idleState = new ZombieIdleState(gameObject, this);
    }

    public override void ChangeState(State<ZombieStateContext> newState)
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

public abstract class ZombieBaseState : State<ZombieStateContext> { }

public class ZombieIdleState : ZombieBaseState
{
    private readonly GameObject gameObject;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly ZombieStateMachine stateMachine;

    public ZombieIdleState(GameObject gameObject, ZombieStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();
    }

    public override void DestroyState(ZombieStateContext context) { }

    public override void EnterState(ZombieStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();
    }

    public override void ExitState(ZombieStateContext context) { }

    public override void LateUpdateState(ZombieStateContext context) { }

    public override void UpdateState(ZombieStateContext context)
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

public class ZombieFollowState : ZombieBaseState
{
    private readonly Transform transform;

    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;

    private readonly ZombieStateMachine stateMachine;

    public ZombieFollowState(GameObject gameObject, ZombieStateMachine stateMachine)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();
    }

    public override void DestroyState(ZombieStateContext context) { }

    public override void EnterState(ZombieStateContext context) { }

    public override void ExitState(ZombieStateContext context) { }

    public override void UpdateState(ZombieStateContext context)
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

    public override void LateUpdateState(ZombieStateContext context) { }

    private void Update(ZombieStateContext context)
    {
        Vector2 targetDirection = (context.Target.position - transform.position).normalized;

        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle(context) && TargetInRange(transform, context))
        {
            if (TargetAwareness.HasLineOfSight(transform.position, context.TargetCollider, context.ai.BlockLayers | context.ai.TargetLayers))
            {
                if (abilityController.CanUseAbility(context.ai.AttackType))
                {
                    stateMachine.ChangeState(stateMachine.attackState);
                    return;
                }
            }
        }
        
        entityMove.MoveTowards(targetDirection);
    }

    private bool InStartAttackAngle(ZombieStateContext context)
    {
        float angleDifference = Vector2.Angle(transform.up, context.Target.position - transform.position);
        return angleDifference < context.ai.MinStartAttackAngle;
    }

    private bool TargetInRange(Transform transform, ZombieStateContext context)
    {
        Vector3 targetPosition = context.Target.position;

        if (context.TargetCollider != null)
        {
            targetPosition = context.TargetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude <= context.ai.AttackRange * context.ai.AttackRange;
    }
}

public class ZombieAttackState : ZombieBaseState
{
    private readonly EntityMove entityMove;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly AbilityIndicator abilityIndicator;

    private readonly ZombieStateMachine stateMachine;

    private float attackTimer = 0f;
    private float recoverTimer = 0f;

    private AttackPhase attackPhase = AttackPhase.WindUp;

    private enum AttackPhase
    {
        WindUp,
        Attack,
        Recover
    }

    public ZombieAttackState(GameObject gameObject, ZombieStateMachine stateMachine, ZombieStateContext context)
    {
        this.stateMachine = stateMachine;

        entityMove = gameObject.GetComponent<EntityMove>();
        entityAim = gameObject.GetComponent<EntityAim>();

        abilityController = gameObject.GetComponent<AbilityController>();

        abilityIndicator = CreateAbilityIndicator(context.ai.AttackType, gameObject);
    }

    public override void DestroyState(ZombieStateContext context)
    {
        if (abilityIndicator == null) return;
        abilityIndicator.Destroy();
    }

    public override void EnterState(ZombieStateContext context)
    {
        entityMove.StopMoving();
        entityAim.StopAiming();

        attackPhase = AttackPhase.WindUp;

        attackTimer = 0f;
        recoverTimer = 0f;

        abilityIndicator?.Reset();
    }

    public override void ExitState(ZombieStateContext context)
    {
        abilityIndicator?.Disable();
    }

    public override void UpdateState(ZombieStateContext context)
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

    public override void LateUpdateState(ZombieStateContext context)
    {
        abilityIndicator?.LateUpdate();
    }

    private void UpdateWindUp(ZombieStateContext context)
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

    private void UpdateAttack(ZombieStateContext context)
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

    private void UpdateRecover(ZombieStateContext context)
    {
        recoverTimer += Time.deltaTime;

        if (recoverTimer > context.ai.AttackRecover)
        {
            FinishAttack(context);
        }
    }

    private void PerformAttack(ZombieStateContext context)
    {
        abilityController.TryUseAbility(context.ai.AttackType);
    }

    private void FinishAttack(ZombieStateContext context)
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