using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Zeke.Items
{
    public class ItemDisplaySlot : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI stacksText;

        public ItemData ItemData { get; private set; }
        public int Stacks { get; private set; }

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

        public void SetData(ItemData itemData)
        {
            Icon = itemData.Icon;
            Outline = itemData.Outline;

            ItemData = itemData;
        }

        public void UpdateStacksAmount(int stacks)
        {
            stacksText.text = "x " + stacks.ToString();
            Stacks = stacks;
        }
    }
}