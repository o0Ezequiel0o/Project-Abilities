using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    public class ItemRegistry : GameService<ItemRegistry>
    {
        [SerializeField] private ItemSettings itemSettings;

        private readonly List<ItemData> items = new List<ItemData>();
        private readonly Dictionary<ItemRarity, List<ItemData>> sortedItems = new Dictionary<ItemRarity, List<ItemData>>()
        {
            { ItemRarity.Common, new List<ItemData>() },
            { ItemRarity.Rare, new List<ItemData>() },
            { ItemRarity.Epic, new List<ItemData>() },
            { ItemRarity.Legendary, new List<ItemData>() },
        };

        private readonly List<ItemData> filteredList = new List<ItemData>(256);

        public ItemData GetRandomItem(ItemRarity rarity)
        {
            List<ItemData> itemsOfRarity = sortedItems[rarity];

            if (itemsOfRarity.Count == 0)
            {
                throw new System.Exception($"No items of {rarity} rarity, maybe core is not loaded?");
            }

            return itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];
        }

        public ItemData GetRandomItem(ItemRarity rarity, ItemBlacklist blacklist)
        {
            List<ItemData> itemsOfRarity = sortedItems[rarity];

            if (itemsOfRarity.Count == 0)
            {
                throw new System.Exception($"No items of {rarity} rarity, maybe core is not loaded?");
            }

            filteredList.Clear();

            foreach (ItemData item in itemsOfRarity)
            {
                if (!blacklist.IsBlacklisted(item))
                {
                    filteredList.Add(item);
                }
            }

            if (filteredList.Count == 0)
            {
                throw new System.Exception($"No items of {rarity} rarity, all items ({itemsOfRarity.Count}) were blacklisted.");
            }

            return filteredList[Random.Range(0, filteredList.Count)];
        }

        public void Add(List<ItemData> itemList)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                Add(itemList[i]);
            }
        }

        public void Add(ItemData item)
        {
            if (item == null)
            {
                throw new System.Exception("Item cannot be null");
            }

            List<ItemData> itemsOfRarity = sortedItems[item.Rarity];

            if (itemsOfRarity.Contains(item))
            {
                throw new System.Exception($"Duplicate item key in {sortedItems}: {item} ({item.Name})");
            }

            itemsOfRarity.Add(item);
            items.Add(item);
        }

        public void Remove(ItemData item)
        {
            if (item == null)
            {
                throw new System.Exception("Item cannot be null");
            }

            sortedItems[item.Rarity].Remove(item);
            items.Remove(item);
        }
    }
}