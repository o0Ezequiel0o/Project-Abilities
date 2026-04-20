using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "ScriptableObjects/Items/ItemSettings", order = 1)]
    public class ItemSettings : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private ItemTriggerOrder itemTriggerOrder;
        [SerializeField] private List<ItemData> avaibleItems;

        [Header("Colors")]
        [SerializeField] private Color commonColor;
        [SerializeField] private Color rareColor;
        [SerializeField] private Color epicColor;
        [SerializeField] private Color legendaryColor;
        [SerializeField] private Color uniqueColor;

        [Header("Debug")]
        [ReadOnly, SerializeField] private List<ItemData> itemsNotInTriggerOrder;

        [Header("Sorted Items")]
        [ReadOnly, SerializeField] private List<ItemData> commonItems;
        [ReadOnly, SerializeField] private List<ItemData> rareItems;
        [ReadOnly, SerializeField] private List<ItemData> epicItems;
        [ReadOnly, SerializeField] private List<ItemData> legendaryItems;
        [ReadOnly, SerializeField] private List<ItemData> uniqueItems;

        public List<ItemData> TriggerOrder => itemTriggerOrder.Order;

        public Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return commonColor;

                case ItemRarity.Rare:
                    return rareColor;

                case ItemRarity.Epic:
                    return epicColor;

                case ItemRarity.Legendary:
                    return legendaryColor;

                case ItemRarity.Unique:
                    return uniqueColor;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public ItemData GetRandomItem(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    if (commonItems.Count > 0)
                        return commonItems[Random.Range(0, commonItems.Count)];
                    break;

                case ItemRarity.Rare:
                    if (rareItems.Count > 0)
                        return rareItems[Random.Range(0, rareItems.Count)];
                    break;

                case ItemRarity.Epic:
                    if (epicItems.Count > 0)
                        return epicItems[Random.Range(0, epicItems.Count)];
                    break;

                case ItemRarity.Legendary:
                    if (legendaryItems.Count > 0)
                        return legendaryItems[Random.Range(0, legendaryItems.Count)];
                    break;

                case ItemRarity.Unique:
                    if (uniqueItems.Count > 0)
                        return uniqueItems[Random.Range(0, uniqueItems.Count)];
                    break;
            }

            return null;
        }

        public int GetTriggerOrder(ItemData itemData)
        {
            for (int i = 0; i < itemTriggerOrder.Order.Count; i++)
            {
                if (itemTriggerOrder.Order[i] == itemData)
                {
                    return i;
                }
            }

            Debug.LogWarning($"{itemData.name} does not have a trigger order, please add it to {itemTriggerOrder.name}.", itemData);

            return 0;
        }

        private void OnValidate()
        {
            SearchForAvaibleItemsNotInTriggerOrder();
            RefreshItemRarityLists();
        }

        private void Awake()
        {
            RefreshItemRarityLists();
        }

        private void RefreshItemRarityLists()
        {
            AddItemsOfRarityToList(commonItems, ItemRarity.Common);
            AddItemsOfRarityToList(rareItems, ItemRarity.Rare);
            AddItemsOfRarityToList(epicItems, ItemRarity.Epic);
            AddItemsOfRarityToList(legendaryItems, ItemRarity.Legendary);
            AddItemsOfRarityToList(uniqueItems, ItemRarity.Unique);
        }

        private void AddItemsOfRarityToList(List<ItemData> list, ItemRarity rarity)
        {
            list.Clear();

            for (int i = 0; i < avaibleItems.Count; i++)
            {
                if (avaibleItems[i].Rarity == rarity)
                {
                    list.Add(avaibleItems[i]);
                }
            }
        }

        private void SearchForAvaibleItemsNotInTriggerOrder()
        {
            itemsNotInTriggerOrder.Clear();

            for (int i = 0; i < avaibleItems.Count; i++)
            {
                if (!itemTriggerOrder.Order.Contains(avaibleItems[i]))
                {
                    itemsNotInTriggerOrder.Add(avaibleItems[i]);
                }
            }
        }
    }
}