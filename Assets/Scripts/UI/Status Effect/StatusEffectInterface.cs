using System.Collections.Generic;
using UnityEngine;

public class StatusEffectInterface : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform root;
    
    [Header("Spawning")]
    [SerializeField] private StatusEffectDisplaySlot statusEffectDisplaySlotPrefab;
    [SerializeField] private Transform statusEffectDisplaySlotsRoot;
    [SerializeField] private int spawnAmount;

    private readonly Stack<StatusEffectDisplaySlot> statusEffectDisplaySlots = new Stack<StatusEffectDisplaySlot>();
    private readonly Dictionary<StatusEffect, StatusEffectDisplaySlot> usedStatusEffectDisplaySlots = new Dictionary<StatusEffect, StatusEffectDisplaySlot>();

    private void Awake()
    {
        SpawnStatusEffectDisplaySlots();
    }

    public void LoadData(List<StatusEffect> statusEffects)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (usedStatusEffectDisplaySlots.ContainsKey(statusEffects[i]))
            {
                UpdateStatusEffectSlot(statusEffects[i]);
            }
            else
            {
                AddStatusEffectSlot(statusEffects[i]);
                UpdateStatusEffectSlot(statusEffects[i]);
            }
        }
    }

    public void UpdateStatusEffectSlot(StatusEffect statusEffect)
    {
        if (usedStatusEffectDisplaySlots.TryGetValue(statusEffect, out StatusEffectDisplaySlot statusEffectDisplaySlot))
        {
            statusEffectDisplaySlot.UpdateStacksAmount(statusEffect.stacks);
        }
    }

    public void AddStatusEffectSlot(StatusEffect statusEffect)
    {
        if (usedStatusEffectDisplaySlots.ContainsKey(statusEffect)) return;
        if (statusEffectDisplaySlots.Count == 0) return;

        usedStatusEffectDisplaySlots.Add(statusEffect, statusEffectDisplaySlots.Pop());
        StatusEffectDisplaySlot statusEffectDisplaySlot = usedStatusEffectDisplaySlots[statusEffect];

        InitializeStatusEffectSlotData(statusEffectDisplaySlot, statusEffect);
        statusEffectDisplaySlot.gameObject.SetActive(true);
    }

    public void RemoveStatusEffectSlot(StatusEffect statusEffect)
    {
        if (usedStatusEffectDisplaySlots.TryGetValue(statusEffect, out StatusEffectDisplaySlot statusEffectDisplaySlot))
        {
            if (statusEffectDisplaySlot == null) return;

            statusEffectDisplaySlot.gameObject.SetActive(false);
            usedStatusEffectDisplaySlots.Remove(statusEffect);
            statusEffectDisplaySlots.Push(statusEffectDisplaySlot);
        }
    }

    private void InitializeStatusEffectSlotData(StatusEffectDisplaySlot abilityDisplaySlot, StatusEffect statusEffect)
    {
        abilityDisplaySlot.Icon = statusEffect.Data.Icon;
    }

    private void SpawnStatusEffectDisplaySlots()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            StatusEffectDisplaySlot slot = Instantiate(statusEffectDisplaySlotPrefab, statusEffectDisplaySlotsRoot);
            statusEffectDisplaySlots.Push(slot);
            slot.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (root.gameObject == null) return;
        Destroy(root.gameObject);
    }
}