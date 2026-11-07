using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class ZombieAI : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private ZombieAISettings settings;
    [SerializeField] private AbilityIndicatorDB abilityIndicatorDB;

    [Space]

    [SerializeField] private Transform attackIndicatorSpawn;

    [Space]

    [SerializeField] private Damageable damageable;

    [Space]

    [SerializeField] private EntityMove entityMove;
    [SerializeField] private EntityAim entityAim;

    [SerializeField] private AbilityController abilityController;

    private Transform target;
    private Collider2D targetCollider;

    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            if (target != null)
            {
                targetCollider = target.GetComponent<Collider2D>();
            }
            else
            {
                targetCollider = null;
            }
        }
    }

    public bool HasTarget => target != null && target.gameObject.activeInHierarchy;

    private ZombieState currentState = ZombieState.Idle;

    private AttackPhase attackPhase = AttackPhase.WindUp;
    private AbilityIndicator abilityIndicator;
    private float attackTimer;
    private float recoverTimer;

    private enum ZombieState
    {
        Idle,
        Follow,
        Attack
    }

    private enum AttackPhase
    {
        WindUp,
        Attack,
        Recover
    }

    private void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
        entityMove = GetComponentInChildren<EntityMove>();
        entityAim = GetComponentInChildren<EntityAim>();

        abilityController = GetComponentInChildren<AbilityController>();
    }

    private void Awake()
    {
        abilityIndicator = GetAbilityIndicator(settings.AttackType);
        damageable.onTakenDamage.Subscribe(OnDamageTaken);
    }

    private void Start()
    {
        ChangeState(ZombieState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case ZombieState.Idle:
                UpdateIdle();
                break;
            case ZombieState.Follow:
                UpdateFollow();
                break;
            case ZombieState.Attack:
                UpdateAttack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (currentState == ZombieState.Attack)
        {
            abilityIndicator?.LateUpdate();
        }
    }

    private void OnDestroy()
    {
        if (AbilityIndicatorPool.IsNull)
        {
            abilityIndicator.Destroy();
        }
        else
        {
            abilityIndicator.Disable();
            AbilityIndicatorPool.Instance.Return(abilityIndicator);
        }
    }

    private void ChangeState(ZombieState newState)
    {
        if (currentState == ZombieState.Attack && newState != ZombieState.Attack)
        {
            abilityIndicator?.Disable();
        }

        currentState = newState;

        switch (currentState)
        {
            case ZombieState.Idle:
                entityMove.StopMoving();
                entityAim.StopAiming();
                break;

            case ZombieState.Follow:
                break;

            case ZombieState.Attack:
                entityMove.StopMoving();
                entityAim.StopAiming();

                attackPhase = AttackPhase.WindUp;

                attackTimer = 0f;
                recoverTimer = 0f;

                abilityIndicator?.Reset(gameObject, attackIndicatorSpawn);
                break;
        }
    }

    private void UpdateIdle()
    {
        if (!HasTarget)
        {
            if (TryGetRandomTarget(out Transform foundTarget))
            {
                Target = foundTarget;
                ChangeState(ZombieState.Follow);
            }
        }
        else
        {
            ChangeState(ZombieState.Follow);
        }
    }

    private bool TryGetRandomTarget(out Transform foundTarget)
    {
        GameObject enemy = TeamManager.Instance.GetRandomEnemy(gameObject);
        foundTarget = null;

        if (enemy != null)
        {
            foundTarget = enemy.transform;
            return true;
        }

        return false;
    }

    private void UpdateFollow()
    {
        if (!HasTarget)
        {
            ChangeState(ZombieState.Idle);
            return;
        }

        Vector2 targetDirection = (target.position - transform.position).normalized;
        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle() && TargetInRange())
        {
            if (abilityController.CanUseAbility(settings.AttackType))
            {
                ChangeState(ZombieState.Attack);
                return;
            }
        }

        entityMove.MoveTowards(targetDirection);
    }

    private bool InStartAttackAngle()
    {
        float angleDifference = Vector2.Angle(transform.up, target.position - transform.position);
        return angleDifference < settings.MinStartAttackAngle;
    }

    private bool TargetInRange()
    {
        Vector3 targetPosition = target.position;

        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude <= settings.AttackRange * settings.AttackRange;
    }

    private void UpdateAttack()
    {
        switch (attackPhase)
        {
            case AttackPhase.WindUp:
                UpdateWindUp();
                break;
            case AttackPhase.Attack:
                UpdateAttackPhase();
                break;
            case AttackPhase.Recover:
                UpdateRecover();
                break;
        }
    }

    private void UpdateWindUp()
    {
        if (abilityIndicator == null)
        {
            PerformAttack();
            attackPhase = AttackPhase.Attack;
        }
        else
        {
            attackTimer += Time.deltaTime;
            abilityIndicator.Update();

            if (attackTimer > abilityIndicator.FirstHideTime)
            {
                PerformAttack();
                attackPhase = AttackPhase.Attack;
            }
        }
    }

    private void UpdateAttackPhase()
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

    private void UpdateRecover()
    {
        recoverTimer += Time.deltaTime;

        if (recoverTimer > settings.AttackRecover)
        {
            FinishAttack();
        }
    }

    private void PerformAttack()
    {
        abilityController.TryUseAbility(settings.AttackType);
    }

    private void FinishAttack()
    {
        if (!HasTarget)
        {
            ChangeState(ZombieState.Idle);
        }
        else
        {
            ChangeState(ZombieState.Follow);
        }
    }

    private AbilityIndicator GetAbilityIndicator(AbilityType abilityType)
    {
        if (abilityController.TryGetAbility(abilityType, out IAbility ability))
        {
            if (abilityIndicatorDB.TryGetAbilityIndicator(ability.Data, out AbilityIndicatorData indicatorData))
            {
                return AbilityIndicatorPool.Instance.Get(indicatorData, gameObject, attackIndicatorSpawn);
            }
        }

        return null;
    }

    private void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        if (TeamManager.Instance.IsAlly(damageEvent.SourceUser, gameObject)) return;
        if (damageEvent.SourceUser == null) return;

        if (HasTarget)
        {
            float currentTargetSqrDistance = (GetClosestTargetPoint() - (Vector2)transform.position).sqrMagnitude;
            float attackerTargetSqrDistance = (damageEvent.SourceUser.transform.position - transform.position).sqrMagnitude;

            if (attackerTargetSqrDistance > currentTargetSqrDistance) return;
        }

        Target = damageEvent.SourceUser.transform;
    }

    private Vector2 GetClosestTargetPoint()
    {
        Vector2 closestPoint = target.position;

        if (targetCollider != null)
        {
            closestPoint = targetCollider.ClosestPoint(transform.position);
        }

        return closestPoint;
    }
}