using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Items
{
    public class ItemHandler : MonoBehaviour
    {
        [Header("Dependency")]
        [SerializeField] private ItemSettings itemSettings;

        [field: Header("Settings")]
        [field: SerializeField] public int Luck { get; private set; }

        public List<Item> Items => items;
        public List<ItemData> ItemsData => itemsData;

        public Action<ItemData> onItemAdded;
        public Action<ItemData> onItemRemoved;

        public Action<ItemData, int> onItemStacksUpdated;

        private readonly List<Item> items = new List<Item>();
        private readonly List<ItemData> itemsData = new List<ItemData>();

        public void AddItems(List<ItemData> itemsData)
        {
            for (int i = 0; i < itemsData.Count; i++)
            {
                AddItem(itemsData[i]);
            }
        }

        public void AddItems(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                AddItem(items[i].Data);
            }
        }

        public void AddItem(ItemData itemData)
        {
            AddItem(itemData, 1);
        }

        public void AddItem(ItemData itemData, int stacks)
        {
            if (TryGetItem(itemData, out Item item))
            {
                AddItemStacks(item, stacks);
            }
            else
            {
                AddNewItem(itemData, stacks);
            }
        }

        public void RemoveItem(ItemData itemData)
        {
            RemoveItem(itemData, int.MaxValue);
        }

        public void RemoveItem(ItemData itemData, int stacks)
        {
            if (TryGetItem(itemData, out Item item))
            {
                RemoveItem(item, stacks);
            }
        }

        public void RemoveItems()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                RemoveItem(items[i], int.MaxValue);
            }
        }

        public bool TryGetItem(ItemData itemData, out Item item)
        {
            item = null;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Data == itemData)
                {
                    item = items[i];
                    return true;
                }
            }

            return false;
        }

        private void Update()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnUpdate();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnRemoved();
            }
        }

        private void AddNewItem(ItemData itemData, int stacks)
        {
            if (stacks == 0) return;

            Item item = itemData.CreateItem(this, gameObject);

            items.Add(item);
            itemsData.Add(itemData);

            item.Initialize();
            onItemAdded?.Invoke(itemData);
            AddItemStacks(item, stacks);
        }

        private void RemoveItem(Item item, int stacks)
        {
            int maxStacks = Mathf.Min(stacks, item.stacks);

            if (maxStacks <= 0) return;

            RemoveItemStacks(item, maxStacks);

            if (item.stacks <= 0)
            {
                items.Remove(item);
                itemsData.Remove(item.Data);
                onItemRemoved?.Invoke(item.Data);
            }
        }

        private void AddItemStacks(Item item, int stacks)
        {
            item.stacks += stacks;
            item.OnStacksAdded(stacks);

            onItemStacksUpdated?.Invoke(item.Data, stacks);
        }

        private void RemoveItemStacks(Item item, int stacks)
        {
            item.stacks -= stacks;
            item.OnStacksRemoved(stacks);

            onItemStacksUpdated?.Invoke(item.Data, stacks);
        }
    }
}