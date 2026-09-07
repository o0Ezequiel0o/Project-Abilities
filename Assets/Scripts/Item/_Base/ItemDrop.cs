using UnityEngine;

namespace Zeke.Items
{
    public class ItemDrop : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemSettings itemSettings;

        [Header("Dependency")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private SpriteRenderer outlineRenderer;

        public Sprite InteractOverlay { get; private set; }

        private ItemData itemData;

        public void LoadItemData(ItemData itemData)
        {
            this.itemData = itemData;

            iconRenderer.sprite = itemData.Icon;
            outlineRenderer.sprite = itemData.Outline;

            InteractOverlay = itemData.Outline;

            outlineRenderer.color = itemSettings.GetRarityColor(itemData.Rarity);
        }

        public bool CanSelect(GameObject source)
        {
            return true;
        }

        public bool CanInteract(GameObject source)
        {
            return source.TryGetComponent(out ItemHandler _);
        }

        public bool Interact(GameObject source)
        {
            if (source.TryGetComponent(out ItemHandler itemHandler))
            {
                itemHandler.AddItem(itemData);
                Destroy(gameObject);

                return true;
            }

            return false;
        }
    }
}