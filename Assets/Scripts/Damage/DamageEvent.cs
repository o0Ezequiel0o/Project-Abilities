using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public class DamageEvent : IDamageEvent, IDealtDamageEvent, IDealDamageEvent, IKillEvent, IHitDamageEvent
    {
        // Context
        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }
        public Vector2 Direction { get; private set; }
        public float ProcCoefficient { get; private set; }

        // Flags
        public bool IsHit { get; private set; }
        public bool IsLethal { get; private set; }
        public bool IgnoresShield { get; private set; }
        public bool IsRejected { get; set; }

        // Data
        public List<ItemData> ProcChainBranch { get; private set; } = new List<ItemData>();

        // Damage
        public float BaseDamage { get; private set; }
        public float Damage { get; private set; }

        public float DamageDealt { get; private set; }

        //Modifiers
        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

        public float ArmorPenetration
        {
            get => Mathf.Clamp(armorPenetration, -1f, 1f);
            set => armorPenetration = value;
        }

        // Output
        public bool DestroyedShield { get; private set; }
        public bool DeathBlow { get; private set; }

        private float armorPenetration = 0f;

        private DamageableEventListener damageEventListener;
        private bool isDamageEventListener = false;

        private readonly List<HealthDamageTaken> healthDamageCache = new List<HealthDamageTaken>();

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
                if (Receiver == null || !Receiver.IsAlive || Receiver.MarkedForDeath) return;

                isDamageEventListener = SourceUser != null && SourceUser.TryGetComponent(out damageEventListener);      

                Receiver.onDamageEvent?.Invoke(this);

                if (Receiver.Immune) HandleImmunityState();

                if (IsRejected) return;

                if (IsHit) HandleOnHitEvents();

                if (IsRejected) return;

                HandlePreDamageEvents();

                if (IsRejected || Damage <= 0f) return;

                DamageDealt = HandleDamage();

                if (DamageDealt <= 0f) return;

                Receiver.TryGetHealthType(Receiver.HealthType, out HealthType life);

                if (!Receiver.MarkedForDeath && life.Value <= 0f)
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

        public DamageEventSnapshot GetSnapshot()
        {
            return new DamageEventSnapshot(Receiver, SourceUser, SourceObject, Direction, ProcCoefficient, IsHit, IsLethal, IgnoresShield, IsRejected,
                new List<ItemData>(ProcChainBranch), BaseDamage, Damage, FlatAccumulator, Multiplier.Value, ArmorPenetration, DestroyedShield, DeathBlow, DamageDealt);
        }

        private void HandleImmunityState()
        {
            IsRejected = true;
        }

        private float HandleDamage()
        {
            Damage = Receiver.CalculateDamage(this);

            healthDamageCache.Clear();
            DamageTakenInfo damageTakenInfo = Receiver.TakeDamage(healthDamageCache, Damage, IsLethal);

            return damageTakenInfo.totalDamage;
        }

        private void HandlePreDamageEvents()
        {
            if (Damage <= 0f) return;

            if (isDamageEventListener)
            {
                damageEventListener.CallDealDamageEvent(this);
            }

            if (Damage <= 0f) return;

            Receiver.onTakeDamage?.Invoke(this);
        }

        private void HandlePostDamageEvents()
        {
            Receiver.onTakenDamage?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallDealtDamageEvent(this);
            }
        }

        private void HandleOnHitEvents()
        {
            Receiver.onHitTaken?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallHitEvent(this);
            }
        }

        private void HandleDeathEvents()
        {
            Receiver.onDeath?.Invoke(this);

            if (isDamageEventListener)
            {
                damageEventListener.CallKillEvent(this);
            }
        }
    }

    public readonly struct DamageEventSnapshot
    {
        // Context
        public readonly Damageable receiver;
        public readonly GameObject sourceUser;
        public readonly GameObject sourceObject;
        public readonly Vector2 direction;
        public readonly float procCoefficient;

        // Flags
        public readonly bool isHit;
        public readonly bool isLethal;
        public readonly bool ignoresShield;
        public readonly bool isRejected;

        // Data
        public readonly List<ItemData> procChainBranch;

        // Damage
        public readonly float baseDamage;
        public readonly float damage;

        // Modifiers
        public readonly float flatAccumulator;
        public readonly float multiplier;
        public readonly float armorPenetration;

        // Results & State
        public readonly float overflowDamage;
        public readonly float damageDealt;
        public readonly float uncappedDamageDealt;

        // Output
        public readonly bool destroyedShield;
        public readonly bool deathBlow;

        public DamageEventSnapshot(Damageable receiver, GameObject sourceUser, GameObject sourceObject, Vector2 direction, float procCoefficient, bool isHit, 
            bool isLethal, bool ignoresShield, bool isRejected, List<ItemData> procChainBranch, float baseDamage, float damage, float flatAccumulator, 
            float multiplier, float armorPenetration, bool destroyedShield, bool deathBlow, float damageDealt)
        {
            this.receiver = receiver;
            this.sourceUser = sourceUser;
            this.sourceObject = sourceObject;
            this.direction = direction;
            this.procCoefficient = procCoefficient;
            this.isHit = isHit;
            this.isLethal = isLethal;
            this.ignoresShield = ignoresShield;
            this.isRejected = isRejected;
            this.procChainBranch = procChainBranch;
            this.baseDamage = baseDamage;
            this.damage = damage;
            this.flatAccumulator = flatAccumulator;
            this.multiplier = multiplier;
            this.armorPenetration = armorPenetration;
            this.destroyedShield = destroyedShield;
            this.deathBlow = deathBlow;
            this.damageDealt = damageDealt;

            overflowDamage = Mathf.Max(0f, damage - damageDealt);
            uncappedDamageDealt = damageDealt + overflowDamage;
        }
    }
}