using System.Collections.Generic;
using UnityEngine;
using static Zeke.Items.ItemHandler;

namespace Zeke.Items
{
    public class ItemNotificatorInterface : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private ItemNotificatorSlot itemNotificationDisplaySlotPrefab;

        [Space]

        [SerializeField] private float duration = 2;

        private readonly Dictionary<ItemData, SlotData> activeSlots = new Dictionary<ItemData, SlotData>();
        private readonly Stack<SlotData> unactiveSlots = new Stack<SlotData>();

        private readonly List<SlotData> deactivateSlots = new List<SlotData>();

        public void SubscribeToEvents(ItemHandler itemHandler)
        {
            itemHandler.onItemAdded += OnItemAdded;
            itemHandler.onStacksAdded += OnItemStacksAdded;
            itemHandler.onStacksRemoved += OnItemStacksRemoved;
        }

        public void UnsubscribeFromEvents(ItemHandler itemHandler)
        {
            itemHandler.onItemAdded -= OnItemAdded;
            itemHandler.onStacksAdded -= OnItemStacksAdded;
            itemHandler.onStacksRemoved += OnItemStacksRemoved;
        }

        private void Update()
        {
            foreach (SlotData slotData in activeSlots.Values)
            {
                slotData.timer += Time.deltaTime;

                if (slotData.timer > duration)
                {
                    deactivateSlots.Add(slotData);
                }
            }

            for (int i = 0; i <  deactivateSlots.Count; i++)
            {
                DeactivateSlot(deactivateSlots[i]);
            }

            deactivateSlots.Clear();
        }

        private void OnItemAdded(ItemData itemData)
        {
            AddSlot(itemData, 0);
        }

        private void OnItemStacksAdded(ItemStackUpdate stackUpdate)
        {
            if (activeSlots.TryGetValue(stackUpdate.itemData, out SlotData slotData))
            {
                UpdateSlot(slotData, stackUpdate.amount);
            }
            else if (unactiveSlots.Count > 0)
            {
                AddUnactiveSlot(stackUpdate.itemData, stackUpdate.amount);
            }
            else
            {
                AddSlot(stackUpdate.itemData, stackUpdate.amount);
            }
        }

        private void OnItemStacksRemoved(ItemStackUpdate stackUpdate)
        {
            if (activeSlots.TryGetValue(stackUpdate.itemData, out SlotData slotData))
            {
                UpdateSlot(slotData, -stackUpdate.amount);
            }
            else if (unactiveSlots.Count > 0)
            {
                AddUnactiveSlot(stackUpdate.itemData, -stackUpdate.amount);
            }
            else
            {
                AddSlot(stackUpdate.itemData, -stackUpdate.amount);
            }
        }

        private void AddSlot(ItemData itemData, int stacksUpdate)
        {
            ItemNotificatorSlot itemDisplaySlot = Instantiate(itemNotificationDisplaySlotPrefab, root);
            SlotData slotData = new SlotData(itemDisplaySlot, itemData);

            itemDisplaySlot.SetData(itemData);
            itemDisplaySlot.UpdateStacksAmount(stacksUpdate);

            activeSlots.Add(itemData, slotData);
        }

        private void AddUnactiveSlot(ItemData itemData, int stacksUpdate)
        {
            SlotData slotData = unactiveSlots.Pop();

            slotData.timer = 0f;
            slotData.itemData = itemData;

            slotData.slot.SetData(itemData);
            slotData.slot.ResetAccumulator();
            slotData.slot.UpdateStacksAmount(stacksUpdate);

            activeSlots.Add(itemData, slotData);
            slotData.slot.gameObject.SetActive(true);
        }

        private void UpdateSlot(SlotData slotData, int stacksUpdate)
        {
            slotData.slot.UpdateStacksAmount(stacksUpdate);

            if (slotData.slot.Accumulator == 0)
            {
                DeactivateSlot(slotData);
            }
        }

        private void DeactivateSlot(SlotData slotData)
        {
            activeSlots.Remove(slotData.itemData);
            slotData.slot.gameObject.SetActive(false);
            unactiveSlots.Push(slotData);
        }

        private class SlotData
        {
            public ItemNotificatorSlot slot;
            public ItemData itemData;
            public float timer = 0f;

            public SlotData(ItemNotificatorSlot slot, ItemData itemData)
            {
                this.slot = slot;
                this.itemData = itemData;
            }
        }
    }
}