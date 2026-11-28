using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Zeke.UI
{
    [DefaultExecutionOrder(-1)]
    public class UIWindowInteractionController : MonoBehaviour
    {
        [SerializeField] private List<Element> elements;

        [Serializable]
        public struct Element
        {
            [field: SerializeField] public UIWindow Window { get; private set; }
            [field: SerializeField] public Dictionary<TriggerMode, UnityEvent> Interactions { get; private set; }
        }

        public enum TriggerMode
        {
            OnShow,
            OnHide,
        }

        private void Awake()
        {
            
        }
    }
}