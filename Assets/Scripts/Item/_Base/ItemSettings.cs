using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "ScriptableObjects/Items/ItemSettings", order = 1)]
    public class ItemSettings : ScriptableObject
    {
        [Header("Colors")]
        [SerializeField] private Color commonColor;
        [SerializeField] private Color rareColor;
        [SerializeField] private Color epicColor;
        [SerializeField] private Color legendaryColor;
        [SerializeField] private Color uniqueColor;

        [Header("Items")]
        [SerializeField] private List<ItemData> avaibleItems;

        [Header("Sorted Items")]
        [ReadOnly, SerializeField] private Dictionary<ItemRarity, List<ItemData>> itemsByRarity;

        public void AddItem(ItemData item)
        {
            if (item != null && !avaibleItems.Contains(item))
            {
                avaibleItems.Add(item);
                AddFilteredItemData(item);
            }
        }

        public Color GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => commonColor,
                ItemRarity.Rare => rareColor,
                ItemRarity.Epic => epicColor,
                ItemRarity.Legendary => legendaryColor,
                ItemRarity.Unique => uniqueColor,
                _ => throw new System.ArgumentOutOfRangeException(),
            };
        }

        public ItemData GetRandomItem(ItemRarity rarity)
        {
            if (itemsByRarity.TryGetValue(rarity, out List<ItemData> items))
            {
                if (items.Count > 0)
                {
                    return items[Random.Range(0, items.Count)];
                }
            }

            return null;
        }

        private void OnValidate()
        {
            RefreshItemRarityList();
        }

        private void OnEnable()
        {
            itemsByRarity ??= new Dictionary<ItemRarity, List<ItemData>>();
            RefreshItemRarityList();
        }

        private void RefreshItemRarityList()
        {
            itemsByRarity.Clear();

            for (int i = 0; i < avaibleItems.Count; i++)
            {
                AddFilteredItemData(avaibleItems[i]);
            }
        }

        private void AddFilteredItemData(ItemData itemData)
        {
            if (!itemsByRarity.ContainsKey(itemData.Rarity))
            {
                itemsByRarity.Add(itemData.Rarity, new List<ItemData>());
            }

            if (!itemsByRarity[itemData.Rarity].Contains(itemData))
            {
                itemsByRarity[itemData.Rarity].Add(itemData);
            }
        }
    }
}