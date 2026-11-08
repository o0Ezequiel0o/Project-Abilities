using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeke.UI;

namespace Zeke.Items
{
    public class ItemsHUDInterface : ItemsInterface
    {
        [SerializeField] protected UIWindow itemAddAnimationPrefab;

        [Space]

        [SerializeField] protected GameObject dummyWindowPrefab;

        [Header("Visual - Modified Animation")]
        [SerializeField, Min(0)] private float maxRotation = 20f;
        [SerializeField, Min(0)] private float effectDuration = 1f;
        [SerializeField, Min(0)] private int frequency = 5;

        [Header("Visual - Removed Animation")]
        [SerializeField] private float jumpHeight = 50f;
        [SerializeField] private float fallDepth = 200f;
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private float rotationAmount = 180f;

        [Header("Visual - Added Animation")]
        [SerializeField] private float flyInDuration = 0.5f;
        [SerializeField] private float startVerticalOffset = -150f;
        [SerializeField] private float xOffsetPerSpawn = 100f;
        [SerializeField] private float xOffsetDecreaseRate = 50f;

        public Action<UIWindow> onItemModified;

        protected readonly Dictionary<ItemData, GameObject> dummyWindows = new Dictionary<ItemData, GameObject>();

        private float xOffset = 0f;

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

        public override UIWindow AddItem(ItemData itemData, int stacks)
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

            OnItemModified(itemData);

            onItemModified?.Invoke(windowInstance);

            return windowInstance;
        }

        public override UIWindow RemoveItem(ItemData itemData)
        {
            if (itemSlots.TryGetValue(itemData, out UIWindow windowInstance))
            {
                itemSlots.Remove(itemData);
                onItemModified?.Invoke(windowInstance);

                OnItemRemoved(itemData, windowInstance);

                return windowInstance;
            }

            return null;
        }

        public override UIWindow UpdateItemStacks(ItemHandler.ItemStackUpdate stackUpdate)
        {
            UIWindow windowInstance = itemSlots[stackUpdate.itemData];

            windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference").Stacks = stackUpdate.stacks;
            windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").SetText(GetStacksString(stackUpdate.stacks));

            LayoutRebuilder.ForceRebuildLayoutImmediate(window.TryGetElement<LayoutGroup>("Layout Group").GetComponent<RectTransform>());

            OnItemModified(stackUpdate.itemData);

            if (stackUpdate.amount > 0)
            {
                if (stackUpdate.stacks != 1)
                {
                    OnStacksAdded(stackUpdate.itemData);
                }
                else
                {
                    OnItemAdded(stackUpdate.itemData);
                }
            }
            else if (stackUpdate.amount < 0 && stackUpdate.stacks != 0)
            {
                OnStacksRemoved(stackUpdate.itemData);
            }

            onItemModified?.Invoke(windowInstance);

            return windowInstance;
        }

        protected virtual void OnItemModified(ItemData itemData)
        {
            CenterOnItem(itemData);
        }

        protected virtual void OnStacksAdded(ItemData itemData)
        {
            PlaySlotUpdateAnimation(itemSlots[itemData].gameObject);
            CreateFlyInEffect(itemData);
        }

        protected virtual void OnStacksRemoved(ItemData itemData)
        {
            UIWindow windowInstance = itemSlots[itemData];
            LayoutGroup layoutGroup = window.TryGetElement<LayoutGroup>("Layout Group");

            int siblingIndex = windowInstance.transform.GetSiblingIndex();

            GameObject copy = Instantiate(windowInstance.gameObject, layoutGroup.transform, false);
            RectTransform copyRect = copy.GetComponent<RectTransform>();

            copyRect.SetSiblingIndex(siblingIndex);

            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());

            copyRect.SetParent(window.transform, true);

