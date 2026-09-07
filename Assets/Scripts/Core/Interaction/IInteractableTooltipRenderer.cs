using UnityEngine;

public interface IInteractableTooltipRenderer
{
    public Sprite Icon { get; }
    public string Name { get; }
    public string Cost { get; }
    public string Description { get; }
}