using UnityEngine;
using UnityEngine.UI;
using Zeke.Items;
using Zeke.UI;
using TMPro;

public class ItemsMenuInterface : ItemsInterface
{
    public override UIWindow AddItem(ItemData itemData, int stacks)
    {
        Transform root = window.TryGetElement<LayoutGroup>("Layout Group").transform;
        UIWindow windowInstance = Instantiate(itemSlotPrefab, root);

        windowInstance.TryGetElement<Button>("Clickbox").onClick.AddListener(() => OnItemWindowClicked(windowInstance));

        windowInstance.TryGetElement<Image>("Icon").sprite = itemData.Icon;
        windowInstance.TryGetElement<Image>("Outline").sprite = itemData.Outline;
        windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").text = GetStacksString(stacks);

        ItemSlotReference itemSlotReference = windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference");

        itemSlotReference.ItemData = itemData;
        itemSlotReference.Stacks = stacks;

        itemSlots.Add(itemData, windowInstance);

        return windowInstance;
    }

    public override UIWindow RemoveItem(ItemData itemData)
    {
        if (itemSlots.TryGetValue(itemData, out UIWindow windowInstance))
        {
            itemSlots.Remove(itemData);
            return windowInstance;
        }

        return null;
    }

    public override UIWindow UpdateItemStacks(ItemHandler.ItemStackUpdate stackUpdate)
    {
        UIWindow windowInstance = itemSlots[stackUpdate.itemData];

        windowInstance.TryGetElement<ItemSlotReference>("Item Slot Reference").Stacks = stackUpdate.stacks;
        windowInstance.TryGetElement<TextMeshProUGUI>("Stacks").SetText(GetStacksString(stackUpdate.stacks));

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