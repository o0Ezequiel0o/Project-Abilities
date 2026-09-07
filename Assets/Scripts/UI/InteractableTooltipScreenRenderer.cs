using UnityEngine;
using Zeke.UI;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(InteractionHandler))]
public class InteractableTooltipScreenRenderer : MonoBehaviour
{
    [SerializeField] private InteractionHandler handler;
    [SerializeField] private UIWindow windowPrefab;

    private UIWindow windowInstance;

    public void OnInteractableSelected(GameObject interactable)
    {
        if (interactable.TryGetComponent(out IInteractableTooltipRenderer renderer))
        {
            UpdateWindowDisplay(renderer);
            windowInstance.gameObject.SetActive(true);
        }
        else if (windowInstance.gameObject.activeSelf)
        {
            windowInstance.gameObject.SetActive(false);
        }
    }

    public void OnInteractableUnselected(GameObject interactable)
    {
        windowInstance.gameObject.SetActive(false);
    }

    private void Reset()
    {
        handler = GetComponent<InteractionHandler>();
    }

    private void Awake()
    {
        windowInstance = Instantiate(windowPrefab, GameInstance.ScreenCanvas.transform);

        handler.onInteractableSelected += OnInteractableSelected;
        handler.onInteractableUnselected += OnInteractableUnselected;

        windowInstance.gameObject.SetActive(false);
    }

    private void UpdateWindowDisplay(IInteractableTooltipRenderer renderer)
    {
        windowInstance.TryGetElement<TextMeshProUGUI>("Name").SetText(renderer.Name);
        windowInstance.TryGetElement<TextMeshProUGUI>("Cost").SetText(renderer.Cost);
        windowInstance.TryGetElement<TextMeshProUGUI>("Description").SetText(renderer.Description);

        windowInstance.TryGetElement<Image>("Icon").sprite = renderer.Icon;
    }
}