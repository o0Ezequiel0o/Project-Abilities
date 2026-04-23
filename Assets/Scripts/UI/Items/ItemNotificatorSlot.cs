using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Zeke.Items
{
    public class ItemNotificatorSlot : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI stacksText;

        public ItemData ItemData { get; private set; }
        public int Stacks { get; private set; }

        private int accumulator = 0;
        private int startValue = 0;

        public Sprite Icon
        {
            get
            {
                return image.sprite;
            }
            set
            {
                image.sprite = value;
            }
        }

        public Sprite Outline
        {
            get
            {
                return outline.sprite;
            }
            set
            {
                outline.sprite = value;
            }
        }

        public void SetData(ItemData itemData, int stacks)
        {
            Icon = itemData.Icon;
            Outline = itemData.Outline;

            ItemData = itemData;

            startValue = stacks;
        }

        public void UpdateStacksAmount(int stacks)
        {
            stacksText.text = "x" + stacks.ToString();
            Stacks = stacks;
        }
    }
}