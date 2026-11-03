using UnityEngine.UI;
using Zeke.Items;
using Zeke.UI;
using TMPro;

public class ItemsMenuInterface : ItemsInterface
{
    public override UIWindow AddItem(ItemData itemData, int stacks)
    {
        UIWindow windowInstance = base.AddItem(itemData, stacks);

        windowInstance.TryGetElement<Button>("Clickbox").onClick.AddListener(() => OnItemWindowClicked(windowInstance));

        return windowInstance;
    }

    private void Awake()
    {
        SetDefaultText();
    }

    private void OnItemWindowClicked(UIWindow windowInstance)
    {
        ItemSlotReference itemSlotReference = windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference");
        UpdateCurrentItem(itemSlotReference.ItemData, itemSlotReference.Stacks);
    }

    private void SetDefaultText()
    {
        window.TryGetElement<TextMeshProUGUI>("Selected Item Name").text = "";
        window.TryGetElement<TextMeshProUGUI>("Selected Item Description").text = "";
    }

    private void UpdateCurrentItem(ItemData itemData, int stacks)
    {
        window.TryGetElement<TextMeshProUGUI>("Selected Item Name").text = itemData.Name;
        window.TryGetElement<TextMeshProUGUI>("Selected Item Description").text = itemData.Description;

        window.TryGetElement<Image>("Selected Item Icon").sprite = itemData.Icon;
        window.TryGetElement<Image>("Selected Item Outline").sprite = itemData.Outline;
    }
}