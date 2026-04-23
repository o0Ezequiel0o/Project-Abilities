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

        public int Accumulator => accumulator;

        private int accumulator = 0;

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
        }

        public void UpdateStacksAmount(int stacksUpdate)
        {
            accumulator += stacksUpdate;
            stacksText.text = accumulator.ToString("+0;-0;0");
        }

        public void ResetAccumulator()
        {
            accumulator = 0;
        }
    }
}