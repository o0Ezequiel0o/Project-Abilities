using UnityEngine;
using UnityEngine.UI;
using Zeke.UI;
using TMPro;

public class InteractableTooltipScreenLoader : MonoBehaviour
{
    [SerializeField] private UIWindow window;

    private InteractionHandler interactionHandler;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        bool playerHasRequiredComponents = newPlayer.TryGetComponent(out interactionHandler);

        if (!playerHasRequiredComponents) return;

        interactionHandler.onInteractableSelected += OnInteractableSelected;
        interactionHandler.onInteractableUnselected += OnInteractableUnselected;
    }

    public void OnInteractableSelected(GameObject interactable)
    {
        if (interactable.TryGetComponent(out IInteractableTooltipRenderer renderer))
        {
            UpdateWindowDisplay(renderer);
            window.SetGraphicVisibility(true);
        }
        else if (window.IsVisible)
        {
            window.SetGraphicVisibility(false);
        }
    }

    public void OnInteractableUnselected(GameObject interactable)
    {
        window.SetGraphicVisibility(false);
    }

    private void UpdateWindowDisplay(IInteractableTooltipRenderer renderer)
    {
        window.TryGetElement<TextMeshProUGUI>("Name").SetText(renderer.Name);
        window.TryGetElement<TextMeshProUGUI>("Cost").SetText(renderer.Cost);

        window.TryGetElement<Image>("Icon").sprite = renderer.Icon;
    }
}