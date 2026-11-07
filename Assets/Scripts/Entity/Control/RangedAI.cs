using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class RangedAI : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private RangedAISettings settings;
    [SerializeField] private AbilityIndicatorDB abilityIndicatorDB;

    [Space]

    [SerializeField] private Transform attackIndicatorSpawn;

    [Space]

    [SerializeField] private Damageable damageable;

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

    private RangedState currentState = RangedState.Idle;

    private AttackPhase attackPhase = AttackPhase.WindUp;

    private AbilityIndicator abilityIndicator;
    private Stat.Multiplier aimSpeedMultiplier;

    private float attackTimer;
    private float recoverTimer;

    private enum RangedState
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
        aimSpeedMultiplier = new Stat.Multiplier(1f);

        abilityIndicator = GetAbilityIndicator(settings.AttackType);

        damageable.onTakenDamage.Subscribe(OnDamageTaken);

    }

    private void Start()
    {
        ChangeState(RangedState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case RangedState.Idle:
                UpdateIdle();
                break;
            case RangedState.Follow:
                UpdateFollow();
                break;
            case RangedState.Attack:
                UpdateAttack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (currentState == RangedState.Attack)
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

    private void ChangeState(RangedState newState)
    {
        if (currentState == RangedState.Attack && newState != RangedState.Attack)
        {
            abilityIndicator?.Disable();
            entityAim.RotationSpeed.RemoveMultiplier(aimSpeedMultiplier);
        }

        currentState = newState;

        switch (currentState)
        {
            case RangedState.Idle:
                entityMove.StopMoving();
                entityAim.StopAiming();
                break;

            case RangedState.Follow:
                break;

            case RangedState.Attack:
                entityMove.StopMoving();
                entityAim.StopAiming();

                attackPhase = AttackPhase.WindUp;
                attackTimer = 0f;
                recoverTimer = 0f;

                abilityIndicator?.Reset(gameObject, attackIndicatorSpawn);
                entityAim.RotationSpeed.AddMultiplier(aimSpeedMultiplier);
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
                ChangeState(RangedState.Follow);
            }
        }
        else
        {
            ChangeState(RangedState.Follow);
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
            ChangeState(RangedState.Idle);
            return;
        }

        Vector2 targetDirection = (target.position - transform.position).normalized;
        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle() && TargetInRange())
        {
            if (abilityController.CanUseAbility(settings.AttackType))
            {
                ChangeState(RangedState.Attack);
                return;
            }
        }

        if (Vector2.Distance(transform.position, target.position) > settings.MinChaseRange)
        {
            entityMove.MoveTowards(targetDirection);
        }
        else
        {
            entityMove.StopMoving();
        }
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
            aimSpeedMultiplier.UpdateMultiplier(1f);
        }
        else
        {
            attackTimer += Time.deltaTime;
            abilityIndicator.Update();
            aimSpeedMultiplier.UpdateMultiplier(settings.AimingSpeedMultiplier);

            if (HasTarget)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                entityAim.AimTowards(direction);
            }
            else
            {
                entityAim.StopAiming();
            }

            if (attackTimer > abilityIndicator.FirstHideTime)
            {
                PerformAttack();
                attackPhase = AttackPhase.Attack;
                aimSpeedMultiplier.UpdateMultiplier(1f);
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
            ChangeState(RangedState.Idle);
        }
        else
        {
            ChangeState(RangedState.Follow);
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