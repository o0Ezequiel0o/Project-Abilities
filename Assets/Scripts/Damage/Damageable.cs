using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.Collections;
using Zeke.Items;
using static Damageable;

public class Damageable : MonoBehaviour, IUpgradable
{
    [Header("Global Settings")]
    [SerializeField] private DamageableSettings settings;

    [field: Header("Stats")]
    [field: SerializeField] public Stat MaxHealth { get; private set; }
    [field: SerializeField] public Stat MaxShield { get; private set; }
    [field: SerializeField] public Stat HealthRegen { get; private set; }
    [field: SerializeField] public Stat ShieldRegen { get; private set; }
    [field: SerializeField] public Stat Armor { get; private set; }
    [field: Space]
    [field: SerializeField] public Stat DamageReceivedMultiplier { get; private set; }
    [field: SerializeField] public Stat HealingReceivedMultiplier { get; private set; }
    [field: SerializeField] public Stat ShieldReceivedMultiplier { get; private set; }

    public float Health { get; private set; }
    public float Shield { get; private set; }

    public float CombinedHealth => Health + Shield;

    public bool IsAlive { get; private set; }
    public bool MarkedForDeath { get; private set; }

    public OrderedAction<HealEvent> onReceiveHealth = new OrderedAction<HealEvent>();
    public OrderedAction<HealEvent> onReceivedHealth = new OrderedAction<HealEvent>();

    /// <summary> Called when hit, before any condition. </summary>
    public OrderedAction<DamageEvent> onDamageEvent = new OrderedAction<DamageEvent>();
    
    public OrderedAction<DamageEvent> onTakeDamage = new OrderedAction<DamageEvent>();
    public OrderedAction<DamageEvent> onTakenDamage = new OrderedAction<DamageEvent>();

    public OrderedAction<DamageEvent> onHitTaken = new OrderedAction<DamageEvent>();
    public OrderedAction<DamageEvent> onDeath = new OrderedAction<DamageEvent>();

    public Action onAnyHealthUpdate;

    public bool Immune => immunitySources.Count > 0;
    private readonly HashSet<int> immunitySources = new HashSet<int>();

    private float regenTimer = 0f;

    public static float CalculateDamageReduction(float armorValue)
    {
        return CalculateDamageReduction(armorValue, 0f);
    }

    public static float CalculateDamageReduction(float armorValue, float armorPenValue)
    {
        float damageReduction = armorValue / (100 + Mathf.Abs(armorValue));

        if (damageReduction > 0)
        {
            damageReduction *= 1 - armorPenValue;
        }

        return damageReduction;
    }

    public DamageEvent DealDamage(DamageInfo damageInfo)
    {
        return DealDamage(damageInfo, null, null, new List<ItemData>());
    }

    public DamageEvent DealDamage(DamageInfo damageInfo, GameObject sourceUser, GameObject sourceObject)
    {
        return DealDamage(damageInfo, sourceUser, sourceObject, new List<ItemData>());
    }

