using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeke.Collections;
using Zeke.UI;

namespace Zeke.Items
{
    public abstract class ItemGenerator : MonoBehaviour, IInteractable, IInteractableTooltipRenderer
    {
        [SerializeField] protected ItemSettings itemSettings;
        [SerializeField] protected ItemGeneratorDrops drops;

        [field: Header("Visual - Object")]
        [field: SerializeField] public Sprite InteractOverlay { get; private set; }

        [field: Header("Visual - UI")]
        [SerializeField] private UIWindow itemSelectionWindow;
        [SerializeField] private UIWindow itemOptionWindow;

        [field: Header("Visual - Tooltip UI")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(4, 4)] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [Header("Settings")]
        [SerializeField] protected int baseCost;

        [Header("Selection")]
        [SerializeField] protected int options = 3;

        public static OrderedActionDictionary<GameObject, List<ItemGenerationData>> onOptionsGenerated = new OrderedActionDictionary<GameObject, List<ItemGenerationData>>();
        public static OrderedActionDictionary<GameObject, ItemGenerationData> onItemSelected = new OrderedActionDictionary<GameObject, ItemGenerationData>();

        public string Cost => $"{cost} gold";

        protected int cost = 0;

        private UIWindow windowInstance;

        private readonly List<UIWindow> spawnedOptionWindows = new List<UIWindow>();

        private readonly GameInstance.PauseID pauseID = new();

        private bool interactionEnabled = true;

        public abstract bool CanSelect(GameObject source);

        public abstract bool CanInteract(GameObject source);

        public abstract bool CanHack(GameObject source);

        public abstract bool Interact(GameObject source);

        public abstract bool Hack(GameObject source);

        public List<ItemGenerationData> GetRandomItems(int amount)
        {
            return RollItems(amount);
        }

        public ItemGenerationData GetRandomItem()
        {
            return RollItem();
        }

        protected virtual void Start()
        {
            int newValue = Mathf.FloorToInt(baseCost * GameInstance.CostMultiplier);

            if (baseCost >= 1 && newValue <= 0)
            {
                newValue = 0;
            }

            cost = newValue;
        }

        protected void GenerateOptions(GameObject source, int amount)
        {
            List<ItemGenerationData> generatedItems = RollItems(amount);
            onOptionsGenerated?.Invoke(source, generatedItems);
            CreateOptionsUIWindow(generatedItems, source);
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

        private List<ItemGenerationData> RollItems(int amount)
        {
            List<ItemGenerationData> generatedItems = new List<ItemGenerationData>(amount);

            for (int i = 0; i < amount; i++)
            {
                generatedItems.Add(RollItem());
            }

            return generatedItems;
        }

        private ItemGenerationData RollItem()
        {
            ItemRarity rarity = WeightedSelect.SelectElement(drops.RaritySlots).rarity;
            ItemData item = itemSettings.GetRandomItem(rarity);
            return new ItemGenerationData(item, this);
        }

        private void CreateOptionsUIWindow(List<ItemGenerationData> generatedItems, GameObject source)
        {
            windowInstance = Instantiate(itemSelectionWindow, GameInstance.ScreenCanvas.transform);
            LayoutGroup layoutGroupRoot = windowInstance.TryGetElement<LayoutGroup>("Layout Group");

            for (int i = 0; i < generatedItems.Count; i++)
            {
                UIWindow window = CreateItemOptionUIWindow(layoutGroupRoot.transform, generatedItems[i], source);
                PlayOptionAppearAnimation(window.gameObject, 0.25f * i);
                spawnedOptionWindows.Add(window);
            }

            GameInstance.Pause(pauseID);
        }

        private UIWindow CreateItemOptionUIWindow(Transform root, ItemGenerationData generatedItem, GameObject source)
        {
            UIWindow optionWindowInstance = Instantiate(itemOptionWindow, root);

            optionWindowInstance.TryGetElement<Image>("Image").sprite = generatedItem.item.Icon;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Name").text = generatedItem.item.Name;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Description").text = generatedItem.item.Description;
            optionWindowInstance.TryGetElement<Image>("Border").color = itemSettings.GetRarityColor(generatedItem.item.Rarity);

            Button button = optionWindowInstance.TryGetElement<Button>("Button");
            button.onClick.AddListener(() => OnOptionSelected(source, generatedItem, optionWindowInstance));

            return optionWindowInstance;
        }

        private void PlayOptionAppearAnimation(GameObject window, float delay)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            rectTransform.localScale = Vector3.one * 0.1f;
            canvasGroup.alpha = 0f;

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: Vector3.one * 0.1f, endValue: Vector3.one, duration: 0.25f, ease: Ease.OutBack, startDelay: delay))
                .Group(Tween.Alpha(canvasGroup, startValue: 0f, endValue: 1f, duration: 0.15f, ease: Ease.OutSine, startDelay: delay));
        }

        private void PlayOptionDisappearAnimation(GameObject window)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: Vector3.zero, duration: 0.3f, ease: Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 0f, duration: 0.3f, ease: Ease.InSine));
        }

        private void PlayOptionSelectAnimation(GameObject window, Action onAnimationFinish)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: Vector3.one * 1.15f, duration: 0.25f, ease: Ease.OutBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 1f, duration: 0.2f))

                .ChainDelay(0.3f)

                .Chain(Tween.Scale(rectTransform, startValue: Vector3.one * 1.15f, endValue: Vector3.zero, duration: 0.2f, ease: Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, startValue: 1f, endValue: 0f, duration: 0.15f, ease: Ease.InSine))

                .ChainCallback(() => onAnimationFinish?.Invoke());
        }

        private void OnOptionSelected(GameObject source, ItemGenerationData generatedItem, UIWindow selectedWindow)
        {
            if (!interactionEnabled) return;

            interactionEnabled = false;

            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                itemHandler.AddItem(generatedItem.item);
            }

            for (int i = 0; i < spawnedOptionWindows.Count; i++)
            {
                UIWindow window = spawnedOptionWindows[i];

                if (window == null) continue;

                if (window == selectedWindow)
                {
                    PlayOptionSelectAnimation(window.gameObject, () =>
                    {
                        onItemSelected?.Invoke(source, generatedItem);
                        CloseOptionsWindow();
                    });
                }
                else
                {
                    PlayOptionDisappearAnimation(window.gameObject);
                }
            }
        }

        private void CloseOptionsWindow()
        {
            if (windowInstance != null)
            {
                windowInstance.DestroyWindow();
            }

            GameInstance.Unpause(pauseID);
        }

        public class ItemGenerationData
        {
            public ItemData item;
            public ItemGenerator itemGenerator;

            public ItemGenerationData(ItemData item, ItemGenerator itemGenerator)
            {
                this.item = item;
                this.itemGenerator = itemGenerator;
            }
        }
    }
}