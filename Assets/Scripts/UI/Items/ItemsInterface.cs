using System.Collections.Generic;
using UnityEngine;
using static Zeke.Items.ItemHandler;

namespace Zeke.Items
{
    public class ItemsInterface : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private ItemDisplaySlot itemDisplaySlotPrefab;

        private readonly Dictionary<ItemData, ItemDisplaySlot> itemDisplaySlots = new Dictionary<ItemData, ItemDisplaySlot>();

        public void SubscribeToEvents(ItemHandler itemHandler)
        {
            itemHandler.onItemAdded += AddNewItem;
            itemHandler.onItemRemoved += RemoveItem;
            itemHandler.onStacksAdded += UpdateItemStacks;
            itemHandler.onStacksRemoved += UpdateItemStacks;
        }

        public void UnsubscribeFromEvents(ItemHandler itemHandler)
        {
            itemHandler.onItemAdded -= AddNewItem;
            itemHandler.onStacksAdded -= UpdateItemStacks;
        }

        public void RefreshAllItems(ItemHandler itemHandler)
        {
            for (int i = 0; i < itemHandler.Items.Count; i++)
            {
                if (itemDisplaySlots.TryGetValue(itemHandler.Items[i].Data, out ItemDisplaySlot itemDisplaySlot))
                {
                    itemDisplaySlot.UpdateStacksAmount(itemHandler.Items[i].stacks);
                }
                else
                {
                    AddItem(itemHandler.Items[i].Data, itemHandler.Items[i].stacks);
                }
            }
        }

        private void AddNewItem(ItemData itemData)
        {
            AddItem(itemData, 1);
        }

        private void AddItem(ItemData itemData, int stacks)
        {
            ItemDisplaySlot itemDisplaySlot = Instantiate(itemDisplaySlotPrefab, root);
            itemDisplaySlot.SetData(itemData);
            itemDisplaySlot.UpdateStacksAmount(stacks);

            itemDisplaySlots.Add(itemData, itemDisplaySlot);
        }

        private void RemoveItem(ItemData itemData)
        {
            if (itemDisplaySlots.TryGetValue(itemData, out ItemDisplaySlot itemDisplaySlot))
            {
                Destroy(itemDisplaySlot.gameObject);
            }
        }

        private void UpdateItemStacks(ItemStackUpdate stackUpdate)
        {
            itemDisplaySlots[stackUpdate.itemData].UpdateStacksAmount(stackUpdate.stacks);
        }
    }
}