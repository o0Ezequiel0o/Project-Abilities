using System.Collections.Generic;
using UnityEngine;
using Zeke.Collections;
using Zeke.UI;
using TMPro;

public class InteractionHandler : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject overlayPrefab;
    [SerializeField] private InteractableSettings settings;

    [Header("Settings")]
    [SerializeField] private float range;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkInterval;

    public GameObject SelectedInteractable => currentInteractable?.gameObject;
    public OrderedAction<InteractionResult> onInteraction = new OrderedAction<InteractionResult>();

    private GameObject overlayObjectInstance;
    private InteractableData currentInteractable;

    private float scanSurroundingsTimer = 0f;

    private UIWindow overlayWindow;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>(16);

    public void TryInteractWithClose()
    {
        currentInteractable = GetClosestUsableInteractable();

        if (currentInteractable != null)
        {
            bool success = currentInteractable.interactable.Interact(gameObject);
            onInteraction?.Invoke(new InteractionResult(currentInteractable.gameObject, success));
        }
    }

    private void Awake()
    {
        overlayObjectInstance = Instantiate(overlayPrefab, transform.position, transform.rotation);
        overlayWindow = overlayObjectInstance.GetComponent<UIWindow>();
        overlayObjectInstance.SetActive(false);
    }

    private void Update()
    {
        scanSurroundingsTimer += Time.deltaTime;

        if (scanSurroundingsTimer > checkInterval)
        {
            DisplayClosestUsableInteractable();
            scanSurroundingsTimer = 0f;
        }
    }

    private void LateUpdate()
    {
        if (currentInteractable == null) return;

        if (currentInteractable.gameObject != null)
        {
            UpdateInteractableDisplay(currentInteractable);
        }
        else
        {
            HideInteractableOverlay(currentInteractable);
            currentInteractable = null;
        }
    }

    private void OnDestroy()
    {
        Destroy(overlayObjectInstance);
    }

    private InteractableData GetClosestUsableInteractable()
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layer, useLayerMask = true, useTriggers = true };
        Physics2D.CircleCast(transform.position, range, Vector2.zero, contactFilter, hits, 0f);

        for (int i = 0; i < hits.Count; i++)
        {
            Vector2 relativeDirection = hits[i].transform.position - transform.position;

            if (Vector2.Angle(relativeDirection, transform.up) > angle) continue;

            if (hits[i].collider.TryGetComponent(out IInteractable interactable) && interactable.CanSelect(gameObject))
            {
                return new InteractableData(interactable, hits[i].collider.gameObject, overlayWindow);
            }
        }

        return null;
    }

    private void DisplayClosestUsableInteractable()
    {
        InteractableData closestInteractable = GetClosestUsableInteractable();

        if (closestInteractable != null && closestInteractable != currentInteractable)
        {
            DisplayInteractableOverlay(closestInteractable);
        }
        else if (currentInteractable != null)
        {
            HideInteractableOverlay(currentInteractable);
        }

        currentInteractable = closestInteractable;
    }

    private void DisplayInteractableOverlay(InteractableData interactableData)
    {
        if (overlayObjectInstance.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.sprite = interactableData.interactable.InteractOverlay;
        }

        UpdateInteractableDisplay(interactableData);
        overlayObjectInstance.SetActive(true);
    }

    private void HideInteractableOverlay(InteractableData interactableData)
    {
        overlayObjectInstance.SetActive(false);
    }

    private void UpdateInteractableDisplay(InteractableData interactableData)
    {
        if (overlayObjectInstance.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            if (interactableData.interactable.CanInteract(gameObject))
            {
                spriteRenderer.color = settings.CanInteractOverlayColor;
            }
            else
            {
                spriteRenderer.color = settings.CantInteractOverlayColor;
            }
        }

        string toolTip = interactableData.interactable.InteractTooltip;

        if (string.IsNullOrEmpty(toolTip)) toolTip = "";

        interactableData.window.TryGetElement<TextMeshProUGUI>("Tooltip").text = toolTip;

        overlayObjectInstance.transform.SetPositionAndRotation(
            interactableData.gameObject.transform.position,
            interactableData.gameObject.transform.rotation);

        overlayObjectInstance.transform.localScale = interactableData.gameObject.transform.localScale;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(255f, 165f, 0f);
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, angle) * transform.up * range);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, -angle) * transform.up * range);
    }

    private class InteractableData
    {
        public IInteractable interactable;
        public GameObject gameObject;
        public UIWindow window;

        public InteractableData(IInteractable interactable, GameObject gameObject, UIWindow window)
        {
            this.interactable = interactable;
            this.gameObject = gameObject;
            this.window = window;
        }
    }
}