using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using TMPro;

namespace Zeke.Items
{
    public class Dismantler : MonoBehaviour, IInteractable, IInteractableTooltipRenderer
    {
        [field: Header("Settings")]
        [SerializeField] private ItemSettings itemSettings;
        [SerializeField] private DismantlerRewards rewards;
        [SerializeField] private DisallowedItems disallowedItems;

        [field: Header("Visual - Object")]
        [field: SerializeField] public Sprite InteractOverlay { get; private set; }

        [field: Header("Visual - UI")]
        [SerializeField] private UIWindow itemSelectionWindow;
        [SerializeField] private UIWindow ItemSlotWindow;

        [Space]

        [SerializeField] private Color selectedSlotColor;
        [SerializeField] private Color unselectedSlotColor;

        [field: Header("Visual - Tooltip UI")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(4, 4)] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public string Cost => $"No Cost";

        private UIWindow windowInstance;
        private SlotData selectedSlot;

        private class SlotData
        {
            public readonly UIWindow window;
            public readonly Item item;

            public SlotData(UIWindow window, Item item)
            {
                this.window = window;
                this.item = item;
            }
        }

        private readonly GameInstance.PauseID pauseID = new();

        public bool CanSelect(GameObject source)
        {
            return true;
        }

        public bool CanInteract(GameObject source)
        {
            return true;
        }

        public bool Interact(GameObject source)
        {
            CreateMenu(source);

            return true;
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

        private void CreateMenu(GameObject source)
        {
            GameInstance.Pause(pauseID);

            windowInstance = Instantiate(itemSelectionWindow, GameInstance.ScreenCanvas.transform);
            windowInstance.TryGetElement<Button>("Close Button").onClick.AddListener(OnCloseMenu);

            RectTransform root = windowInstance.TryGetElement<GridLayoutGroup>("Layout Group").GetComponent<RectTransform>();

            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                GenerateMenuSlots(itemHandler, root);
            }

            windowInstance.TryGetElement<Button>("Dismantle Button").onClick.AddListener(() => OnDismantle(source));
        }

        private void GenerateMenuSlots(ItemHandler itemHandler, RectTransform root)
        {
            List<Item> items = itemHandler.Items;

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                if (disallowedItems.IsDisallowed(item.Data)) continue;

                UIWindow slot = Instantiate(ItemSlotWindow, root);

                slot.TryGetElement<Image>("Icon").sprite = item.Data.Icon;
                slot.TryGetElement<TextMeshProUGUI>("Stacks").SetText("x" + item.stacks.ToString());

                Image iconOutline = slot.TryGetElement<Image>("Icon Outline");

                iconOutline.sprite = item.Data.Outline;
                iconOutline.color = itemSettings.GetRarityColor(item.Data.Rarity);

                slot.TryGetElement<Button>("Clickbox").onClick.AddListener(() => OnSlotSelected(new SlotData(slot, item)));
                slot.TryGetElement<Image>("Select Overlay").color = unselectedSlotColor;
            }
        }

        private void OnSlotSelected(SlotData slotData)
        {
            if (selectedSlot != null)
            {
                selectedSlot.window.TryGetElement<Image>("Select Overlay").color = unselectedSlotColor;
            }

            slotData.window.TryGetElement<Image>("Select Overlay").color = selectedSlotColor;
            selectedSlot = slotData;
        }

        private void OnDismantle(GameObject source)
        {
            if (selectedSlot == null) return;

            const int REMOVE_STACKS = 1;

            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                rewards.GiveRewards(selectedSlot.item.Data, itemHandler);
                itemHandler.RemoveItem(selectedSlot.item.Data, REMOVE_STACKS);
                UpdateSelectedSlot();
            }

            //check if there's any new item mb?
        }

        private void UpdateSelectedSlot()
        {
            if (selectedSlot.item.stacks == 0)
            {
                selectedSlot.window.DestroyWindow();
                selectedSlot = null;
            }
            else
            {
                selectedSlot.window.TryGetElement<TextMeshProUGUI>("Stacks").SetText("x" + selectedSlot.item.stacks.ToString());
            }
        }

        private void OnCloseMenu()
        {
            selectedSlot = null;

            //cache this somehow and then update it for optimization
            Destroy(windowInstance.gameObject);

            GameInstance.Unpause(pauseID);
        }
    }
}