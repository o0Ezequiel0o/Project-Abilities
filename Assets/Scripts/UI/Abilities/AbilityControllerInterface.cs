using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeke.Abilities;
using Zeke.UI;

public class AbilityControllerInterface : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform root;
    [SerializeField] private UIWindow descriptionWindow;

    [Header("Spawning")]
    [SerializeField] private AbilityDisplaySlot abilityDisplaySlotPrefab;
    [SerializeField] private Transform abilityDisplaySlotsRoot;

    private readonly Dictionary<IAbility, AbilityDisplaySlot> usedAbilityDisplaySlots = new Dictionary<IAbility, AbilityDisplaySlot>();

    private void Awake()
    {
        SpawnAbilityDisplaySlots();
        descriptionWindow.gameObject.SetActive(false);
    }

    public void LoadData(Dictionary<AbilityType, AbilityController.AbilitySlot> abilities)
    {
        foreach (AbilityType abilityType in abilities.Keys)
        {
            if (abilities[abilityType].Ability == null) continue;
            AddAbilitySlot(abilities[abilityType].Ability, abilityType);
        }
    }

    public void UpdateAbilitySlotRender(IAbility ability)
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
                abilityDisplaySlot.UpdateCooldownBar(1f);
            }
            else
            {
                abilityDisplaySlot.UpdateDurationBar(0f);
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

    public void AddAbilitySlot(IAbility ability, AbilityType abilityType)
    {
        if (!usedAbilityDisplaySlots.ContainsKey(ability))
        {
            usedAbilityDisplaySlots.Add(ability, null);
            LayoutGroup layoutGroup = root.GetComponentInChildren<LayoutGroup>();
            Transform obj = layoutGroup.transform.GetChild((int)abilityType);

            AbilityDisplaySlot slot = obj.GetComponent<AbilityDisplaySlot>();

            usedAbilityDisplaySlots[ability] = slot;
            RefreshAbilitySlotData(slot, ability.Data);

            slot.gameObject.SetActive(true);
        }
    }

    public void RemoveAbilitySlot(IAbility ability, AbilityType _)
    {
        if (usedAbilityDisplaySlots.TryGetValue(ability, out AbilityDisplaySlot slot))
        {
            if (usedAbilityDisplaySlots.Remove(ability))
            {
                if (slot == null) return;
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void RefreshAbilitySlotData(AbilityDisplaySlot abilityDisplaySlot, AbilityData abilityData)
    {
        abilityDisplaySlot.CooldowSprite = abilityData.Icon;
        abilityDisplaySlot.UsableSprite = abilityData.Icon;
        abilityDisplaySlot.Background = abilityData.Icon;
    }

    private void SpawnAbilityDisplaySlots()
    {
        System.Collections.IList list = Enum.GetValues(typeof(AbilityType));

        for (int i = 0; i < list.Count; i++)
        {
            AbilityDisplaySlot slot = Instantiate(abilityDisplaySlotPrefab, abilityDisplaySlotsRoot);
            slot.onPointerEnter += OnPointerEnterSlot;
            slot.onPointerExit += OnPointerExitSlot;
            slot.gameObject.SetActive(false);
        }
    }

    private void OnPointerEnterSlot(AbilityDisplaySlot slot)
    {
        foreach (IAbility key in usedAbilityDisplaySlots.Keys)
        {
            if (usedAbilityDisplaySlots[key] == slot)
            {
                RefreshAbilityDescriptionMenu(key);
                descriptionWindow.gameObject.SetActive(true);
            }
        }
    }

    private void OnPointerExitSlot(AbilityDisplaySlot slot)
    {
        descriptionWindow.gameObject.SetActive(false);
    }

    private void RefreshAbilityDescriptionMenu(IAbility ability)
    {
        descriptionWindow.TryGetElement<TextMeshProUGUI>("Name").SetText(ability.Data.Name);
        descriptionWindow.TryGetElement<TextMeshProUGUI>("Description").SetText(ability.Data.Description);
        descriptionWindow.TryGetElement<TextMeshProUGUI>("Cooldown").SetText("[CD: " + ability.CooldownTime.ToString("F1", CultureInfo.InvariantCulture) + "]");
    }

    private void OnDestroy()
    {
        if (root.gameObject == null) return;
        Destroy(root.gameObject);
    }
}