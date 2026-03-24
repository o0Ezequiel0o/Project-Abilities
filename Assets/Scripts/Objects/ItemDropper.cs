using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ItemDropper : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSettings itemSettings;

    [field: Header("Visual")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }

    [field: SerializeField] public Color CanInteractOverlayColor { get; private set; }
    [field: SerializeField] public Color CantInteractOverlayColor { get; private set; }

    [Header("Settings")]
    [SerializeField] protected int cost;
    [Space]
    [SerializeField] private float dropDistance;
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private List<ItemDropSlot> itemDropChance;

    public abstract bool CanSelect(GameObject source);

    public abstract bool CanInteract(GameObject source);

    public abstract bool Interact(GameObject source);

    protected void SpawnRandomItem()
    {
        ItemRarity rarity = WeightedSelect.SelectElement(itemDropChance).rarity;
        SpawnItem(rarity);
    }

    protected void DisableColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    protected void DestroyObject()
    {
        Destroy(gameObject);
    }

    void OnValidate()
    {
        AddMissingRarities();
        RemoveDuplicates();
    }

    void Reset()
    {
        itemDropChance = new List<ItemDropSlot>();
        AddMissingRarities();
    }

    void SpawnItem(ItemRarity rarity)
    {
        ItemData item = itemSettings.GetRandomItem(rarity);

        if (item == null) return;

        Vector3 spawnPosition = transform.position + transform.up * dropDistance;
        GameObject itemDropInstance = Instantiate(itemDropPrefab, spawnPosition, Quaternion.identity);

        if (itemDropInstance.TryGetComponent(out ItemDrop itemDrop))
        {
            itemDrop.LoadItemData(item);
        }
    }

    void AddMissingRarities()
    {
        AddMissingRarity(ItemRarity.Common, 50);
        AddMissingRarity(ItemRarity.Rare, 50);
        AddMissingRarity(ItemRarity.Epic, 50);
        AddMissingRarity(ItemRarity.Legendary, 50);
        AddMissingRarity(ItemRarity.Unique, 0);
    }

    void AddMissingRarity(ItemRarity itemRarity, int weight)
    {
        if (!HasRarity(itemRarity))
        {
            itemDropChance.Add(new ItemDropSlot(itemRarity, weight));
        }
    }

    void RemoveDuplicates()
    {
        List<int> indexToRemove = new List<int>();

        for (int i = 0; i < itemDropChance.Count; i++)
        {
            int duplicatesAmount = -1;

            for (int x = 0; x < itemDropChance.Count; x++)
            {
                if (itemDropChance[x].rarity == itemDropChance[i].rarity)
                {
                    duplicatesAmount += 1;
                }
            }

            for (int x = itemDropChance.Count - 1; x >= 0; x--)
            {
                if (duplicatesAmount <= 0) continue;

                if (itemDropChance[x].rarity == itemDropChance[i].rarity)
                {
                    indexToRemove.Add(x);
                    duplicatesAmount -= 1;
                }
            }
        }

        for (int i = itemDropChance.Count - 1; i >= 0; i--)
        {
            if (indexToRemove.Contains(i))
            {
                indexToRemove.Remove(i);
                itemDropChance.RemoveAt(i);
            }
        }
    }

    bool HasRarity(ItemRarity rarity)
    {
        for (int i = 0; i < itemDropChance.Count; i++)
        {
            if (itemDropChance[i].rarity == rarity)
            {
                return true;
            }
        }

        return false;
    }

    [Serializable]
    private class ItemDropSlot : IWeighted
    {
        [ReadOnly] public ItemRarity rarity;
        [SerializeField] public int weight;

        public int Weight => weight;

        public ItemDropSlot() { }

        public ItemDropSlot(ItemRarity rarity, int weight)
        {
            this.rarity = rarity;
            this.weight = weight;
        }
    }
}