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
                for (int x = 0; x < items[i].stacks; x++)
                {
                    AddItem(items[i].Data);
                }
            }
        }

        public void AddItem(ItemData itemData)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Data == itemData)
                {
                    UpdateItemStacks(items[i]);
                    onItemStacksUpdated?.Invoke(itemData, items[i].stacks);
                    return;
                }
            }

            AddNewItem(itemData);
            onItemAdded?.Invoke(itemData);
        }

        public void AddItemStacks(ItemData itemData, int stacks)
        {
            if (!TryGetItem(itemData, out Item item))
            {
                AddNewItem(itemData);
                onItemAdded?.Invoke(itemData);
                item = items[^1];
                stacks -= 1;
            }

            for (int i = 0; i < stacks; i++)
            {
                UpdateItemStacks(item);
                onItemStacksUpdated?.Invoke(itemData, item.stacks);
            }
        }

        public void RemoveItem(ItemData itemData)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Data == itemData)
                {
                    if (items[i].stacks == 1)
                    {
                        items[i].OnRemoved();
                        items.RemoveAt(i);
                        onItemRemoved?.Invoke(itemData);

                        itemsData.Remove(itemData);
                    }
                    else
                    {
                        items[i].stacks -= 1;
                        items[i].OnStackRemoved();
                        onItemStacksUpdated?.Invoke(itemData, items[i].stacks);
                    }

                    return;
                }
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

        public void RemoveItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnRemoved();
            }

            items.Clear();
            itemsData.Clear();
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

        private void AddNewItem(ItemData itemData)
        {
            items.Add(itemData.CreateItem(this, gameObject));
            items[^1].stacks = 1;
            items[^1].OnAdded();

            itemsData.Add(itemData);
        }

        private void UpdateItemStacks(Item item)
        {
            item.OnStackAdded();
            item.stacks += 1;
        }
    }
}