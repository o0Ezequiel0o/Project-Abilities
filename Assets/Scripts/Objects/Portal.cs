using UnityEngine;

public class Portal : MonoBehaviour, IInteractable, IInteractableTooltipRenderer
{
    [field: Header("Visual - Tooltip UI")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    public string Cost => "No Cost";

    private bool activated = false;

    public bool CanInteract(GameObject source) => !activated;
    public bool CanSelect(GameObject source) => !activated;

    public bool Interact(GameObject source)
    {
        GlobalEventBus.Invoke(new GameLevelHandler.LoadNextLevelEvent());

        Destroy(gameObject);
        activated = true;

        return true;
    }
}