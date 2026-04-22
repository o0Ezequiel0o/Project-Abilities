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

    private readonly List<StatusEffectDisplaySlot> statusEffectDisplaySlots = new List<StatusEffectDisplaySlot>();
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
        if (usedStatusEffectDisplaySlots.Count >= statusEffectDisplaySlots.Count) return;

        usedStatusEffectDisplaySlots.Add(statusEffect, statusEffectDisplaySlots[usedStatusEffectDisplaySlots.Count]);
        StatusEffectDisplaySlot statusEffectDisplaySlot = usedStatusEffectDisplaySlots[statusEffect];

        InitializeStatusEffectSlotData(statusEffectDisplaySlot, statusEffect);
        statusEffectDisplaySlot.gameObject.SetActive(true);
    }

    public void RemoveStatusEffectSlot(StatusEffect statusEffect)
    {
        if (usedStatusEffectDisplaySlots.TryGetValue(statusEffect, out StatusEffectDisplaySlot statusEffectDisplaySlot))
        {
            statusEffectDisplaySlot.gameObject.SetActive(false);
            usedStatusEffectDisplaySlots.Remove(statusEffect);
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
            statusEffectDisplaySlots.Add(Instantiate(statusEffectDisplaySlotPrefab, statusEffectDisplaySlotsRoot));
            statusEffectDisplaySlots[^1].gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (root.gameObject == null) return;
        Destroy(root.gameObject);
    }
}