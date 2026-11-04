using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.Collections;
using Zeke.Items;

using static Stat;

public partial class Damageable : MonoBehaviour, IUpgradable
{
    [field: Header("Global Settings")]
    [field: SerializeField] public DamageableSettings Settings { get; private set; }

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

    public float Health
    {
        get => _health;
        set => _health = Mathf.Clamp(value, 0f, MaxHealth.Value);
    }

    public float Shield
    {
        get => _shield;
        set => _shield = Mathf.Clamp(value, 0f, MaxShield.Value);
    }

    public float CombinedHealth => Health + Shield;

    public bool IsAlive { get; private set; }
    public bool MarkedForDeath { get; private set; }

    public readonly OrderedAction<HealEvent> onReceiveHealth = new OrderedAction<HealEvent>();
    public readonly OrderedAction<HealEvent> onReceivedHealth = new OrderedAction<HealEvent>();

    /// <summary> Called when hit, before any condition. </summary>
    public readonly OrderedAction<DamageEvent> onDamageEvent = new OrderedAction<DamageEvent>();
    
    public readonly OrderedAction<DamageEvent> onTakeDamage = new OrderedAction<DamageEvent>();
    public readonly OrderedAction<DamageEvent> onTakenDamage = new OrderedAction<DamageEvent>();

    public readonly OrderedAction<DamageEvent> onHitTaken = new OrderedAction<DamageEvent>();
    public readonly OrderedAction<DamageEvent> onDeath = new OrderedAction<DamageEvent>();

    public Action<Damageable> onDespawn;
    public Action onAnyHealthUpdate;

    public bool Immune => immunitySources.Count > 0;

    private readonly HashSet<ImmunitySourceID> immunitySources = new HashSet<ImmunitySourceID>();

    private float _health = 0f;
    private float _shield = 0f;

    private float regenTimer = 0f;

    public class ImmunitySourceID { }

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

    public HealEvent GiveHealing(HealInfo healInfo)
    {
        return GiveHealing(healInfo, null, null);
    }

    public HealEvent GiveHealing(HealInfo healInfo, GameObject sourceUser, GameObject sourceObject)
    {
        return GiveHealing(new HealEvent(healInfo, this, sourceUser, sourceObject, new List<ItemData>()));
    }

    public HealEvent GiveHealing(HealInfo healInfo, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
    {
        return GiveHealing(new HealEvent(healInfo, this, sourceUser, sourceObject, procChain));
    }

    public HealEvent GiveHealing(HealEvent healingEvent)
    {
        healingEvent.ExecuteEventFlow();
        return healingEvent;
    }

    public void GiveShield(float shields, GameObject sourceUser, GameObject sourceObject)
    {
        ReceiveShield(shields);
    }

    public float CalculateDamage(float damage, float armorPenetration)
    {
        float damageReduction = CalculateDamageReduction(Armor.Value, armorPenetration);
        return damage * (1 - damageReduction) * DamageReceivedMultiplier.Value;
    }

    public float CalculateHealing(float healing)
    {
        return healing * HealingReceivedMultiplier.Value;
    }

    public void AddImmunitySource(ImmunitySourceID id)
    {
        immunitySources.Add(id);
    }

    public void RemoveImmunitySource(ImmunitySourceID id)
    {
        immunitySources.Remove(id);
    }

    public void Upgrade(int levels)
    {
        MaxHealth.Upgrade(levels);
        HealthRegen.Upgrade(levels);

        MaxShield.Upgrade(levels);
        ShieldRegen.Upgrade(levels);

        Armor.Upgrade(levels);

        DamageReceivedMultiplier.Upgrade(levels);
        HealingReceivedMultiplier.Upgrade(levels);
        ShieldReceivedMultiplier.Upgrade(levels);
    }

    private float CalculateDamage(DamageEvent damageEvent)
    {
        float damage = Mathf.Max(0, (damageEvent.Damage + damageEvent.FlatAccumulator) * damageEvent.Multiplier.Value);
        return CalculateDamage(damage, damageEvent.ArmorPenetration);
    }

    private float CalculateHealing(HealEvent healingEvent)
    {
        float healing = Mathf.Max(0, (healingEvent.Healing + healingEvent.FlatAccumulator) * healingEvent.Multiplier.Value);
        return CalculateHealing(healing);
    }

    private float TakeDamage(float damage, bool lethal, bool ignoresShield)
    {
        float damageTaken;

        if (!ignoresShield)
        {
            damageTaken = Mathf.Min(CombinedHealth, damage);

            if (!lethal && damageTaken >= CombinedHealth)
            {
                damageTaken *= 0.999f;
            }

            float overflow = GetOverflow(damageTaken, Shield);

            Shield -= damageTaken - overflow;
            Health -= overflow;
        }
        else
        {
            damageTaken = Mathf.Min(Health, damage);

            if (!lethal && damageTaken >= Health)
            {
                damageTaken *= 0.999f;
            }

            Health -= damageTaken;
        }

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
        MaxHealth.onValueChanged += SyncHealth;
        MaxShield.onValueChanged += SyncShield;
    }

    private void Update()
    {
        UpdateRegeneration();
    }

    private void OnDestroy()
    {
        onDespawn?.Invoke(this);
    }

    private void SyncHealth(StatUpdate statUpdate)
    {
        float healthRatio = Mathf.Clamp01(Health / statUpdate.oldValue);
        float change = MaxHealth.Value - statUpdate.oldValue;

        if (change == 0f) return;

        if (change > 0f)
        {
            Health += change * healthRatio;
        }
        else if (change < 0f && _health > MaxHealth.Value)
        {
            _health = MaxHealth.Value;
        }

        onAnyHealthUpdate?.Invoke();
    }

    private void SyncShield(StatUpdate statUpdate)
    {
        float change = MaxHealth.Value - statUpdate.oldValue;

        if (change == 0f) return;

        onAnyHealthUpdate?.Invoke();
    }

    private void UpdateRegeneration()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= Settings.RegenInterval)
        {
            if (Health < MaxHealth.Value)
            {
                Health += HealthRegen.Value * regenTimer;
            }
            else
            {
                Shield += ShieldRegen.Value * regenTimer;
            }

            onAnyHealthUpdate?.Invoke();
            regenTimer = 0f;
        }
    }
}