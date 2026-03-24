using UnityEngine;

public interface IInteractable
{
    public Sprite InteractOverlay { get; }

    public Color CanInteractOverlayColor { get; }
    public Color CantInteractOverlayColor { get; }

    public bool CanSelect(GameObject source);

    public bool CanInteract(GameObject source);

    public bool Interact(GameObject source);
}