    public DamageEvent DealDamage(DamageInfo damageInfo, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
    {
        return DealDamage(new DamageEvent(damageInfo, this, sourceUser, sourceObject, procChain));
    }

    public DamageEvent DealDamage(DamageEvent damageEvent)
    {
        damageEvent.ExecuteEventFlow();
        return damageEvent;
    }

    public HealEvent GiveHealing(float healing)
    {
        return GiveHealing(healing, null, null);
    }

    public HealEvent GiveHealing(float healing, GameObject sourceUser, GameObject sourceObject)
    {
        return GiveHealing(new HealEvent(healing, this, sourceUser, sourceObject, new List<ItemData>()));
    }

    public HealEvent GiveHealing(float healing, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
    {
        return GiveHealing(new HealEvent(healing, this, sourceUser, sourceObject, procChain));
    }

    public HealEvent GiveHealing(HealEvent healingEvent)
    {
        healingEvent.ExecuteEventFlow();
        return healingEvent;
    }

    public void GiveShields(float shields, GameObject sourceUser, GameObject sourceObject)
    {
        ReceiveShield(shields);
    }

    public float CalculateDamage(float damage, float armorPenetration)
    {
        float damageReduction = CalculateDamageReduction(Armor.Value, armorPenetration);
        return damage * (1 - damageReduction) * DamageReceivedMultiplier.Value;
    }

    public void AddImmunitySource(int ID)
    {
        immunitySources.Add(ID);
    }

    public void RemoveImmunitySource(int ID)
    {
        immunitySources.Remove(ID);
    }

    public void Upgrade()
    {
        MaxHealth.Upgrade();
        HealthRegen.Upgrade();

        MaxShield.Upgrade();
        ShieldRegen.Upgrade();

        Armor.Upgrade();

        DamageReceivedMultiplier.Upgrade();
        HealingReceivedMultiplier.Upgrade();
        ShieldReceivedMultiplier.Upgrade();
    }

    private float CalculateDamage(DamageEvent damageEvent)
    {
        return CalculateDamage(damageEvent.Damage, damageEvent.ArmorPenetration);
    }

    private float CalculateHealing(HealEvent healingEvent)
    {
        return Mathf.Max(0, healingEvent.Healing * HealingReceivedMultiplier.Value);
    }

    private float TakeDamage(float damage, bool lethal)
    {
        float damageTaken = Mathf.Min(CombinedHealth, damage);

        if (!lethal && damageTaken >= CombinedHealth)
        {
            damageTaken -= 0.01f;
        }

        float overflow = GetOverflow(damageTaken, Shield);

        Shield -= damageTaken - overflow;
        Health -= overflow;

        if (damageTaken > 0)
        {
            onAnyHealthUpdate?.Invoke();
        }

        return damageTaken;
    }

    private float ReceiveHealing(float healing)
    {
        float healthHealed = Mathf.Min(MaxHealth.Value - Health, healing);
        Health += healthHealed;

        if (healthHealed > 0)
        {
            onAnyHealthUpdate?.Invoke();
        }

        return healthHealed;
    }

    private float ReceiveShield(float shields)
    {
        float shieldGained = Mathf.Min(MaxShield.Value - Shield, shields);
        Shield += shieldGained;

        if (shieldGained > 0)
        {
            onAnyHealthUpdate?.Invoke();
        }

        return shieldGained;
    }

    private float GetOverflow(float damage, float health)
    {
        return Mathf.Max(0f, damage - health);
    }

    private void Awake()
    {
        Health = MaxHealth.Value;
        Shield = MaxShield.Value;
        IsAlive = true;

        MarkedForDeath = false;
    }

    private void Update()
    {
        UpdateRegeneration();
    }

    private void UpdateRegeneration()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= settings.RegenInterval)
        {
            if (Health < MaxHealth.Value)
            {
                ReceiveHealing(HealthRegen.Value * settings.RegenInterval);
            }
            else
            {
                ReceiveShield(ShieldRegen.Value * settings.RegenInterval);
            }

            regenTimer = 0f;
        }
    }

    public class DamageEvent
    {
        public static readonly OrderedActionDictionary<GameObject, DamageEvent> onDamageDealt = new OrderedActionDictionary<GameObject, DamageEvent>();
        public static readonly OrderedActionDictionary<GameObject, DamageEvent> onDealDamage = new OrderedActionDictionary<GameObject, DamageEvent>();
        public static readonly OrderedActionDictionary<GameObject, DamageEvent> onKill = new OrderedActionDictionary<GameObject, DamageEvent>();
        public static readonly OrderedActionDictionary<GameObject, DamageEvent> onHit = new OrderedActionDictionary<GameObject, DamageEvent>();

        public float Damage { get; private set; }
        public float BaseDamage { get; private set; }
        public float DamageDealt { get; private set; }
        public float OverflowDamage { get; private set; }

        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        public bool DestroyedShield { get; private set; }
        public bool DeathBlow { get; private set; }
        public bool IsLethal { get; private set; }
        public bool IsHit { get; private set; }

        public Vector2 Direction { get; private set; }

        public float ProcCoefficient { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; }

        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }

        public float damageMultiplier = 1f;
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

