using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public class HealEvent : IHealEvent, IHealedHealthEvent, IHealHealthEvent, IHitHealEvent
    {
        // Context
        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }
        public Vector2 Direction { get; private set; }
        public float ProcCoefficient { get; private set; }

        // Flags
        public bool IsHit { get; private set; }
        public bool IsRejected { get; set; }

        // Data
        public List<ItemData> ProcChainBranch { get; private set; } = new List<ItemData>();

        // Damage
        public float BaseHealing { get; private set; }
        public float Healing { get; private set; }

        //Modifiers
        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

        // Results & State
        public float HealthHealed { get; private set; }

        private DamageableEventListener healEventListener;
        private bool isHealEventListener = false;

        public HealEvent() { }

        public HealEvent Initialize(HealInfo healInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
        {
            IsHit = healInfo.hit;

            ProcChainBranch.Clear();
            ProcChainBranch.AddRange(procChain);

            Healing = healInfo.baseHealing;
            BaseHealing = healInfo.baseHealing;
            ProcCoefficient = healInfo.procCoefficient;

            Direction = healInfo.direction;

            Receiver = receiver;
            SourceUser = userSource;
            SourceObject = sourceObject;

            HealthHealed = 0f;
            IsRejected = false;

            FlatAccumulator = 0f;

            healEventListener = null;
            isHealEventListener = false;

            Multiplier.Clear();

            return this;
        }

        public void Execute()
        {
            try
            {
                if (Receiver == null || !Receiver.IsAlive || Receiver.MarkedForDeath) return;

                isHealEventListener = SourceUser != null && SourceUser.TryGetComponent(out healEventListener);

                Receiver.onHealEvent?.Invoke(this);

                if (IsRejected) return;

                if (IsHit) HandleOnHitEvents();

                if (IsRejected) return;

                HandlePreHealingEvents();

                if (IsRejected || Healing <= 0f) return;

                float healthHealed = HandleHealing();

                if (healthHealed <= 0f) return;

                HandlePostHealingEvents();
            }
            finally
            {
                if (!DamageableEventPool.IsNull)
                {
                    DamageableEventPool.Instance.ReturnHeal(this);
                }
            }
        }

        public HealEventSnapshot GetSnapshot()
        {
            return new HealEventSnapshot(Receiver, SourceUser, SourceObject, Direction, ProcCoefficient, IsHit, IsRejected,
                new List<ItemData>(ProcChainBranch), BaseHealing, Healing, FlatAccumulator, Multiplier.Value, HealthHealed);
        }

        private float HandleHealing()
        {
            Healing = Receiver.CalculateHealing(this);
            HealthHealed = Receiver.ReceiveHealing(Healing);

            return HealthHealed;
        }

        private void HandlePreHealingEvents()
        {
            if (Healing <= 0f) return;

            if (isHealEventListener)
            {
                healEventListener.CallHealHealthEvent(this);
            }

            if (Healing <= 0f) return;

            Receiver.onReceiveHealth?.Invoke(this);
        }

        private void HandlePostHealingEvents()
        {
            Receiver.onReceivedHealth?.Invoke(this);

            if (isHealEventListener)
            {
                healEventListener.CallHealedHealthEvent(this);
            }
        }

        private void HandleOnHitEvents()
        {
            Receiver.onHealHitTaken?.Invoke(this);

            if (isHealEventListener)
            {
                healEventListener.CallHealHitEvent(this);
            }
        }
    }

    public readonly struct HealEventSnapshot
    {
        // Context
        public readonly Damageable receiver;
        public readonly GameObject sourceUser;
        public readonly GameObject sourceObject;
        public readonly Vector2 direction;
        public readonly float procCoefficient;

        // Flags
        public readonly bool isHit;
        public readonly bool isRejected;

        // Data
        public readonly List<ItemData> procChainBranch;

        // Damage
        public readonly float baseHealing;
        public readonly float healing;

        // Modifiers
        public readonly float flatAccumulator;
        public readonly float multiplier;

        // Results & State
        public readonly float healthHealed;
        public readonly float overflowHealing;
        public readonly float uncappedHealing;

        public HealEventSnapshot(Damageable receiver, GameObject sourceUser, GameObject sourceObject, Vector2 direction, float procCoefficient, bool isHit, 
            bool isRejected, List<ItemData> procChainBranch, float baseHealing, float healing, float flatAccumulator, float multiplier, float healthHealed)
        {
            this.receiver = receiver;
            this.sourceUser = sourceUser;
            this.sourceObject = sourceObject;
            this.direction = direction;
            this.procCoefficient = procCoefficient;
            this.isHit = isHit;
            this.isRejected = isRejected;
            this.procChainBranch = procChainBranch;
            this.baseHealing = baseHealing;
            this.healing = healing;
            this.flatAccumulator = flatAccumulator;
            this.multiplier = multiplier;
            this.healthHealed = healthHealed;

            overflowHealing = Mathf.Max(0f, healing - healthHealed);
            uncappedHealing = healthHealed + overflowHealing;
        }
    }
}