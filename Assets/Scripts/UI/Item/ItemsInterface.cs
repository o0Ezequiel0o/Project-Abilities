using PrimeTween;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeke.UI;
using System;

namespace Zeke.Items
{
    public class ItemsInterface : MonoBehaviour
    {
        [SerializeField] protected UIWindow window;
        [SerializeField] protected UIWindow itemSlotPrefab;

        public UIWindow Window => window;

        protected readonly Dictionary<ItemData, UIWindow> itemSlots = new Dictionary<ItemData, UIWindow>();

        public Action<UIWindow> onItemModified;

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

            CenterOnItem(windowInstance.gameObject.GetComponent<RectTransform>(), window.TryGetElement<ScrollRect>("Scroll Rect"));

            PlaySlotUpdateAnimation(windowInstance.gameObject);

            onItemModified?.Invoke(windowInstance);

            return windowInstance;
        }

        public virtual UIWindow RemoveItem(ItemData itemData)
        {
            if (itemSlots.TryGetValue(itemData, out UIWindow windowInstance))
            {
                itemSlots.Remove(itemData);
                Destroy(windowInstance.gameObject);

                onItemModified?.Invoke(windowInstance);

                return windowInstance;
            }

            return null;
        }

        public virtual UIWindow UpdateItemStacks(ItemHandler.ItemStackUpdate stackUpdate)
        {
            UIWindow windowInstance = itemSlots[stackUpdate.itemData];

            windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference").Stacks = stackUpdate.stacks;
            windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").SetText(GetStacksString(stackUpdate.stacks));

            LayoutRebuilder.ForceRebuildLayoutImmediate(window.TryGetElement<LayoutGroup>("Layout Group").GetComponent<RectTransform>());
            CenterOnItem(windowInstance.gameObject.GetComponent<RectTransform>(), window.TryGetElement<ScrollRect>("Scroll Rect"));

            PlaySlotUpdateAnimation(windowInstance.gameObject);

            onItemModified?.Invoke(windowInstance);

            return windowInstance;
        }

        protected void PlaySlotUpdateAnimation(GameObject element)
        {
            if (!element.activeInHierarchy) return;

            RectTransform rectTransform = element.GetComponent<RectTransform>();

            Tween.ShakeLocalRotation(rectTransform, strength: new Vector3(0f, 0f, 45f), duration: 1f, frequency: 5);
        }

        protected virtual void CenterOnItem(RectTransform target, ScrollRect scrollRect)
        {
            if (target == null || scrollRect == null) return;

            RectTransform targetRect = target;
            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            if (targetRect == null || content == null || viewport == null) return;

            Vector3 worldCenter = targetRect.TransformPoint(targetRect.rect.center);
            Vector3 viewportCenter = viewport.InverseTransformPoint(worldCenter);

            Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);

            float offset = viewportCenter.x - viewport.rect.center.x;
            float scrollableWidth = contentBounds.size.x - viewport.rect.width;

            float normalizedPosition = scrollRect.horizontalNormalizedPosition + offset / scrollableWidth;

            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        protected string GetStacksString(int stacks) => "x" + stacks.ToString();
    }
}