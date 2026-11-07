using System;
using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;
using Zeke.TeamSystem;

public class RobotBossAI : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private RobotBossAISettings settings;
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

    private BossState currentState = BossState.Idle;

    private AttackPhase attackPhase = AttackPhase.WindUp;

    private AbilityIndicator primaryAbilityIndicator;
    private AbilityIndicator secondaryAbilityIndicator;

    private float attackTimer;
    private float recoverTimer;

    private Predicate<GameObject> targetFilter;

    private enum BossState
    {
        Idle,
        Follow,
        PrimaryAttack,
        SecondaryAttack
    }

    private enum AttackPhase
    {
        WindUp,
        Attack,
        Recover,
        Track
    }

    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
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
        targetFilter = TargetFilter;

        primaryAbilityIndicator = GetAbilityIndicator(settings.PrimaryAttackType);
        secondaryAbilityIndicator = GetAbilityIndicator(settings.SecondaryAttackType);

        damageable.onTakenDamage.Subscribe(OnDamageTaken);
    }

    private void Start()
    {
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                UpdateIdle();
                break;
            case BossState.Follow:
                UpdateFollow();
                break;
            case BossState.PrimaryAttack:
                UpdatePrimaryAttack();
                break;
            case BossState.SecondaryAttack:
                UpdateSecondaryAttack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (currentState == BossState.PrimaryAttack)
        {
            primaryAbilityIndicator?.LateUpdate();
        }
        else if (currentState == BossState.SecondaryAttack)
        {
            secondaryAbilityIndicator?.LateUpdate();
        }
    }

    private void OnDestroy()
    {
        if (AbilityIndicatorPool.IsNull)
        {
            primaryAbilityIndicator.Destroy();
        }
        else
        {
            primaryAbilityIndicator.Disable();
            AbilityIndicatorPool.Instance.Return(primaryAbilityIndicator);
        }

        if (AbilityIndicatorPool.IsNull)
        {
            secondaryAbilityIndicator.Destroy();
        }
        else
        {
            secondaryAbilityIndicator.Disable();
            AbilityIndicatorPool.Instance.Return(secondaryAbilityIndicator);
        }
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == BossState.PrimaryAttack && newState != BossState.PrimaryAttack)
        {
            primaryAbilityIndicator?.Disable();
        }
        else if (currentState == BossState.SecondaryAttack && newState != BossState.SecondaryAttack)
        {
            secondaryAbilityIndicator?.Disable();
        }

        currentState = newState;

        switch (currentState)
        {
            case BossState.Idle:
                entityMove.StopMoving();
                entityAim.StopAiming();
                break;

            case BossState.Follow:
                break;

            case BossState.PrimaryAttack:
                entityMove.StopMoving();
                entityAim.StopAiming();

                attackPhase = AttackPhase.WindUp;

                attackTimer = 0f;
                recoverTimer = 0f;

                primaryAbilityIndicator?.Reset(gameObject, attackIndicatorSpawn);
                break;

            case BossState.SecondaryAttack:
                entityMove.StopMoving();
                entityAim.StopAiming();

                attackPhase = AttackPhase.WindUp;

                attackTimer = 0f;

                secondaryAbilityIndicator?.Reset(gameObject, attackIndicatorSpawn);
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
                ChangeState(BossState.Follow);
            }
        }
        else
        {
            ChangeState(BossState.Follow);
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
            ChangeState(BossState.Idle);
            return;
        }

        Vector2 targetDirection = (target.position - transform.position).normalized;
        entityAim.AimTowards(targetDirection);

        if (InStartAttackAngle() && TargetInSecondaryRange())
        {
            if (abilityController.CanUseAbility(settings.SecondaryAttackType))
            {
                ChangeState(BossState.SecondaryAttack);
                return;
            }
        }

        if (InStartAttackAngle() && TargetInPrimaryRange())
        {
            if (abilityController.CanUseAbility(settings.PrimaryAttackType))
            {
                ChangeState(BossState.PrimaryAttack);
                return;
            }
        }

        entityMove.MoveTowards(targetDirection);
    }

    private void UpdatePrimaryAttack()
    {
        switch (attackPhase)
        {
            case AttackPhase.WindUp:
                if (primaryAbilityIndicator == null)
                {
                    abilityController.TryUseAbility(settings.PrimaryAttackType);
                    attackPhase = AttackPhase.Attack;
                }
                else
                {
                    attackTimer += Time.deltaTime;
                    primaryAbilityIndicator.Update();

                    if (attackTimer > primaryAbilityIndicator.FirstHideTime)
                    {
                        abilityController.TryUseAbility(settings.PrimaryAttackType);
                        attackPhase = AttackPhase.Attack;
                    }
                }
                break;

            case AttackPhase.Attack:
                if (primaryAbilityIndicator != null)
                {
                    attackTimer += Time.deltaTime;
                    primaryAbilityIndicator.Update();

                    if (attackTimer > primaryAbilityIndicator.LastHideTime)
                    {
                        attackPhase = AttackPhase.Recover;
                    }
                }
                else
                {
                    attackPhase = AttackPhase.Recover;
                }
                break;

            case AttackPhase.Recover:
                recoverTimer += Time.deltaTime;
                if (recoverTimer > settings.PrimaryAttackRecover)
                {
                    FinishAttack();
                }
                break;
        }
    }

    private void UpdateSecondaryAttack()
    {
        switch (attackPhase)
        {
            case AttackPhase.WindUp:
                if (secondaryAbilityIndicator == null)
                {
                    abilityController.TryUseAbility(settings.SecondaryAttackType);
                    attackPhase = AttackPhase.Track;
                }
                else
                {
                    attackTimer += Time.deltaTime;
                    secondaryAbilityIndicator.Update();

                    if (attackTimer > secondaryAbilityIndicator.FirstHideTime)
                    {
                        abilityController.TryUseAbility(settings.SecondaryAttackType);
                        attackPhase = AttackPhase.Track;
                    }
                }
                break;

            case AttackPhase.Track:
                secondaryAbilityIndicator?.Update();

                if (!HasTarget)
                {
                    Target = TargetAwareness.GetClosestTargetToDirection(
                        transform.position,
                        entityAim.AimDirection,
                        settings.SecondaryAttackRange,
                        settings.TargetLayers,
                        settings.BlockLayers,
                        targetFilter
                    );
                }

                if (HasTarget)
                {
                    Vector2 targetDirection = (target.position - transform.position).normalized;
                    entityAim.AimTowards(targetDirection);
                }

                if (abilityController.TryGetAbility(settings.SecondaryAttackType, out IAbility ability))
                {
                    if (!ability.DurationActive)
                    {
                        FinishAttack();
                    }
                }
                else
                {
                    FinishAttack();
                }
                break;
        }
    }

    private void FinishAttack()
    {
        if (!HasTarget)
        {
            ChangeState(BossState.Idle);
        }
        else
        {
            ChangeState(BossState.Follow);
        }
    }

    private bool TargetInPrimaryRange()
    {
        Vector3 targetPosition = target.position;
        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(transform.position);
        }
        return (targetPosition - transform.position).sqrMagnitude <= settings.PrimaryStartRange * settings.PrimaryStartRange;
    }

    private bool TargetInSecondaryRange()
    {
        Vector3 targetPosition = target.position;
        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(transform.position);
        }
        return (targetPosition - transform.position).sqrMagnitude <= settings.SecondaryStartRange * settings.SecondaryStartRange;
    }

    private bool InStartAttackAngle()
    {
        float angleDifference = Vector2.Angle(transform.up, target.position - transform.position);
        return angleDifference < settings.MinStartAttackAngle;
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

    private bool TargetFilter(GameObject candidateTarget)
    {
        if (candidateTarget == gameObject) return false;
        if (TeamManager.Instance.IsAlly(gameObject, candidateTarget)) return false;
        return true;
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