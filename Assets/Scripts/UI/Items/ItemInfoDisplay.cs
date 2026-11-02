using UnityEngine;
using Zeke.Tooltips;

namespace Zeke.Items
{
    [RequireComponent(typeof(ItemSlotReference)), RequireComponent(typeof(UITooltipTrigger))]
    public class ItemInfoDisplay : MonoBehaviour
    {
        [Header("Dependency")]
        [SerializeField] private ItemSlotReference itemSlotReference;
        [SerializeField] private UITooltipTrigger trigger;

        private void Reset()
        {
            itemSlotReference = GetComponent<ItemSlotReference>();
            trigger = GetComponent<UITooltipTrigger>();
        }

        private void Awake()
        {
            trigger.description = () => itemSlotReference.ItemData.Description;
        }
    }
}