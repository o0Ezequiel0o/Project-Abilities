using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private GameObject graphicRoot;

        [HideInInspector, SerializeField] private Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();

        public IReadOnlyDictionary<string, UIElement> Elements => elements;

        public bool IsVisible => graphicRoot != null ? graphicRoot.activeSelf : gameObject.activeSelf;

        public Action<UIWindow> onWindowOpened;
        public Action<UIWindow> onWindowClosed;

        private GraphicVisibilityTracker graphicVisibilityTracker;

        public void SetGraphicVisibility(bool state)
        {
            if (graphicRoot == null) return;
            if (state == graphicRoot.activeSelf) return;

            graphicRoot.SetActive(state);
        }

        public UIElement TryGetElement(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            elements.TryGetValue(name, out UIElement uiElement);
            return uiElement;
        }

        public T TryGetElement<T>(string name) where T : Component
        {
            if (elements.TryGetValue(name, out UIElement uiElement))
            {
                return uiElement.GetElement<T>();
            }
            return null;
        }

        public bool Contains(UIElement element)
        {
            return elements.ContainsKey(element.Name);
        }

        public void Add(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Name)) return;

            if (elements.ContainsKey(element.Name))
            {
                Debug.LogWarning($"Duplicate '{element.Name}' UIElement naming:", element);
                return;
            }

            elements.Add(element.Name, element);
        }

        public void Remove(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Name)) return;

            if (elements.TryGetValue(element.Name, out var registeredElement) && registeredElement == element)
            {
                elements.Remove(element.Name);
            }
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        public void SetDictionary(Dictionary<string, UIElement> newElements)
        {
            elements = newElements;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private void Awake()
        {
            if (graphicRoot == null) return;

            if (!graphicRoot.TryGetComponent(out graphicVisibilityTracker))
            {
                graphicVisibilityTracker = graphicRoot.AddComponent<GraphicVisibilityTracker>();
            }
        }

        private void OnEnable()
        {
            if (graphicVisibilityTracker == null) return;

            graphicVisibilityTracker.onEnable += TriggerGraphicEnabledEvents;
            graphicVisibilityTracker.onDisable += TriggerGraphicDisabledEvents;
        }

        private void OnDisable()
        {
            if (graphicVisibilityTracker == null) return;

            graphicVisibilityTracker.onEnable -= TriggerGraphicEnabledEvents;
            graphicVisibilityTracker.onDisable -= TriggerGraphicDisabledEvents;
        }

        private void TriggerGraphicEnabledEvents()
        {
            onWindowOpened?.Invoke(this);
        }

        private void TriggerGraphicDisabledEvents()
        {
            onWindowClosed?.Invoke(this);
        }
    }
}