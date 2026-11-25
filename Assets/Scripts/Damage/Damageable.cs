using System.Collections.Generic;
using UnityEngine;
using Zeke.Collections;
using Zeke.Items;
using System;

public partial class Damageable : MonoBehaviour, IUpgradable
{
    [field: Header("Settings")]
    [field: SerializeField] public HealthTypeData HealthType { get; private set; }
    [field: SerializeField] public HealthTypeData ShieldTempPatch { get; private set; }

    [field: Header("Stats")]
    [field: SerializeField] public Stat Armor { get; private set; }
    [field: SerializeField] public Stat DamageReceivedMultiplier { get; private set; }
    [SerializeField] private List<HealthType> healthTypes;

    public float CombinedHealth
    {
        get
        {
            float total = 0f;

            for (int i = 0; i < healthTypes.Count; i++)
            {
                total += healthTypes[i].Value;
            }

            return total;
        }
    }

    public float Health
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == HealthType)
                {
                    return healthTypes[i].Value;
                }
            }

            return 0f;
        }
    }

    public float Shield
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == ShieldTempPatch)
                {
                    return healthTypes[i].Value;
                }
            }

            return 0f;
        }
    }

    public Stat MaxHealth
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == HealthType)
                {
                    return healthTypes[i].Max;
                }
            }

            return null;
        }
    }

    public Stat MaxShield
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == ShieldTempPatch)
                {
                    return healthTypes[i].Max;
                }
            }

            return null;
        }
    }

    public Stat HealthRegen
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == HealthType)
                {
                    return healthTypes[i].Regen;
                }
            }

            return null;
        }
    }

    public Stat ShieldRegen
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == ShieldTempPatch)
                {
                    return healthTypes[i].Regen;
                }
            }

            return null;
        }
    }

    public Stat HealingReceivedMultiplier
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == HealthType)
                {
                    return healthTypes[i].ReceivedMultiplier;
                }
            }

            return null;
        }
    }

    public Stat ShieldReceivedMultiplier
    {
        get
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Type == ShieldTempPatch)
                {
                    return healthTypes[i].ReceivedMultiplier;
                }
            }

            return null;
        }
    }

    public bool IsAlive { get; private set; }
    public bool MarkedForDeath { get; private set; }

    /// <summary> Called when hit, before any condition. </summary>
    public readonly OrderedAction<DamageEvent> onDamageEvent = new OrderedAction<DamageEvent>();
    /// <summary> Called when healed, before any condition. </summary>
    public readonly OrderedAction<HealEvent> onHealEvent = new OrderedAction<HealEvent>();

    public readonly OrderedAction<IDealDamageEvent> onTakeDamage = new OrderedAction<IDealDamageEvent>();
    public readonly OrderedAction<IDealtDamageEvent> onTakenDamage = new OrderedAction<IDealtDamageEvent>();
    public readonly OrderedAction<IHealHealthEvent> onReceiveHealth = new OrderedAction<IHealHealthEvent>();
    public readonly OrderedAction<IHealedHealthEvent> onReceivedHealth = new OrderedAction<IHealedHealthEvent>();

    public readonly OrderedAction<IHitDamageEvent> onHitTaken = new OrderedAction<IHitDamageEvent>();
    public readonly OrderedAction<IHitHealEvent> onHealHitTaken = new OrderedAction<IHitHealEvent>();

    public readonly OrderedAction<IKillEvent> onDeath = new OrderedAction<IKillEvent>();

    public Action<Damageable> onDespawn;
    public Action onAnyHealthUpdate;

    public bool Immune => immunitySources.Count > 0;

    private readonly HashSet<ImmunitySourceID> immunitySources = new HashSet<ImmunitySourceID>();

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

    public bool TryGetHealthType(HealthTypeData healthType, out HealthType health)
    {
        health = null;

        for (int i = 0; i < healthTypes.Count; i++)
        {
            if (healthTypes[i].Type == healthType)
            {
                health = healthTypes[i];
                return true;
            }
        }

        return false;
    }

    public IDamageEvent DealDamage(DamageInfo damageInfo)
    {
        return DealDamage(DamageableEventPool.Instance.GetDamage(damageInfo, this, null, null, new List<ItemData>()));
    }

    public IDamageEvent DealDamage(DamageInfo damageInfo, GameObject sourceUser, GameObject sourceObject)
    {
        return DealDamage(DamageableEventPool.Instance.GetDamage(damageInfo, this, sourceUser, sourceObject, new List<ItemData>()));
    }

    public IDamageEvent DealDamage(DamageInfo damageInfo, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
    {
        return DealDamage(DamageableEventPool.Instance.GetDamage(damageInfo, this, sourceUser, sourceObject, procChain));
    }

    public IDamageEvent DealDamage(DamageEvent damageEvent)
    {
        damageEvent.Execute();
        return damageEvent;
    }

    public IHealEvent GiveHealing(HealInfo healInfo)
    {
        return GiveHealing(DamageableEventPool.Instance.GetHeal(healInfo, null, null, null, new List<ItemData>()));
    }

    public IHealEvent GiveHealing(HealInfo healInfo, GameObject sourceUser, GameObject sourceObject)
    {
        return GiveHealing(DamageableEventPool.Instance.GetHeal(healInfo, this, sourceUser, sourceObject, new List<ItemData>()));
    }

    public IHealEvent GiveHealing(HealInfo healInfo, GameObject sourceUser, GameObject sourceObject, List<ItemData> procChain)
    {
        return GiveHealing(DamageableEventPool.Instance.GetHeal(healInfo, this, sourceUser, sourceObject, procChain));
    }

    public IHealEvent GiveHealing(HealEvent HealEvent)
    {
        HealEvent.Execute();
        return HealEvent;
    }

    public void AddHealthType(HealthTypeData healthType, Stat max, Stat regen, Stat receivedMultiplier)
    {
        healthTypes.Add(new HealthType().Initialize(healthType, max, regen, receivedMultiplier));
        healthTypes.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public float CalculateDamage(float damage, float armorPenetration)
    {
        float damageReduction = CalculateDamageReduction(Armor.Value, armorPenetration);
        return damage * (1 - damageReduction) * DamageReceivedMultiplier.Value;
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
        for (int i = 0; i < healthTypes.Count; i++)
        {
            healthTypes[i].Upgrade(levels);
        }

        Armor.Upgrade(levels);
        DamageReceivedMultiplier.Upgrade(levels);
    }

    //FIX
    private float CalculateHealing(HealEvent healEvent)
    {
        return 0f;
    }

    private float CalculateDamage(DamageEvent damageEvent)
    {
        float damage = Mathf.Max(0f, (damageEvent.Damage + damageEvent.FlatAccumulator) * damageEvent.Multiplier.Value);
        return CalculateDamage(damage, damageEvent.ArmorPenetration);
    }

    //Make overload for filtered one that takes a specific health type
    private DamageTakenInfo TakeDamage(List<HealthDamageTaken> healthDamage, float damage, bool lethal)
    {
        damage = Mathf.Min(CombinedHealth, damage);

        float currentDamage = damage;
        float totalDamage = 0f;

        if (damage > 0)
        {
            for (int i = 0; i < healthTypes.Count; i++)
            {
                if (healthTypes[i].Value <= 0f) continue;

                //If the damage is bigger than our current health type then we get the overflow and continue iterating
                if (currentDamage >= healthTypes[i].Value)
                {
                    //If that health is our life and not lethal then we reduce it enough to not kill us.
                    if (healthTypes[i].Type == HealthType && !lethal)
                    {
                        currentDamage = healthTypes[i].Value * 0.999f;
                        totalDamage += currentDamage;
                        Debug.Log($"index at: {i}, reached life and isn't lethal, we stop here.");
                        healthDamage.Add(new HealthDamageTaken(healthTypes[i].Type, currentDamage, false));

                        break;
                    }
                    else
                    {
                        healthDamage.Add(new HealthDamageTaken(healthTypes[i].Type, healthTypes[i].Value, true));
                        currentDamage = GetOverflow(damage, healthTypes[i].Value);
                        totalDamage += currentDamage;
                        healthTypes[i].Value = 0f;
                    }
                }
                else //If the damage is not bigger than our current health type then we stop looping and log the current damage taken of that bar.
                {
                    healthDamage.Add(new HealthDamageTaken(healthTypes[i].Type, currentDamage, false));
                    totalDamage += currentDamage;
                    currentDamage = 0f;
                    break;
                }
            }

            onAnyHealthUpdate?.Invoke();
        }

        return new DamageTakenInfo(totalDamage, currentDamage);
    }

    //FIX
    private float ReceiveHealing(float healing)
    {
        //float healthHealed = Mathf.Min(MaxHealth.Value - healthTypes, healing);
        //healthTypes += healthHealed;

        //if (healthHealed > 0)
        //{
        //    onAnyHealthUpdate?.Invoke();
        //}

        //return healthHealed;

        return 0f;
    }

    private float GetOverflow(float damage, float health)
    {
        return Mathf.Max(0f, damage - health);
    }

    private void Awake()
    {
        IsAlive = true;
        MarkedForDeath = false;

        healthTypes.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        for (int i = 0; i < healthTypes.Count; i++)
        {
            healthTypes[i].SetSource(this);
            Debug.Log($"{healthTypes[i].Type.name}: {healthTypes[i].Value} / {healthTypes[i].Max.Value}");
        }
    }

    private void Update()
    {
        for (int i = 0; i < healthTypes.Count; i++)
        {
            healthTypes[i].Update();
        }
    }

    private void OnDestroy()
    {
        onDespawn?.Invoke(this);
    }

    public readonly struct DamageTakenInfo
    {
        public readonly float overflowDamage;
        public readonly float totalDamage;

        public DamageTakenInfo(float totalDamage, float overflowDamage)
        {
            this.totalDamage = totalDamage;
            this.overflowDamage = overflowDamage;
        }
    }

    public readonly struct HealthDamageTaken
    {
        public readonly HealthTypeData healthType;
        public readonly float damageTaken;
        public readonly bool depleted;

        public HealthDamageTaken(HealthTypeData healthType, float damageTaken, bool depleted)
        {
            this.healthType = healthType;
            this.damageTaken = damageTaken;
            this.depleted = depleted;
        }
    }
}