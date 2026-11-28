using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zeke.Tooltips
{
    public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Func<string> description;

        private bool inside = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!inside)
            {
                Tooltip.Instance.Show(description?.Invoke());
                inside = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (inside)
            {
                Tooltip.Instance.Hide();
                inside = false;
            }
        }

        private void OnDisable()
        {
            if (inside) Tooltip.Instance.Hide();
        }
    }
}