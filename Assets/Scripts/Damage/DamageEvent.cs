using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public class DamageEvent
    {
        public float Damage { get; private set; }
        public float BaseDamage { get; private set; }
        public float DamageDealt { get; private set; }
        public float OverflowDamage { get; private set; }

        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        public bool DestroyedShield { get; private set; }
        public bool DeathBlow { get; private set; }

        public bool IgnoresShield { get; private set; }
        public bool IsLethal { get; private set; }
        public bool IsHit { get; private set; }

        public Vector2 Direction { get; private set; }

        public float ProcCoefficient { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; }

        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }

        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; private set; }

        public bool damageRejected = false;

        private float armorPenetration;
        public float ArmorPenetration
        {
            get
            {
                return Mathf.Clamp(armorPenetration, -1, 1);
            }
            set
            {
                armorPenetration = value;
            }
        }

        private DamageEventListener damageEventListener;
        private bool isDamageEventListener = false;

        public DamageEvent(DamageInfo damageInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
        {
            IsHit = damageInfo.hit;
            IsLethal = damageInfo.lethal;
            IgnoresShield = damageInfo.ignoresShield;

            ProcChainBranch = procChain;

            Damage = damageInfo.baseDamage;
            BaseDamage = damageInfo.baseDamage;
            ProcCoefficient = damageInfo.procCoefficient;
            ArmorPenetration = damageInfo.armorPenetration;

            Direction = damageInfo.direction;

            Receiver = receiver;
            SourceUser = userSource;
            SourceObject = sourceObject;

            Multiplier = new Stat(1f, 0f, 0f, float.PositiveInfinity);
        }

        public void ExecuteEventFlow()
        {
            isDamageEventListener = SourceUser.TryGetComponent(out damageEventListener);

            if (!Receiver.IsAlive || Receiver.MarkedForDeath)
            {
                EndDamageEvent();
                return;
            }

            Receiver.onDamageEvent?.Invoke(this);

            HandleImmunityState();

            if (damageRejected)
            {
                EndDamageEvent();
                return;
            }

            if (IsHit)
            {
                HandleOnHitEvents();
            }

            if (damageRejected)
            {
                EndDamageEvent();
                return;
            }

            HandlePreDamageEvents();

            if (damageRejected || Damage <= 0)
            {
                EndDamageEvent();
                return;
            }

            HandleDamage();

            if (DamageDealt <= 0)
            {
                EndDamageEvent();
                return;
            }

            if (!Receiver.MarkedForDeath && Receiver.Health <= 0)
            {
                DeathBlow = true;
                Receiver.MarkedForDeath = true;
            }

            HandlePostDamageEvents();

            if (!Receiver.MarkedForDeath || !Receiver.IsAlive)
            {
                EndDamageEvent();
                return;
            }

            Receiver.IsAlive = false;
            HandleDeathEvents();
        }

        private void EndDamageEvent() { }

        private void HandleImmunityState()
        {
            if (!Receiver.Immune) return;

            damageRejected = true;
            DamageDealt = 0;
        }

        private void HandleDamage()
        {
            bool hadShield = Receiver.Shield > 0;

            Damage = Receiver.CalculateDamage(this);
            DamageDealt = Receiver.TakeDamage(Damage, IsLethal, IgnoresShield);

            OverflowDamage = Mathf.Max(0f, Damage - DamageDealt);

            if (hadShield && Receiver.Shield <= 0)
            {
                DestroyedShield = true;
            }
        }

        private void HandlePreDamageEvents()
        {
            if (Damage <= 0) return;

            if (isDamageEventListener)
            {
                damageEventListener.CallDealDamageEvent(new OnDealDamageEvent(this));
            }

            if (Damage <= 0) return;

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