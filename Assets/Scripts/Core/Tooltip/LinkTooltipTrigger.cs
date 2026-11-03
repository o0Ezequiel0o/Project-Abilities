using UnityEngine;
using Zeke.Tooltips;

namespace Zeke.Abilities
{
    [RequireComponent(typeof(ILinkHoverHandler))]
    public class LinkTooltipTrigger : MonoBehaviour
    {
        [SerializeField] private TooltipLinksDB tooltipLinksDB;

        private ILinkHoverHandler linkHoverEvents;

        private bool inside = false;

        private void Awake()
        {
            linkHoverEvents = GetComponent<ILinkHoverHandler>();
        }

        private void OnEnable()
        {
            linkHoverEvents.OnHoverEnterLink += OnHoverEnterLink;
            linkHoverEvents.OnHoverExitLink += OnHoverExitLink;
        }

        private void OnDisable()
        {
            linkHoverEvents.OnHoverEnterLink -= OnHoverEnterLink;
            linkHoverEvents.OnHoverExitLink -= OnHoverExitLink;

            if (inside)
            {
                Tooltip.Hide();
            }
        }

        private void OnHoverEnterLink(LinkHoverEventInfo linkHoverEventInfo)
        {
            if (inside) return;

            if (tooltipLinksDB.TryGetValue(linkHoverEventInfo.id, out TooltipLinksDB.TooltipInfo toolTipInfo))
            {
                Tooltip.Show(toolTipInfo.description);
            }

            inside = true;
        }

        private void OnHoverExitLink(LinkHoverEventInfo linkHoverEventInfo)
        {
            if (!inside) return;

            Tooltip.Hide();
            inside = false;
        }
    }
}