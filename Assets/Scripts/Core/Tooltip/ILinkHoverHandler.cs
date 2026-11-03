using System;

namespace Zeke.Tooltips
{
	public interface ILinkHoverHandler
    {
        public Action<LinkHoverEventInfo> OnHoverEnterLink { get; set; }
        public Action<LinkHoverEventInfo> OnHoverStayLink { get; set; }
        public Action<LinkHoverEventInfo> OnHoverExitLink { get; set; }
    }
}