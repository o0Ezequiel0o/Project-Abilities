using UnityEngine;
using System;
using Zeke.Abilities;
using Zeke.TeamSystem;

public class TurretAI : MonoBehaviour
{
    private enum TurretState
    {
        Idle,
        Attack
    }

    [Header("Dependency")]
    [SerializeField] private TurretAISettings settings;

    [Space]

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

    private TurretState currentState = TurretState.Idle;

    private Predicate<GameObject> targetFilter;

    private void Reset()
    {
        entityAim = GetComponentInChildren<EntityAim>();
        abilityController = GetComponentInChildren<AbilityController>();
    }

    private void Awake()
    {
        targetFilter = candidateTarget => TeamManager.Instance.IsEnemy(gameObject, candidateTarget);
    }

    private void Start()
    {
        ChangeState(TurretState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case TurretState.Idle:
                UpdateIdle();
                break;
            case TurretState.Attack:
                UpdateAttack();
                break;
        }
    }

    private void ChangeState(TurretState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case TurretState.Idle:
                Target = null;
                break;

            case TurretState.Attack:
                break;
        }
    }

    private void UpdateIdle()
    {
        if (TargetAwareness.TryGetClosestTargetToDirection(
            transform.position,
            entityAim.AimDirection,
            settings.Range,
            settings.TargetLayers,
            settings.BlockLayers,
            targetFilter,
            out Transform foundTarget))
        {
            Target = foundTarget;
            ChangeState(TurretState.Attack);
        }
    }

    private void UpdateAttack()
    {
        if (!HasTarget || TargetOutOfRange())
        {
            ChangeState(TurretState.Idle);
        }
        else
        {
            entityAim.AimTowards((target.position - transform.position).normalized);

            if (TargetAwareness.AnyTargetInLineOfSight(
                transform.position,
                entityAim.AimDirection,
                settings.Range,
                settings.TargetLayers,
                targetFilter,
                false))
            {
                abilityController.TryUseAbility(settings.AttackType);
            }
        }
    }

    private bool TargetOutOfRange()
    {
        Vector3 targetPosition = target.position;

        if (targetCollider != null)
        {
            targetPosition = targetCollider.ClosestPoint(transform.position);
        }

        return (targetPosition - transform.position).sqrMagnitude > settings.Range * settings.Range;
    }
}