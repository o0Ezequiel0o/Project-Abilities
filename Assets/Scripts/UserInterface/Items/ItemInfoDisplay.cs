using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(ItemDisplaySlot))]
public class ItemInfoDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Dependency")]
    [SerializeField] private ItemDisplaySlot itemDisplaySlot;
    [SerializeField] private ItemInfoDisplaySlot itemInfoDisplaySlotPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 offset;

    private ItemInfoDisplaySlot itemInfoDisplaySlotInstance;

    private void Reset()
    {
        itemDisplaySlot = GetComponentInChildren<ItemDisplaySlot>();
    }

    private void OnDisable()
    {
        if (itemInfoDisplaySlotInstance != null)
        {
            Destroy(itemInfoDisplaySlotInstance.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (itemInfoDisplaySlotInstance != null)
        {
            Destroy(itemInfoDisplaySlotInstance.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoDisplaySlotInstance = Instantiate(itemInfoDisplaySlotPrefab, GameInstance.ScreenCanvas.transform);
        itemInfoDisplaySlotInstance.transform.position = transform.position + offset;
        itemInfoDisplaySlotInstance.SetData(itemDisplaySlot.ItemData.Name, itemDisplaySlot.ItemData.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemInfoDisplaySlotInstance != null)
        {
            Destroy(itemInfoDisplaySlotInstance.gameObject);
        }
    }
}