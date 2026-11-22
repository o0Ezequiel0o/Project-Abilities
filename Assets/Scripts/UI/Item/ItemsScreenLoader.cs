using UnityEngine;

namespace Zeke.Items
{
    public class ItemsScreenLoader : MonoBehaviour
    {
        [SerializeField] private ItemsHUDInterface window;
        [SerializeField] private ItemsMenuInterface menuWindow;

        private ItemHandler itemHandler;

        private void Awake()
        {
            Run.Instance.onPlayerAdded += OnPlayerAdded;
        }

        private void OnPlayerAdded(Player newPlayer)
        {
            Run.Instance.onPlayerAdded -= OnPlayerAdded;

            bool playerHasRequiredComponents = newPlayer.TryGetComponent(out itemHandler);

            if (!playerHasRequiredComponents) return;

            window.AddMissingItems(itemHandler);
            SubscribeToEvents();
        }

        public void ToggleMenuVisibility()
        {
            menuWindow.Window.ToggleGraphicVisibility();
        }

        private void SubscribeToEvents()
        {
            itemHandler.onItemAdded += OnItemAdded;
            itemHandler.onItemRemoved += OnItemRemoved;
            itemHandler.onStacksAdded += UpdateItemStacks;
            itemHandler.onStacksRemoved += UpdateItemStacks;
        }

        private void OnItemAdded(ItemData itemData)
        {
            window.AddItem(itemData, 1);
            menuWindow.AddItem(itemData, 1);
        }

        private void OnItemRemoved(ItemData itemData)
        {
            window.RemoveItem(itemData);
            menuWindow.RemoveItem(itemData);
        }

        private void UpdateItemStacks(ItemHandler.ItemStackUpdate @event)
        {
            window.UpdateItemStacks(@event);
            menuWindow.UpdateItemStacks(@event);
        }
    }
}