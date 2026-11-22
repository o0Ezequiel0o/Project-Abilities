using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public class DamageEvent
    {
        // Input & Context
        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }
        public Vector2 Direction { get; private set; }
        public float ProcCoefficient { get; private set; }
        public bool IsHit { get; private set; }
        public bool IsLethal { get; private set; }
        public bool IgnoresShield { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; } = new List<ItemData>();

        // Damage & Modifiers
        public float BaseDamage { get; private set; }
        public float Damage { get; private set; }
        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

        private float armorPenetration;
        public float ArmorPenetration
        {
            get => Mathf.Clamp(armorPenetration, -1f, 1f);
            set => armorPenetration = value;
        }

        // Results & State
        public float HealthDamageDealt { get; private set; }
        public float ShieldDamageDealt { get; private set; }
        public float OverflowDamage { get; private set; }
        public float DamageDealt => HealthDamageDealt + ShieldDamageDealt;
        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        public bool DestroyedShield { get; private set; }
        public bool DeathBlow { get; private set; }
        public bool IsRejected { get; set; }

        private DamageableEventListener damageEventListener;
        private bool isDamageEventListener = false;

        public DamageEvent() { }

        public DamageEvent Initialize(DamageInfo damageInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
        {
            IsHit = damageInfo.hit;
            IsLethal = damageInfo.lethal;
            IgnoresShield = damageInfo.ignoresShield;

            ProcChainBranch.Clear();
            ProcChainBranch.AddRange(procChain);

            Damage = damageInfo.baseDamage;
            BaseDamage = damageInfo.baseDamage;
            ProcCoefficient = damageInfo.procCoefficient;
            ArmorPenetration = damageInfo.armorPenetration;

            Direction = damageInfo.direction;

            Receiver = receiver;
            SourceUser = userSource;
            SourceObject = sourceObject;

            OverflowDamage = 0f;
            HealthDamageDealt = 0f;
            ShieldDamageDealt = 0f;

            DestroyedShield = false;
            IsRejected = false;
            DeathBlow = false;

            FlatAccumulator = 0f;

            damageEventListener = null;
            isDamageEventListener = false;

            Multiplier.Clear();

            return this;
        }

        public void Execute()
        {
            try
            {
                isDamageEventListener = SourceUser != null && SourceUser.TryGetComponent(out damageEventListener);

                if (Receiver == null || !Receiver.IsAlive || Receiver.MarkedForDeath) return;

                Receiver.onDamageEvent?.Invoke(this);

                if (Receiver.Immune) HandleImmunityState();

                if (IsRejected) return;

                if (IsHit) HandleOnHitEvents();

                if (IsRejected) return;

                HandlePreDamageEvents();

                if (IsRejected || Damage <= 0f) return;

                HandleDamage();

                if (DamageDealt <= 0f) return;

                if (!Receiver.MarkedForDeath && Receiver.Health <= 0f)
                {
                    DeathBlow = true;
                    Receiver.MarkedForDeath = true;
                }

                HandlePostDamageEvents();

                if (!Receiver.MarkedForDeath || !Receiver.IsAlive) return;

                Receiver.IsAlive = false;
                HandleDeathEvents();
            }
            finally
            {
                if (!DamageableEventPool.IsNull)
                {
                    DamageableEventPool.Instance.ReturnDamage(this);
                }
            }
        }

        private void HandleImmunityState()
        {
            IsRejected = true;
            HealthDamageDealt = 0f;
            ShieldDamageDealt = 0f;
        }

        private void HandleDamage()
        {
            bool hadShield = Receiver.Shield > 0f;

            Damage = Receiver.CalculateDamage(this);
            DamageTakenInfo damageTakenInfo = Receiver.TakeDamage(Damage, IsLethal, IgnoresShield);

            HealthDamageDealt = damageTakenInfo.healthDamageTaken;
            ShieldDamageDealt = damageTakenInfo.shieldDamageTaken;

            OverflowDamage = Mathf.Max(0f, Damage - DamageDealt);

            if (hadShield && Receiver.Shield <= 0f)
            {
                DestroyedShield = true;
            }
        }

        private void HandlePreDamageEvents()
        {
            if (Damage <= 0f) return;

            if (isDamageEventListener)
            {
                damageEventListener.CallDealDamageEvent(new OnDealDamageEvent(this));
            }

            if (Damage <= 0f) return;

            Receiver.onTakeDamage?.Invoke(this);
        }

        private void HandlePostDamageEvents()
        {
            Receiver.onTakenDamage?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallDealtDamageEvent(new OnDealtDamageEvent(this));
            }
        }

        private void HandleOnHitEvents()
        {
            Receiver.onHitTaken?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallHitEvent(new OnHitEvent(this));
            }
        }

        private void HandleDeathEvents()
        {
            Receiver.onDeath?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallKillEvent(new OnKillEvent(this));
            }
        }

        public readonly struct OnDealtDamageEvent
        {
            public readonly DamageEvent damageEvent;

            public OnDealtDamageEvent(DamageEvent damageEvent)
            {
                this.damageEvent = damageEvent;
            }
        }

        public readonly struct OnDealDamageEvent
        {
            public readonly DamageEvent damageEvent;

            public OnDealDamageEvent(DamageEvent damageEvent)
            {
                this.damageEvent = damageEvent;
            }
        }

        public readonly struct OnKillEvent
        {
            public readonly DamageEvent damageEvent;

            public OnKillEvent(DamageEvent damageEvent)
            {
                this.damageEvent = damageEvent;
            }
        }

        public readonly struct OnHitEvent
        {
            public readonly DamageEvent damageEvent;

            public OnHitEvent(DamageEvent damageEvent)
            {
                this.damageEvent = damageEvent;
            }
        }
    }
}