using System.Collections.Generic;
using UnityEngine;

namespace Zeke.UI
{
    public class UIWindow : MonoBehaviour
    {
        private readonly Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();

        public UIElement TryGetElement(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (elements.TryGetValue(name, out UIElement uiElement))
            {
                return uiElement;
            }

            return null;
        }

        public T TryGetElement<T>(string name) where T : Component
        {
            if (elements.TryGetValue(name, out UIElement uiElement))
            {
                return uiElement.GetElement<T>();
            }

            return null;
        }

        public void Add(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Name))
            {
                Debug.LogWarning("Attempted to add a null or unnamed UIElement.");
                return;
            }

            if (elements.ContainsKey(element.Name))
            {
                Debug.LogWarning($"Duplicate '{element.Name}' UIElement naming:", element);
            }

            elements.Add(element.Name, element);
        }

        public void Remove(UIElement element)
        {
            if (element == null || string.IsNullOrEmpty(element.Name)) return;

            if (!elements.TryGetValue(element.Name, out var registeredElement) || registeredElement != element)
            {
                Debug.Log($"{element} (key: {element.Name}) is not registered to this UIWindow.");
                return;
            }

            elements.Remove(element.Name);
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }
}