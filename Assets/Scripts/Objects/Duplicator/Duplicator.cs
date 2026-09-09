using UnityEngine;
using UnityEngine.UI;
using Zeke.UI;
using TMPro;

namespace Zeke.Items
{
    public class Duplicator : MonoBehaviour, IInteractable, IInteractableTooltipRenderer
    {
        [SerializeField] private ItemSettings itemSettings;
        [SerializeField] private ItemGeneratorDrops item;
        [SerializeField] private DuplicatorCosts costs;
        [SerializeField] private ItemData itemCost;

        [field: Header("Visual - Object")]
        [field: SerializeField] public Sprite InteractOverlay { get; private set; }

        [field: Space]

        [SerializeField] private SpriteRenderer itemDisplay;
        [SerializeField] private SpriteRenderer itemOutlineDisplay;

        [field: Header("Visual - UI")]
        [SerializeField] private UIWindow duplicatorWindow;

        [field: Header("Visual - Tooltip UI")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(4, 4)] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public string Cost => $"{cost} essence";

        private int cost = 0;

        private UIWindow windowInstance;
        private ItemData reward;

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

        private void Start()
        {
            reward = RollItem();
            cost = costs.GetCost(reward);

            UpdateItemDisplay();
        }

        private ItemData RollItem()
        {
            ItemRarity rarity = WeightedSelect.SelectElement(item.RaritySlots).rarity;
            return itemSettings.GetRandomItem(rarity);
        }

        private void UpdateItemDisplay()
        {
            itemDisplay.sprite = reward.Icon;
            itemOutlineDisplay.sprite = reward.Outline;
            itemOutlineDisplay.color = itemSettings.GetRarityColor(reward.Rarity);
        }

        private void CreateMenu(GameObject source)
        {
            GameInstance.Pause(pauseID);

            windowInstance = Instantiate(duplicatorWindow, GameInstance.ScreenCanvas.transform);

            LoadCostPanelData();
            LoadRewardPanelData();
            LoadCurrencyPanelData(source);

            windowInstance.TryGetElement<Button>("Craft Button").onClick.AddListener(() => Craft(source));
            windowInstance.TryGetElement<Button>("Close Button").onClick.AddListener(OnCloseMenu);
        }

        private void LoadCostPanelData()
        {
            windowInstance.TryGetElement<Image>("Cost Icon").sprite = itemCost.Icon;
            windowInstance.TryGetElement<TextMeshProUGUI>("Cost Stacks").SetText("x" + cost);

            Image costIconOutline = windowInstance.TryGetElement<Image>("Cost Icon Outline");

            costIconOutline.sprite = itemCost.Outline;
            costIconOutline.color = itemSettings.GetRarityColor(itemCost.Rarity);

            windowInstance.TryGetElement<TextMeshProUGUI>("Cost Item Name").SetText(itemCost.Name);
        }

        private void LoadRewardPanelData()
        {
            windowInstance.TryGetElement<Image>("Reward Icon").sprite = reward.Icon;

            Image rewardIconOutline = windowInstance.TryGetElement<Image>("Reward Icon Outline");

            rewardIconOutline.sprite = reward.Outline;
            rewardIconOutline.color = itemSettings.GetRarityColor(reward.Rarity);

            windowInstance.TryGetElement<TextMeshProUGUI>("Reward Item Name").SetText(reward.Name);
        }

        private void LoadCurrencyPanelData(GameObject source)
        {
            windowInstance.TryGetElement<Image>("Currency Icon").sprite = itemCost.Icon;

            Image costIconOutline = windowInstance.TryGetElement<Image>("Currency Icon Outline");

            costIconOutline.sprite = itemCost.Outline;
            costIconOutline.color = itemSettings.GetRarityColor(itemCost.Rarity);

            UpdateCurrencyPanel(source);
        }

        private void UpdateCurrencyPanel(GameObject source)
        {
            UpdateCurrencyPanel(source.GetComponent<ItemHandler>());
        }

        private void UpdateCurrencyPanel(ItemHandler itemHandler)
        {
            int currency = 0;

            if (itemHandler != null && itemHandler.TryGetItem(itemCost, out Item item))
            {
                currency = item.stacks;
            }

            windowInstance.TryGetElement<TextMeshProUGUI>("Currency Stacks").SetText($"x{currency} {itemCost.Name}");
        }

        private void Craft(GameObject source)
        {
            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                if (itemHandler.TryGetItem(itemCost, out Item item) && item.stacks >= cost)
                {
                    itemHandler.RemoveItem(itemCost, cost);
                    itemHandler.AddItem(reward);

                    UpdateCurrencyPanel(itemHandler);
                }
            }
        }

        private void OnCloseMenu()
        {
            //cache this somehow and then update it for optimization
            Destroy(windowInstance.gameObject);

            GameInstance.Unpause(pauseID);
        }
    }
}