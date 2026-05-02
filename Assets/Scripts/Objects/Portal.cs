using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string InteractTooltip { get; private set; }

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