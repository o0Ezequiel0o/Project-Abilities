using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeke.GlobalEventsBus;
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
        [field: SerializeField] public Sprite Icon { get; private set; }

        [Header("Settings")]
        [SerializeField] protected int baseCost;

        [Header("Selection")]
        [SerializeField] protected int options = 3;

        [Header("Animation")]
        [SerializeField] private float optionAppearDuration = 0.2f;

        public string Cost => $"{cost} gold";

        protected int cost = 0;

        private UIWindow windowInstance;

        private readonly List<UIWindow> spawnedOptionWindows = new List<UIWindow>();

        private readonly Run.PauseID pauseID = new Run.PauseID();

        private bool interactionEnabled = true;

        public abstract bool CanSelect(GameObject source);

        public abstract bool CanInteract(GameObject source);

        public abstract bool CanHack(GameObject source);

        public abstract bool Interact(GameObject source);

        public abstract bool Hack(GameObject source);

        public List<ItemData> GetRandomItems(int amount)
        {
            return RollItems(amount);
        }

        public ItemData GetRandomItem()
        {
            return RollItem();
        }

        protected virtual void Start()
        {
            int newValue = Mathf.FloorToInt(baseCost * Run.Instance.CostMultiplier);

            if (baseCost >= 1 && newValue <= 0)
            {
                newValue = 0;
            }

            cost = newValue;
        }

        protected void GenerateOptions(GameObject source, int amount)
        {
            List<ItemData> generatedItems = RollItems(amount);

            GlobalEventBus.Invoke(new OnOptionGeneratedEvent(this, generatedItems));

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

        private List<ItemData> RollItems(int amount)
        {
            List<ItemData> generatedItems = new List<ItemData>(amount);

            for (int i = 0; i < amount; i++)
            {
                generatedItems.Add(RollItem());
            }

            return generatedItems;
        }

        private ItemData RollItem()
        {
            ItemRarity rarity = WeightedSelect.SelectElement(drops.RaritySlots).rarity;
            return itemSettings.GetRandomItem(rarity);
        }

        private void CreateOptionsUIWindow(List<ItemData> generatedItems, GameObject source)
        {
            windowInstance = Instantiate(itemSelectionWindow, Run.Instance.ScreenCanvas.transform);
            LayoutGroup layoutGroupRoot = windowInstance.TryGetElement<LayoutGroup>("Layout Group");

            for (int i = 0; i < generatedItems.Count; i++)
            {
                UIWindow window = CreateItemOptionUIWindow(layoutGroupRoot.transform, generatedItems[i], source);
                PlayOptionAppearAnimation(window.gameObject, optionAppearDuration * i);
                spawnedOptionWindows.Add(window);
            }

            Run.Instance.Pause(pauseID);
        }

        private UIWindow CreateItemOptionUIWindow(Transform root, ItemData generatedItem, GameObject source)
        {
            UIWindow optionWindowInstance = Instantiate(itemOptionWindow, root);

            optionWindowInstance.TryGetElement<Image>("Image").sprite = generatedItem.Icon;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Name").text = generatedItem.Name;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Description").text = generatedItem.Description;
            optionWindowInstance.TryGetElement<Image>("Border").color = itemSettings.GetRarityColor(generatedItem.Rarity);

            Button button = optionWindowInstance.TryGetElement<Button>("Button");
            button.onClick.AddListener(() => OnOptionSelected(source, generatedItem, optionWindowInstance));

            return optionWindowInstance;
        }

        private void OnOptionSelected(GameObject source, ItemData generatedItem, UIWindow selectedWindow)
        {
            if (!interactionEnabled) return;

            interactionEnabled = false;

            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                itemHandler.AddItem(generatedItem);
            }

            for (int i = 0; i < spawnedOptionWindows.Count; i++)
            {
                UIWindow window = spawnedOptionWindows[i];

                if (window == null) continue;

                if (window == selectedWindow)
                {
                    GlobalEventBus.Invoke(new OnItemSelectedEvent(this, generatedItem));

                    PlayOptionSelectAnimation(window.gameObject, () =>
                    {
                        PlayOptionDisappearAnimation(window.gameObject);
                        PlayMenuPopOutAnimation(windowInstance.gameObject, CloseOptionsWindow);
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

            Run.Instance.Unpause(pauseID);
        }

        private void PlayOptionAppearAnimation(GameObject window, float delay)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            rectTransform.localScale = Vector3.one * 0.1f;
            canvasGroup.alpha = 0f;

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: Vector3.one * 0.1f, endValue: Vector3.one, duration: optionAppearDuration, ease: Ease.OutBack, startDelay: delay))
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

        private void PlayMenuPopOutAnimation(GameObject window, Action onComplete)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: rectTransform.localScale * 1.05f, duration: 0.15f, ease: Ease.OutQuad))

                .Chain(Tween.Scale(rectTransform, startValue: rectTransform.localScale * 1.05f, endValue: Vector3.zero, duration: 0.2f, ease: Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 0f, duration: 0.15f, ease: Ease.InSine))

                .ChainCallback(() => onComplete?.Invoke());
        }

        private void PlayOptionSelectAnimation(GameObject window, Action onAnimationFinish)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: Vector3.one * 1.15f, duration: 0.25f, ease: Ease.OutBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 1f, duration: 0.2f))

                .ChainDelay(0.3f)

                .ChainCallback(() => onAnimationFinish?.Invoke());
        }

        public readonly struct OnOptionGeneratedEvent : IGlobalEvent
        {
            public readonly ItemGenerator itemGenerator;
            public readonly List<ItemData> generatedItems;

            public OnOptionGeneratedEvent(ItemGenerator itemGenerator, List<ItemData> generatedItems)
            {
                this.itemGenerator = itemGenerator;
                this.generatedItems = generatedItems;
            }
        }

        public readonly struct OnItemSelectedEvent : IGlobalEvent
        {
            public readonly ItemGenerator itemGenerator;
            public readonly ItemData itemData;

            public OnItemSelectedEvent(ItemGenerator itemGenerator, ItemData itemData)
            {
                this.itemGenerator = itemGenerator;
                this.itemData = itemData;
            }
        }
    }
}