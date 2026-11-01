using UnityEngine;
using Zeke.Tooltips;

namespace Zeke.Items
{
    [RequireComponent(typeof(ItemDisplaySlot)), RequireComponent(typeof(UITooltipTrigger))]
    public class ItemInfoDisplay : MonoBehaviour
    {
        [Header("Dependency")]
        [SerializeField] private ItemDisplaySlot itemDisplaySlot;
        [SerializeField] private UITooltipTrigger trigger;

        private void Reset()
        {
            itemDisplaySlot = GetComponent<ItemDisplaySlot>();
            trigger = GetComponent<UITooltipTrigger>();
        }

        private void Awake()
        {
            trigger.description = () => itemDisplaySlot.ItemData.Description;
        }
    }
}