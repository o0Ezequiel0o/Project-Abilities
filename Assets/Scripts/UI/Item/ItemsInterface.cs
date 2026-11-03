using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zeke.UI;
using TMPro;

using static Zeke.Items.ItemHandler;

namespace Zeke.Items
{
    public class ItemsInterface : MonoBehaviour
    {
        [SerializeField] protected UIWindow window;
        [SerializeField] protected UIWindow itemSlotPrefab;

        public UIWindow Window => window;

        protected readonly Dictionary<ItemData, UIWindow> itemSlots = new Dictionary<ItemData, UIWindow>();

        public void AddMissingItems(ItemHandler itemHandler)
        {
            for (int i = 0; i < itemHandler.Items.Count; i++)
            {
                if (itemSlots.TryGetValue(itemHandler.Items[i].Data, out UIWindow windowInstance))
                {
                    windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").SetText(GetStacksString(itemHandler.Items[i].stacks));
                }
                else
                {
                    AddItem(itemHandler.Items[i].Data, itemHandler.Items[i].stacks);
                }
            }
        }

        public virtual UIWindow AddItem(ItemData itemData, int stacks)
        {
            Transform root = window.TryGetElement<LayoutGroup>("Layout Group").transform;
            UIWindow windowInstance = Instantiate(itemSlotPrefab, root);

            windowInstance.TryGetElement<Image>("Icon").sprite = itemData.Icon;
            windowInstance.TryGetElement<Image>("Outline").sprite = itemData.Outline;
            windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").text = GetStacksString(stacks);

            ItemSlotReference itemSlotReference = windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference");

            itemSlotReference.ItemData = itemData;
            itemSlotReference.Stacks = stacks;

            itemSlots.Add(itemData, windowInstance);

            return windowInstance;
        }

        public virtual UIWindow RemoveItem(ItemData itemData)
        {
            if (itemSlots.TryGetValue(itemData, out UIWindow windowInstance))
            {
                itemSlots.Remove(itemData);
                Destroy(windowInstance.gameObject);
                return windowInstance;
            }

            return null;
        }

        public virtual UIWindow UpdateItemStacks(ItemStackUpdate stackUpdate)
        {
            UIWindow windowInstance = itemSlots[stackUpdate.itemData];

            windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference").Stacks = stackUpdate.stacks;
            windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").SetText(GetStacksString(stackUpdate.stacks));

            return windowInstance;
        }

        protected string GetStacksString(int stacks) => "x" + stacks.ToString();
    }
}