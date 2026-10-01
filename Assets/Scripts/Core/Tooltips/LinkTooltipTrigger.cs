using UnityEngine;
using Zeke.Tooltips;

namespace Zeke.Abilities
{
    [RequireComponent(typeof(ILinkHoverEvents))]
    public class LinkTooltipTrigger : MonoBehaviour
    {
        [SerializeField] private TooltipLinksDB tooltipLinksDB;

        private void Awake()
        {
            ILinkHoverEvents linkHoverEvents = GetComponent<ILinkHoverEvents>();

            linkHoverEvents.OnHoverEnterLink += OnHoverEnterLink;
            linkHoverEvents.OnHoverExitLink += OnHoverExitLink;
        }

        private void OnHoverEnterLink(LinkHoverEventInfo linkHoverEventInfo)
        {
            if (tooltipLinksDB.TryGetValue(linkHoverEventInfo.id, out TooltipLinksDB.TooltipInfo toolTipInfo))
            {
                Tooltip.Show(toolTipInfo.description);
            }
        }

        private void OnHoverExitLink(LinkHoverEventInfo linkHoverEventInfo)
        {
            Tooltip.Hide();
        }
    }
}