using UnityEngine;
using System;
using Zeke.Abilities;
using Zeke.TeamSystem;

public class TurretAI : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private TurretAISettings settings;

    private TurretStateContext context;
    private TurretStateMachine stateMachine;

    private void Awake()
    {
        context = new TurretStateContext(settings);
        stateMachine = new TurretStateMachine(gameObject, context);

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

public class TurretStateContext
{
    public readonly TurretAISettings ai;

    private Transform target;
    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;

            if (target != null)
            {
                TargetCollider = value.GetComponent<Collider2D>();
            }
        }
    }

    public Collider2D TargetCollider { get; private set; }

    public TurretStateContext(TurretAISettings settings)
    {
        ai = settings;
    }
}

public class TurretStateMachine : StateMachine<TurretStateContext>
{
    public readonly TurretAttackState attackState;
    public readonly TurretIdleState idleState;

    private readonly TurretStateContext context;

    public TurretStateMachine(GameObject gameObject, TurretStateContext context)
    {
        idleState = new TurretIdleState(gameObject, this);
        attackState = new TurretAttackState(gameObject, this);

        this.context = context;
    }

    public override void ChangeState(State<TurretStateContext> newState)
    {
        currentState?.ExitState(context);
        currentState = newState;
        currentState?.EnterState(context);
    }

    public override void Destroy()
    {
        attackState.DestroyState(context);
        idleState.DestroyState(context);
    }

    public override void Update()
    {
        currentState?.UpdateState(context);
    }
}

public class TurretIdleState : State<TurretStateContext>
{
    private readonly Transform transform;
    private readonly EntityAim entityAim;

    private readonly TurretStateMachine stateMachine;
    private readonly Predicate<GameObject> IsEnemy;

    public TurretIdleState(GameObject gameObject, TurretStateMachine stateMachine)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityAim = gameObject.GetComponent<EntityAim>();
        IsEnemy = target => TeamManager.IsEnemy(gameObject, target);
    }

    public override void DestroyState(TurretStateContext context) { }

    public override void EnterState(TurretStateContext context) { }

    public override void ExitState(TurretStateContext context) { }

    public override void UpdateState(TurretStateContext context)
    {
        if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.ai.Range, context.ai.TargetLayers, context.ai.BlockLayers, IsEnemy, out Transform target))
        {
            context.Target = target;
            stateMachine.ChangeState(stateMachine.attackState);
        }
    }
}

public class TurretAttackState : State<TurretStateContext>
{
    private readonly Transform transform;
    private readonly EntityAim entityAim;

    private readonly AbilityController abilityController;
    private readonly TurretStateMachine stateMachine;

    private readonly Predicate<GameObject> IsEnemy;

    public TurretAttackState(GameObject gameObject, TurretStateMachine stateMachine)
    {
        transform = gameObject.transform;
        this.stateMachine = stateMachine;

        entityAim = gameObject.GetComponent<EntityAim>();
        abilityController = gameObject.GetComponent<AbilityController>();

        IsEnemy = target => TeamManager.IsEnemy(gameObject, target);
    }

    public override void DestroyState(TurretStateContext context) { }

    public override void EnterState(TurretStateContext context) { }

    public override void ExitState(TurretStateContext context) { }

    public override void UpdateState(TurretStateContext context)
    {
        if (context.Target == null || TargetOutOfRange(context))
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }
        else
        {
            entityAim.AimTowards((context.Target.position - transform.position).normalized);

            if (TargetAwareness.AnyTargetInLineOfSight(transform.position, entityAim.AimDirection, context.ai.Range, context.ai.TargetLayers, IsEnemy))
            {
                Attack(context);
            }
        }
    }

    private void Attack(TurretStateContext context)
    {
        abilityController.TryUseAbility(context.ai.AttackType);
    }

    private bool TargetOutOfRange(TurretStateContext context)
    {
        Vector3 targetPosition = context.Target.position;

        if (context.TargetCollider != null)
        {
            targetPosition = context.TargetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude > context.ai.Range * context.ai.Range;
    }
}