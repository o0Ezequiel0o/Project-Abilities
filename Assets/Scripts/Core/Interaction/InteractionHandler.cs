using UnityEngine;
using System;

public class InteractionHandler : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject overlayPrefab;

    [Header("Settings")]
    [SerializeField] private float range;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkInterval;

    public GameObject SelectedInteractable => currentInteractable?.gameObject;
    public Action<GameObject> onInteraction;

    private RaycastHit2D[] hits;
    private GameObject overlayObjectInstance;
    private InteractableData currentInteractable;

    private float scanSurroundingsTimer = 0f;

    public void TryInteractWithClose()
    {
        currentInteractable = GetClosestUsableInteractable();
        currentInteractable?.interactable.Interact(gameObject);
        onInteraction?.Invoke(currentInteractable.gameObject);
    }

    private void Awake()
    {
        overlayObjectInstance = Instantiate(overlayPrefab, transform.position, transform.rotation);
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
        hits = Physics2D.CircleCastAll(transform.position, range, Vector2.zero, 0f, layer);

        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 relativeDirection = hits[i].transform.position - transform.position;

            if (Vector2.Angle(relativeDirection, transform.up) > angle) continue;

            if (hits[i].collider.TryGetComponent(out IInteractable interactable) && interactable.CanSelect(gameObject))
            {
                return new InteractableData(interactable, hits[i].collider.gameObject);
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
                spriteRenderer.color = interactableData.interactable.CanInteractOverlayColor;
            }
            else
            {
                spriteRenderer.color = interactableData.interactable.CantInteractOverlayColor;
            }
        }

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

        public InteractableData(IInteractable interactable, GameObject gameObject)
        {
            this.interactable = interactable;
            this.gameObject = gameObject;
        }
    }
}