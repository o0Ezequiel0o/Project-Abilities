using UnityEngine;

[CreateAssetMenu(fileName = "Interactable Settings", menuName = "Interaction/Interactable Settings", order = 1)]
public class InteractableSettings : ScriptableObject
{
    [field: SerializeField] public Color CanInteractOverlayColor { get; private set; }
    [field: SerializeField] public Color CantInteractOverlayColor { get; private set; }
}