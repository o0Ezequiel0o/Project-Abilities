using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;
using Zeke.Abilities.Indicators;

namespace AI.BehaviourTrees 
{
    public interface IStrategy 
    {
        Node.Status Process();

        void Reset() {
            // Noop
        }
    }

    public class ActionStrategy : IStrategy 
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething) 
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process() 
        {
            doSomething();
            return Node.Status.Success;
        }
    }

    public class Condition : IStrategy 
    {
        private readonly Func<bool> predicate;

        public Condition(Func<bool> predicate) 
        {
            this.predicate = predicate;
        }

        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
    }

    public class MoveToTarget : IStrategy
    {
        private readonly Transform transform;
        private readonly ITargeter targeter;

        private readonly EntityMove entityMove;
        private readonly EntityAim entityAim;

        public MoveToTarget(GameObject gameObject, ITargeter targeter)
        {
            transform = gameObject.transform;
            this.targeter = targeter;

            entityMove = gameObject.GetComponent<EntityMove>();
            entityAim = gameObject.GetComponent<EntityAim>();
        }

        public Node.Status Process()
        {
            if (targeter.Target == null) return Node.Status.Failure;

            Vector2 targetDirection = (targeter.Target.position - transform.position).normalized;

            entityAim.AimTowards(targetDirection);
            entityMove.MoveTowards(targetDirection);

            return Node.Status.Running;
        }

        public void Reset() { }
    }

    public class GetAttackTypeForTarget : IStrategy
    {
        private readonly Transform transform;
        private readonly IAttacker attacker;

        private readonly AbilityController abilityController;
        private readonly AbilityIndicator abilityIndicator;

        public GetAttackTypeForTarget(GameObject gameObject, IAttacker attacker)
        {
            transform = gameObject.transform;
            this.attacker = attacker;
        }

        public Node.Status Process()
        {
            if (attacker.Target == null) return Node.Status.Failure;

            return Node.Status.Running;
        }

        public void Reset() { }
    }

    public class ZombieAIData : ITargeter, IAttacker
    {
        public Transform Target
        {
            get => _target;

            set
            {
                if (value != null && value.TryGetComponent(out Collider2D col))
                {
                    TargetCollider = col;
                }

                _target = value;
            }
        }

        public Collider2D TargetCollider { get; private set; }

        public float TargetRange { get; private set; }

        public LayerMask TargetLayer { get; private set; }
        public LayerMask BlockLayer { get; private set; }

        public AttackInfo CurrentAttack { get; set; }
        public List<AttackInfo> AttacksInfo { get; private set; }

        private Transform _target;

        public ZombieAIData(LayerMask targetLayer, LayerMask blockLayer, float targetRange, List<AttackInfo> attacksInfo)
        {
            TargetLayer = targetLayer;
            BlockLayer = blockLayer;
            TargetRange = targetRange;

            AttacksInfo = attacksInfo;
        }
    }

    public interface ITargeter
    {
        public Transform Target { get; }
        public Collider2D TargetCollider { get; }

        public float TargetRange { get; }

        public LayerMask TargetLayer { get; }
        public LayerMask BlockLayer { get; }
    }

    public interface IAttacker
    {
        public Transform Target { get; }
        public Collider2D TargetCollider { get; }

        public AttackInfo CurrentAttack { get; }
        public List<AttackInfo> AttacksInfo { get; }
    }

    public readonly struct AttackInfo
    {
        public readonly AbilityType abilityType;
        public readonly float attackStartRange;
        public readonly float attackStartAngle;
        public readonly float attackStopRange;
        public readonly int priority;

        public AttackInfo(AbilityType abilityType, float attackStartRange, float attackStartAngle, float attackStopRange, int priority = 0)
        {
            this.abilityType = abilityType;
            this.attackStartRange = attackStartRange;
            this.attackStartAngle = attackStartAngle;
            this.attackStopRange = attackStopRange;
            this.priority = priority;
        }
    }
}
