using UnityEngine;
using TMPro;

public class ItemInfoDisplaySlot : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI descriptionText;

    public void SetData(string nameText, string descriptionText)
    {
        this.nameText.text = nameText;
        this.descriptionText.text = descriptionText;
    }
}