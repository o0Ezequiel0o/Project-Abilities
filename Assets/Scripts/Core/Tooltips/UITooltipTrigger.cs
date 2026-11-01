using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zeke.Tooltips
{
    public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Func<string> description;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.Show(description?.Invoke());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Hide();
        }
    }
}