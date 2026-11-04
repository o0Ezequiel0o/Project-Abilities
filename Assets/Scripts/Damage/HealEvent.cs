using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public class HealEvent
    {
        public float Healing { get; private set; }
        public float BaseHealing { get; private set; }
        public float HealthHealed { get; private set; }
        public float OverflowHealing { get; private set; }

        public float UncappedHealing => HealthHealed + OverflowHealing;

        public float ProcCoefficient { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; }

        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }

        public bool IsHit { get; private set; }
        public Vector2 Direction { get; private set; }

        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; private set; }

        private HealEventListener healEventListener;
        private bool isHealEventListener = false;

        public HealEvent(HealInfo healInfo, Damageable receiver, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
        {
            IsHit = healInfo.hit;
            Direction = healInfo.direction;

            Healing = healInfo.baseHealing;
            BaseHealing = healInfo.baseHealing;

            Receiver = receiver;
            SourceUser = sourceUser;
            SourceObject = sourceObject;

            ProcChainBranch = procChain;
            Multiplier = new Stat(1f, 0f, 0f, float.PositiveInfinity);
        }

        public void ExecuteEventFlow()
        {
            isHealEventListener = SourceUser.TryGetComponent(out healEventListener);

            if (Healing >= 0)
            {
                HandlePreHealingEvents();
                HandleHealing();
            }

            if (HealthHealed > 0)
            {
                HandleHealingEvents();
            }
        }

        private void HandleHealing()
        {
            Healing = Receiver.CalculateHealing(this);
            HealthHealed = Receiver.ReceiveHealing(Healing);

            OverflowHealing = Mathf.Max(0f, Healing - HealthHealed);
        }

        private void HandlePreHealingEvents()
        {
            if (isHealEventListener)
            {
                healEventListener.CallHealHealthEvent(new OnHealHealthEvent(this));
            }

            Receiver.onReceiveHealth?.Invoke(this);
        }

        private void HandleHealingEvents()
        {
            Receiver.onReceivedHealth?.Invoke(this);

            if (isHealEventListener)
            {
                healEventListener.CallHealedHealthEvent(new OnHealedHealthEvent(this));
            }
        }

        public readonly struct OnHealHealthEvent
        {
            public readonly HealEvent healEvent;

            public OnHealHealthEvent(HealEvent healEvent)
            {
                this.healEvent = healEvent;
            }
        }

        public readonly struct OnHealedHealthEvent
        {
            public readonly HealEvent healEvent;

            public OnHealedHealthEvent(HealEvent healEvent)
            {
                this.healEvent = healEvent;
            }
        }
    }
}