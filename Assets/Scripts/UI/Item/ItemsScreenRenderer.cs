using UnityEngine;

namespace Zeke.Items
{
    [RequireComponent(typeof(ItemHandler))]
    public class ItemsScreenRenderer : MonoBehaviour
    {
        [SerializeField] private ItemsInterface itemsInterfacePrefab;
        [SerializeField] private ItemsMenuInterface itemsInterfaceMenuPrefab;
        [SerializeField] private ItemNotificatorInterface itemNotificatorInterfacePrefab;

        private ItemHandler itemHandler;
        private ItemsInterface itemsInterface;
        private ItemsMenuInterface itemsMenuInterface;
        private ItemNotificatorInterface itemNotificatorInterface;

        public void ToggleMenuVisibility()
        {
            if (GameInstance.IsPaused) return;
            itemsMenuInterface.Window.ToggleGraphicVisibility();
        }

        private void Awake()
        {
            SpawnInterfacesInCanvas();
            itemHandler = GetComponent<ItemHandler>();
            itemsMenuInterface.Window.SetGraphicVisibility(false);

            SubscribeToEvents();
        }

        private void Start()
        {
            itemsInterface.AddMissingItems(itemHandler);
        }

        private void OnEnable()
        {
            GameInstance.onPause += OnGamePaused;
        }

        private void OnDisable()
        {
            GameInstance.onPause -= OnGamePaused;
        }

        private void OnGamePaused() => itemsMenuInterface.Window.SetGraphicVisibility(false);

        private void OnDestroy()
        {
            if (itemsInterface == null) return;

            UnsubscribeFromEvents();
            Destroy(itemsInterface.gameObject);
        }

        private void SpawnInterfacesInCanvas()
        {
            itemsInterface = Instantiate(itemsInterfacePrefab, GameInstance.ScreenCanvas.transform);
            itemsMenuInterface = Instantiate(itemsInterfaceMenuPrefab, GameInstance.ScreenCanvas.transform);
            itemNotificatorInterface = Instantiate(itemNotificatorInterfacePrefab, GameInstance.ScreenCanvas.transform);
        }

        private void SubscribeToEvents()
        {
            itemHandler.onItemAdded += OnItemAdded;
            itemHandler.onItemRemoved += OnItemRemoved;
            itemHandler.onStacksAdded += OnStacksAdded;
            itemHandler.onStacksRemoved += OnStacksAdded;
        }

        private void UnsubscribeFromEvents()
        {
            itemHandler.onItemAdded -= OnItemAdded;
            itemHandler.onItemRemoved -= OnItemRemoved;
            itemHandler.onStacksAdded -= OnStacksAdded;
            itemHandler.onStacksRemoved -= OnStacksRemoved;
        }

        private void OnItemAdded(ItemData itemData)
        {
            itemsInterface.AddItem(itemData, 1);
            itemsMenuInterface.AddItem(itemData, 1);
            itemNotificatorInterface.OnItemAdded(itemData);
        }

        private void OnItemRemoved(ItemData itemData)
        {
            itemsInterface.RemoveItem(itemData);
            itemsMenuInterface.RemoveItem(itemData);
        }

        private void OnStacksAdded(ItemHandler.ItemStackUpdate @event)
        {
            itemsInterface.UpdateItemStacks(@event);
            itemsMenuInterface.UpdateItemStacks(@event);
            itemNotificatorInterface.OnItemStacksAdded(@event);
        }

        private void OnStacksRemoved(ItemHandler.ItemStackUpdate @event)
        {
            itemsInterface.UpdateItemStacks(@event);
            itemsMenuInterface.UpdateItemStacks(@event);
            itemNotificatorInterface.OnItemStacksRemoved(@event);
        }
    }
}