using System.Collections.Generic;
using UnityEngine;
using System;

namespace Testing
{
    public class Entity
    {
        private StateMachine stateMachine;
        private CustomContext context;

        private AttackStateTest<CustomContext> attackState;
        private DoNothingState doNothingState;

        private readonly GameObject gameObject; //dummy
        private readonly Transform transform; //dummy too

        private static readonly List<Collider2D> hits = new List<Collider2D>(16);

        public void Init()
        {
            stateMachine = new StateMachine();
            context = new CustomContext(null);

            CreateStates();
            CreateTransitions();
        }

        private void CreateStates()
        {
            attackState = new AttackStateTest<CustomContext>(stateMachine, gameObject, context);
        }

        private void CreateTransitions()
        {
            attackState.AddTransition(
                () => context.ProtectTarget != null && ProtectTargetOutOfRange(transform.position, context),
                doNothingState); //return state but i haven't made it yet

            attackState.AddTransition(
                () => TargetsInFleetingRange(transform.position, context.TargetRange, context.TargetLayers, IsEnemy),
                doNothingState); //flee state but i haven't made it yet

            attackState.AddTransition(
                () => context.Target == null,
                doNothingState); //protect state
        }

        protected bool ProtectTargetOutOfRange(Vector3 position, IProtectContext context)
        {
            if (context.ProtectTargetCollider != null)
            {
                return !TargetInRange(position, context.ProtectTargetCollider, context.MaxDistanceFromProtectTarget);
            }
            else
            {
                return !TargetInRange(position, context.ProtectTarget.position, context.MaxDistanceFromProtectTarget);
            }
        }

        protected bool TargetInRange(Vector2 position, Collider2D target, float range)
        {
            return TargetInRange(position, target.ClosestPoint(position), range);
        }

        protected bool TargetInRange(Vector2 position, Vector2 target, float range)
        {
            return (target - position).sqrMagnitude <= range * range;
        }

        protected bool TargetsInFleetingRange(Vector2 position, float range, LayerMask targetLayers, Predicate<GameObject> filter)
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

        private bool IsEnemy(GameObject targetObject)
        {
            return TeamManager.IsEnemy(gameObject, targetObject);
        }
    }

    public interface IAttackContext
    {
        public AbilityType AttackType { get; }
    }

    public interface ITargetContext
    {
        public Transform Target { get; set; }
        public Collider2D TargetCollider { get; set; }

        public LayerMask TargetLayers { get; }
        public LayerMask BlockLayers { get; }

        public float TargetRange { get; }
    }

    public interface IProtectContext
    {
        public Transform ProtectTarget { get; set; }
        public Collider2D ProtectTargetCollider { get; set; }

        public float MinFollowDistance { get; }
        public float ReturnLockStateTime { get; }
        public float MaxDistanceFromProtectTarget { get; }
    }

    public class CustomContext : ITargetContext, IProtectContext, IAttackContext
    {
        public Collider2D TargetCollider { get; set; }
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

        public Collider2D ProtectTargetCollider { get; set; }
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

        public LayerMask TargetLayers => settings.TargetLayers;
        public LayerMask BlockLayers => settings.BlockLayers;

        public float TargetRange => settings.EngageRange;

        public float MinFollowDistance => settings.MinFollowDistance;
        public float ReturnLockStateTime => settings.ReturnLockStateTime;
        public float MaxDistanceFromProtectTarget => settings.MaxDistanceFromProtectTarget;

        public AbilityType AttackType => settings.AttackType;

        private Transform target;
        private Transform protectTarget;

        public readonly BodyguardAISettings settings;

        public CustomContext(BodyguardAISettings settings)
        {
            this.settings = settings;
        }
    }

    public abstract class State
    {
        public readonly List<Transition> transitions = new List<Transition>();

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();

        public Transition AddTransition(Func<bool> condition, State state)
        {
            Transition transition = new Transition(condition, state);
            transitions.Add(transition);
            return transition;
        }

        public readonly struct Transition
        {
            public readonly State state;

            private readonly Func<bool> condition;

            public Transition(Func<bool> condition, State state)
            {
                this.condition = condition;
                this.state = state;
            }

            public bool EvaluateCondition()
            {
                return condition.Invoke();
            }
        }
    }

    public class StateMachine
    {
        protected State state;

        public void Update()
        {
            state?.Update();
        }

        public void ChangeState(State newState)
        {
            state?.Exit();
            state = newState;
            state?.Enter();
        }

        public void EvaluateTransitions(List<State.Transition> transitions)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].EvaluateCondition())
                {
                    ChangeState(transitions[i].state);
                    return;
                }
            }
        }
    }

    public class DoNothingState : BaseState
    {
        public override void Enter() { }
        public override void Exit() { }
        public override void Update() { }
    }

    public class AttackStateTest<T> : BaseState where T : ITargetContext, IAttackContext, IProtectContext
    {
        private readonly GameObject gameObject;
        private readonly Transform transform;

        private readonly EntityMove entityMove;
        private readonly EntityAim entityAim;

        private readonly AbilityController abilityController;
        private readonly ProjectileDetector projectileDetector;

        private readonly StateMachine stateMachine;

        private readonly T context;

        public AttackStateTest(StateMachine stateMachine, GameObject gameObject, T context)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
            this.stateMachine = stateMachine;

            entityMove = gameObject.GetComponent<EntityMove>();
            entityAim = gameObject.GetComponent<EntityAim>();

            abilityController = gameObject.GetComponent<AbilityController>();
            projectileDetector = gameObject.GetComponentInChildren<ProjectileDetector>();

            this.context = context;
        }

        public override void Enter() { }

        public override void Exit() { }

        public override void Update()
        {
            bool hasTarget = false;

            if (TargetAwareness.TryGetClosestTargetToDirection(transform.position, entityAim.AimDirection, context.TargetRange, context.TargetLayers, context.BlockLayers, IsEnemy, out Transform target))
            {
                context.Target = target;
                hasTarget = true;
            }

            Vector2 direction = GetProjectileDodgeDirection(transform.position, projectileDetector, gameObject);

            entityMove.MoveTowards(direction);

            if (hasTarget)
            {
                if (TargetAwareness.AnyTargetInLineOfSight(transform.position, entityAim.AimDirection, context.TargetRange, context.TargetLayers, IsEnemy) && abilityController.CanUseAbility(context.AttackType))
                {
                    abilityController.TryUseAbility(context.AttackType);
                }

                entityAim.AimTowards((target.position - transform.position).normalized);
            }

            stateMachine.EvaluateTransitions(transitions);
        }

        private bool IsEnemy(GameObject targetObject)
        {
            return TeamManager.IsEnemy(gameObject, targetObject);
        }
    }

    public abstract class BaseState : State
    {
        private static readonly List<Collider2D> hits = new List<Collider2D>(16);
        private static readonly List<Vector2> projectileDirections = new List<Vector2>(16);

        protected bool TargetsInFleetingRange(Vector2 position, float range, LayerMask targetLayers, Predicate<GameObject> filter)
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
                if (projectile.SourceUser == source) continue;

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

        protected bool ProtectTargetOutOfRange(Vector3 position, IProtectContext context)
        {
            if (context.ProtectTargetCollider != null)
            {
                return !TargetInRange(position, context.ProtectTargetCollider, context.MaxDistanceFromProtectTarget);
            }
            else
            {
                return !TargetInRange(position, context.ProtectTarget.position, context.MaxDistanceFromProtectTarget);
            }
        }
    }
}