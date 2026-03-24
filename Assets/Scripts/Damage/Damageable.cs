using System.Collections.Generic;
using UnityEngine;
using System;

public class Damageable : MonoBehaviour, IUpgradable
{
    [field: Header("Stats")]
    [field: SerializeField] public Stat MaxHealth { get; private set; }
    [field: SerializeField] public Stat HealthRegen { get; private set; }
    [field: SerializeField] public Stat Armor { get; private set; }
    [field: Space]
    [field: SerializeField] public Stat DamageReceivedMultiplier { get; private set; }
    [field: SerializeField] public Stat HealingReceivedMultiplier { get; private set; }

    public float Health { get; private set; }
    public bool IsAlive { get; private set; }

    public Action<HealEvent> onReceiveHealth;
    public Action<HealEvent> onHealthReceived;

    /// <summary> Called when hit, before any condition. </summary>
    public Action<DamageEvent> onTakeHitAny;
    
    public Action<DamageEvent> onTakeDamage;
    public Action<DamageEvent> onDamageTaken;

    public Action<DamageEvent> onHitTaken;
    public Action<DamageEvent> onDeath;

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

    public void AddImmunitySource(int ID)
    {
        immunitySources.Add(ID);
    }

    public void RemoveImmunitySource(int ID)
    {
        immunitySources.Remove(ID);
    }

    private float CalculateDamage(DamageEvent damageEvent)
    {
        float damageReduction = CalculateDamageReduction(Armor.Value, damageEvent.ArmorPenetration);
        return damageEvent.Damage * (1 - damageReduction) * DamageReceivedMultiplier.Value;
    }

    private float CalculateHealing(HealEvent healingEvent)
    {
        return Mathf.Max(0, healingEvent.Healing * HealingReceivedMultiplier.Value);
    }

    private float TakeDamage(float damage, bool lethal)
    {
        float healthLost = Mathf.Min(Health, damage);

        if (!lethal && healthLost >= Health)
        {
            healthLost -= 0.01f;
        }

        Health -= healthLost;

        return healthLost;
    }

    private float ReceiveHealing(HealEvent healingEvent)
    {
        float healthHealed = Mathf.Min(MaxHealth.Value - Health, healingEvent.Healing);
        Health += healthHealed;

        return healthHealed;
    }

    public void Upgrade()
    {
        MaxHealth.Upgrade();
        HealthRegen.Upgrade();

        Armor.Upgrade();

        DamageReceivedMultiplier.Upgrade();
        HealingReceivedMultiplier.Upgrade();
    }

    private void Awake()
    {
        Health = MaxHealth.Value;
        IsAlive = true;
    }

    private void Update()
    {
        if (Health == MaxHealth.Value) return;
        UpdateHealthRegeneration();
    }

    private void UpdateHealthRegeneration()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= 1f)
        {
            GiveHealing(HealthRegen.Value, gameObject, gameObject);
            regenTimer = 0f;
        }
    }

    public class DamageEvent
    {
        public static readonly ActionDictionary<GameObject, DamageEvent> onDamageDealt = new ActionDictionary<GameObject, DamageEvent>();
        public static readonly ActionDictionary<GameObject, DamageEvent> onDealDamage = new ActionDictionary<GameObject, DamageEvent>();
        public static readonly ActionDictionary<GameObject, DamageEvent> onKill = new ActionDictionary<GameObject, DamageEvent>();
        public static readonly ActionDictionary<GameObject, DamageEvent> onHit = new ActionDictionary<GameObject, DamageEvent>();

        public float Damage { get; private set; }
        public float BaseDamage { get; private set; }
        public float DamageDealt { get; private set; }
        public float OverflowDamage { get; private set; }

        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        public bool IsLethal { get; private set; }

        public float ProcCoefficient { get; private set; }
        public List<ItemData> ProcChainBranch { get; private set; }

        public Damageable Receiver { get; private set; }
        public GameObject SourceUser { get; private set; }
        public GameObject SourceObject { get; private set; }

        public float damageMultiplier = 1f;

        public bool damageRejected = false;
        public bool deathBlow = false;

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
            IsLethal = damageInfo.lethal;
            ProcChainBranch = procChain;

            Damage = damageInfo.baseDamage;
            BaseDamage = damageInfo.baseDamage;
            ProcCoefficient = damageInfo.procCoefficient;
            ArmorPenetration = damageInfo.armorPenetration;

            Receiver = receiver;
            SourceUser = userSource;
            SourceObject = sourceObject;
        }

        public void ExecuteEventFlow()
        {
            if (!Receiver.IsAlive) return;

            HandleHitAnyEvents();
            HandleImmunityState();

            if (damageRejected) return;

            HandleOnHitEvents();

            if (damageRejected) return;

            HandlePreDamageEvents();

            if (damageRejected || Damage <= 0) return;

            HandleDamage();

            if (DamageDealt <= 0) return;

            if (Receiver.IsAlive && Receiver.Health <= 0)
            {
                deathBlow = true;
                Receiver.IsAlive = false;
            }

            HandlePostDamageEvents();

            if (!deathBlow) return;

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
            Damage *= damageMultiplier;

            Damage = Receiver.CalculateDamage(this);
            DamageDealt = Receiver.TakeDamage(Damage, IsLethal);

            OverflowDamage = Mathf.Max(0f, Damage - DamageDealt);
        }

        private void HandleHitAnyEvents()
        {
            Receiver.onTakeHitAny?.Invoke(this);
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
            Receiver.onDamageTaken?.Invoke(this);
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
            HealthHealed = Receiver.ReceiveHealing(this);

            OverflowHealing = Mathf.Max(0f, Healing - HealthHealed);
        }

        private void HandlePreHealingEvents()
        {
            onHealHealth.Invoke(SourceUser, this);
            Receiver.onReceiveHealth?.Invoke(this);
        }

        private void HandleHealingEvents()
        {
            Receiver.onHealthReceived?.Invoke(this);
            onHealthHealed.Invoke(SourceUser, this);
        }
    }
}