        public DamageEvent(DamageInfo damageInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
        {
            IsHit = damageInfo.hit;
            IsLethal = damageInfo.lethal;
            ProcChainBranch = procChain;

            Damage = damageInfo.baseDamage;
            BaseDamage = damageInfo.baseDamage;
            ProcCoefficient = damageInfo.procCoefficient;
            ArmorPenetration = damageInfo.armorPenetration;

            Direction = damageInfo.direction;

            Receiver = receiver;
            SourceUser = userSource;
            SourceObject = sourceObject;
        }

        public void ExecuteEventFlow()
        {
            if (!Receiver.IsAlive || Receiver.MarkedForDeath) return;

            Receiver.onDamageEvent?.Invoke(this);

            HandleImmunityState();

            if (damageRejected) return;

            if (IsHit)
            {
                HandleOnHitEvents();
            }

            if (damageRejected) return;

            HandlePreDamageEvents();

            if (damageRejected || Damage <= 0) return;

            HandleDamage();

            if (DamageDealt <= 0) return;

            if (!Receiver.MarkedForDeath && Receiver.Health <= 0)
            {
                DeathBlow = true;
                Receiver.MarkedForDeath = true;
            }

            HandlePostDamageEvents();

            if (!Receiver.MarkedForDeath || !Receiver.IsAlive) return;

            Receiver.IsAlive = false;
            HandleDeathEvents();
        }

        private void HandleImmunityState()
        {
            if (!Receiver.Immune) return;

            damageRejected = true;
            DamageDealt = 0;
        }

        private void HandleDamage()
        {
            bool hadShield = Receiver.Shield > 0;

            Damage *= damageMultiplier;

            Damage = Receiver.CalculateDamage(this);
            DamageDealt = Receiver.TakeDamage(Damage, IsLethal);

            OverflowDamage = Mathf.Max(0f, Damage - DamageDealt);

            if (hadShield && Receiver.Shield <= 0)
            {
                DestroyedShield = true;
            }
        }

        private void HandlePreDamageEvents()
        {
            if (Damage <= 0) return;
            onDealDamage.Invoke(SourceUser, this);

            if (Damage <= 0) return;
            Receiver.onTakeDamage?.Invoke(this);
        }

        private void HandlePostDamageEvents()
        {
            Receiver.onTakenDamage?.Invoke(this);
            onDamageDealt.Invoke(SourceUser, this);
        }

        private void HandleOnHitEvents()
        {
            Receiver.onHitTaken?.Invoke(this);
            onHit.Invoke(SourceUser, this);
        }

        private void HandleDeathEvents()
        {
            Receiver.onDeath?.Invoke(this);
            onKill.Invoke(SourceUser, this);
        }
    }

    public class HealEvent
    {
        public static readonly ActionDictionary<GameObject, HealEvent> onHealHealth = new ActionDictionary<GameObject, HealEvent>();
        public static readonly ActionDictionary<GameObject, HealEvent> onHealthHealed = new ActionDictionary<GameObject, HealEvent>();

        public float Healing { get; private set; }
        public float BaseHealing {  get; private set; }
        public float HealthHealed { get; private set; }
        public float OverflowHealing { get; private set; }

        public float UncappedHealing => Healing + OverflowHealing;

        public float ProcCoefficient { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; }

        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }

        public float healingMultiplier = 1f;

        public HealEvent(float healing, Damageable receiver, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
        {
            Healing = healing;
            BaseHealing = Healing;

            Receiver = receiver;
            SourceUser = sourceUser;
            SourceObject = sourceObject;

            ProcChainBranch = procChain;
        }

        public void ExecuteEventFlow()
        {
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
            Healing *= healingMultiplier;

            Healing = Receiver.CalculateHealing(this);
            HealthHealed = Receiver.ReceiveHealing(Healing);

            OverflowHealing = Mathf.Max(0f, Healing - HealthHealed);
        }

        private void HandlePreHealingEvents()
        {
            onHealHealth.Invoke(SourceUser, this);
            Receiver.onReceiveHealth?.Invoke(this);
        }

        private void HandleHealingEvents()
        {
            Receiver.onReceivedHealth?.Invoke(this);
            onHealthHealed.Invoke(SourceUser, this);
        }
    }
}