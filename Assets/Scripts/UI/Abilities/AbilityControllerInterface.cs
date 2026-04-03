using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;

public class AbilityControllerInterface : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform root;
    [SerializeField] private List<AbilityType> abilityDisplayOrder;

    [Header("Spawning")]
    [SerializeField] private AbilityDisplaySlot abilityDisplaySlotPrefab;
    [SerializeField] private Transform abilityDisplaySlotsRoot;
    [SerializeField] private int spawnAmount;

    private readonly List<AbilityDisplaySlot> abilityDisplaySlots = new List<AbilityDisplaySlot>();
    private readonly Dictionary<IAbility, AbilityDisplaySlot> usedAbilityDisplaySlots = new Dictionary<IAbility, AbilityDisplaySlot>();

    void Awake()
    {
        SpawnAbilityDisplaySlots();
    }

    public void LoadData(List<IAbility> abilities)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (!usedAbilityDisplaySlots.ContainsKey(abilities[i]))
            {
                AddAbilitySlot(abilities[i]);
            }
        }
    }

    public void UpdateAbilitySlot(IAbility ability)
    {
        if (usedAbilityDisplaySlots.TryGetValue(ability, out AbilityDisplaySlot abilityDisplaySlot))
        {
            if (ability.CooldownTime > 0 || ability.Charges > 0)
            {
                abilityDisplaySlot.UpdateUseState(ability.Charges, ability.DurationActive);
            }

            if (ability.DurationActive)
            {
                abilityDisplaySlot.UpdateDurationBar(ability.DurationPercentage);
                abilityDisplaySlot.UpdateCooldownBar(0f);
            }
            else
            {
                abilityDisplaySlot.UpdateDurationBar(1f);
                abilityDisplaySlot.UpdateCooldownBar(ability.ChargePercentage);
            }

            if (ability.MaxCharges > 1)
            {
                abilityDisplaySlot.UpdateChargesText(ability.Charges);
            }
            else
            {
                abilityDisplaySlot.ClearChargesText();
            }
        }
    }

    public void AddAbilitySlot(IAbility ability)
    {
        if (usedAbilityDisplaySlots.Count >= abilityDisplaySlots.Count) return;

        usedAbilityDisplaySlots.Add(ability, null);
        RefreshAbilitySlotsData();
    }

    public void RemoveAbilitySlot(IAbility ability)
    {
        if (usedAbilityDisplaySlots.Remove(ability))
        {
            RefreshAbilitySlotsData();
        }
    }

    public void RefreshAbilitySlotsData()
    {
        List<IAbility> abilities = new List<IAbility>(usedAbilityDisplaySlots.Keys);
        List<IAbility> orderedAbilities = OrderAbilitiesByType(abilities);

        usedAbilityDisplaySlots.Clear();

        for (int i = 0; i < abilityDisplaySlots.Count; i++)
        {
            if (abilityDisplaySlots[i] == null) continue;

            if (i < orderedAbilities.Count)
            {
                usedAbilityDisplaySlots.Add(orderedAbilities[i], abilityDisplaySlots[i]);
                RefreshAbilitySlotData(abilityDisplaySlots[i], orderedAbilities[i].Data);

                abilityDisplaySlots[i].gameObject.SetActive(true);
            }
            else
            {
                abilityDisplaySlots[i].gameObject.SetActive(false);
            }
        }
    }

    void RefreshAbilitySlotData(AbilityDisplaySlot abilityDisplaySlot, AbilityData abilityData)
    {
        abilityDisplaySlot.CooldowSprite = abilityData.Icon;
        abilityDisplaySlot.UsableSprite = abilityData.Icon;
        abilityDisplaySlot.Background = abilityData.Icon;
    }

    List<IAbility> OrderAbilitiesByType(List<IAbility> original)
    {
        List<IAbility> copy = new List<IAbility>();

        for (int i = 0; i < abilityDisplayOrder.Count; i++)
        {
            for (int x = 0; x < original.Count; x++)
            {
                if (original[x].Data.AbilityType == abilityDisplayOrder[i])
                {
                    copy.Add(original[x]);
                }
            }
        }

        return copy;
    }

    void SpawnAbilityDisplaySlots()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            abilityDisplaySlots.Add(Instantiate(abilityDisplaySlotPrefab, abilityDisplaySlotsRoot));
        }
    }
    
    void OnDestroy()
    {
        if (root.gameObject == null) return;
        Destroy(root.gameObject);
    }
}