            PlayDropAnimation(copyRect);
        }

        protected virtual void OnItemAdded(ItemData itemData)
        {
            CreateFlyInEffect(itemData);
        }

        private void CreateFlyInEffect(ItemData itemData)
        {
            RectTransform root = window.GetComponent<RectTransform>();

            UIWindow windowInstance = Instantiate(itemAddAnimationPrefab, root.transform);
            RectTransform itemRect = windowInstance.GetComponent<RectTransform>();

            if (xOffset > root.rect.width - itemRect.rect.width * 0.5f)
            {
                xOffset = 0f;
            }

            windowInstance.TryGetElement<Image>("Icon").sprite = itemData.Icon;
            windowInstance.TryGetElement<Image>("Outline").sprite = itemData.Outline;

            CanvasGroup itemCanvasGroup = windowInstance.GetComponent<CanvasGroup>();

            Vector3 targetPosition = new Vector3(xOffset + itemRect.rect.width * 0.5f, itemRect.rect.height * 0.5f, 0f);
            itemRect.position += new Vector3(xOffset + itemRect.rect.width * 0.5f, startVerticalOffset, 0f);

            itemCanvasGroup.alpha = 0f;

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.UIAnchoredPosition(itemRect, targetPosition, flyInDuration, Ease.OutQuad))
                .Group(Tween.Alpha(itemCanvasGroup, 1f, flyInDuration * 0.25f, Ease.InSine))
                .Group(Tween.Alpha(itemCanvasGroup, 0f, flyInDuration * 0.6f, Ease.InSine, startDelay: flyInDuration * 0.25f))
                .OnComplete(() =>
                {
                    windowInstance.DestroyWindow();
                });

            xOffset += xOffsetPerSpawn;
        }

        protected virtual void OnItemRemoved(ItemData itemData, UIWindow windowInstance)
        {
            RectTransform layoutGroup = window.TryGetElement<LayoutGroup>("Layout Group").GetComponent<RectTransform>();
            RectTransform rectTransform = windowInstance.GetComponent<RectTransform>();

            int siblingIndex = rectTransform.GetSiblingIndex();

            PlayDropAnimation(rectTransform, (itemRect) => Destroy(itemRect.gameObject));
            CreateDummyItemSlotAtIndex(layoutGroup, siblingIndex);
        }

        protected void CenterOnItem(ItemData itemData)
        {
            RectTransform rectTransform = itemSlots[itemData].gameObject.GetComponent<RectTransform>();
            ScrollRect scrollRect = window.TryGetElement<ScrollRect>("Scroll Rect");

            if (rectTransform == null || scrollRect == null) return;

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            if (rectTransform == null || content == null || viewport == null) return;

            Vector3 worldCenter = rectTransform.TransformPoint(rectTransform.rect.center);
            Vector3 viewportCenter = viewport.InverseTransformPoint(worldCenter);

            Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);

            float offset = viewportCenter.x - viewport.rect.center.x;
            float scrollableWidth = contentBounds.size.x - viewport.rect.width;

            float normalizedPosition = scrollRect.horizontalNormalizedPosition + offset / scrollableWidth;

            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        protected void PlaySlotUpdateAnimation(GameObject element)
        {
            if (!element.activeInHierarchy) return;

            RectTransform rectTransform = element.GetComponent<RectTransform>();

            Tween.ShakeLocalRotation(rectTransform, strength: new Vector3(0f, 0f, maxRotation), duration: effectDuration, frequency: frequency);
        }

        private void Update()
        {
            xOffset = Mathf.Max(0, xOffset - xOffsetDecreaseRate * Time.deltaTime);
        }

        private void PlayDropAnimation(RectTransform itemRect)
        {
            PlayDropAnimation(itemRect, null);
        }

        private void PlayDropAnimation(RectTransform itemRect, Action<RectTransform> callback)
        {
            CanvasGroup canvasGroup = itemRect.GetComponent<CanvasGroup>();

            itemRect.SetParent(window.GetComponent<RectTransform>(), true);

            float randomizedDuration = duration * UnityEngine.Random.Range(0.75f, 1f);
            float randomizedJump = jumpHeight * UnityEngine.Random.Range(0.7f, 1.3f);
            float randomizedRotation = rotationAmount * UnityEngine.Random.Range(0.2f, 5f);

            float randomizedAlphaDuration = randomizedDuration * UnityEngine.Random.Range(0.75f, 1f);

            float randomXOffset = UnityEngine.Random.Range(-30f, 30f);

            Vector2 startPos = itemRect.anchoredPosition;
            Vector2 peakPos = startPos + new Vector2(randomXOffset * 0.5f, randomizedJump);
            Vector2 endPos = startPos + new Vector2(randomXOffset, -fallDepth);

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.UIAnchoredPosition(itemRect, peakPos, randomizedDuration * 0.3f, Ease.OutQuad))
                .Chain(Tween.UIAnchoredPosition(itemRect, endPos, randomizedDuration * 0.7f, Ease.InQuad))
                .Group(Tween.Rotation(itemRect, Quaternion.Euler(0, 0, randomizedRotation), randomizedDuration * 0.7f, Ease.InQuad))
                .Group(Tween.Alpha(canvasGroup, 0f, randomizedAlphaDuration, Ease.InSine))
                .OnComplete(() =>
                {
                    callback?.Invoke(itemRect);
                });;
        }

        private void CreateDummyItemSlotAtIndex(RectTransform root, int index)
        {
            GameObject dummy = Instantiate(dummyWindowPrefab, root);
            RectTransform dummyRect = dummy.GetComponent<RectTransform>();

            dummy.transform.SetSiblingIndex(index);
            float startingWidth = dummyRect.rect.width;

            Tween.Custom(dummy, startingWidth, 0f, duration, (target, val) => { dummyRect.sizeDelta = new Vector2(val, dummyRect.sizeDelta.y); }, Ease.InExpo, useUnscaledTime: true)
                .OnComplete(() =>
                {
                    Destroy(dummy);
                });
        }
    }
}