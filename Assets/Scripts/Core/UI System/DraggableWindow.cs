using UnityEngine;
using UnityEngine.EventSystems;

namespace Zeke.UI
{
    public class DraggableWindow : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform grabBounds;

        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            grabBounds.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}