using UnityEngine;

namespace Zeke.Items
{
    [RequireComponent(typeof(ItemHandler))]
    public class ItemsScreenRenderer : MonoBehaviour
    {
        [SerializeField] private ItemsInterface itemsInterfacePrefab;
        [SerializeField] private ItemsInterface itemsInterfaceMenuPrefab;

        private ItemHandler itemHandler;
        private ItemsInterface itemsInterface;
        private ItemsInterface itemsMenuInterface;

        public void ToggleMenuVisibility()
        {
            SetMenuVisibility(!itemsMenuInterface.gameObject.activeSelf);
        }

        public void SetMenuVisibility(bool visibility)
        {
            itemsMenuInterface.gameObject.SetActive(visibility);

            if (visibility == true)
            {
                itemsMenuInterface.SubscribeToEvents(itemHandler);
                itemsMenuInterface.RefreshAllItems(itemHandler);
            }
            else
            {
                itemsMenuInterface.UnsubscribeFromEvents(itemHandler);
            }
        }

        private void Awake()
        {
            SpawnInterfacesInCanvas();
            itemHandler = GetComponent<ItemHandler>();
            itemsInterface.SubscribeToEvents(itemHandler);

            itemsMenuInterface.gameObject.SetActive(false);
        }

        private void Start()
        {
            itemsInterface.RefreshAllItems(itemHandler);
        }

        private void SpawnInterfacesInCanvas()
        {
            itemsInterface = Instantiate(itemsInterfacePrefab, GameInstance.ScreenCanvas.transform);
            itemsMenuInterface = Instantiate(itemsInterfaceMenuPrefab, GameInstance.ScreenCanvas.transform);
        }
    }
}