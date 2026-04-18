using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Zeke.UI;
using TMPro;

public abstract class ItemGenerator : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSettings itemSettings;

    [field: Header("Visual - Object")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }

    [field: Header("Visual - UI")]
    [SerializeField] private UIWindow itemSelectionWindow;
    [SerializeField] private UIWindow itemOptionWindow;

    [Header("Settings")]
    [SerializeField] protected int cost;

    [Header("Selection")]
    [SerializeField] protected int options = 3;
    [SerializeField] private List<ItemDropSlot> itemDropChance;

    public static ActionDictionary<GameObject, List<ItemGenerationData>> onOptionsGenerated;
    public static ActionDictionary<GameObject, ItemGenerationData> onItemSelected;

    private UIWindow windowInstance;

    public abstract bool CanSelect(GameObject source);

    public abstract bool CanInteract(GameObject source);

    public abstract bool Interact(GameObject source);

    public void GenerateOptions(GameObject source, int amount)
    {
        List<ItemGenerationData> generatedItems = RollOptions(amount);
        onOptionsGenerated?.Invoke(source, generatedItems);
        CreateOptionsUIWindow(generatedItems, source);
    }

    private void OnOptionSelected(GameObject source, ItemGenerationData generatedItem)
    {
        if (source.TryGetComponent(out ItemHandler itemHandler))
        {
            itemHandler.AddItem(generatedItem.item);
        }

        windowInstance.DestroyWindow();

        onItemSelected?.Invoke(source, generatedItem);
    }

    private List<ItemGenerationData> RollOptions(int amount)
    {
        List<ItemGenerationData> generatedItems = new List<ItemGenerationData>(amount);

        for (int i = 0; i < amount; i++)
        {
            ItemRarity rarity = WeightedSelect.SelectElement(itemDropChance).rarity;
            ItemData item = itemSettings.GetRandomItem(rarity);
            generatedItems.Add(new ItemGenerationData(item, this));
        }

        return generatedItems;
    }

    private void CreateOptionsUIWindow(List<ItemGenerationData> generatedItems, GameObject source)
    {
        windowInstance = Instantiate(itemSelectionWindow, GameInstance.ScreenCanvas.transform);
        LayoutGroup layoutGroupRoot = windowInstance.TryGetElement<LayoutGroup>("Layout Group");

        for (int i = 0; i < generatedItems.Count; i++)
        {
            CreateItemOptionUIWindow(layoutGroupRoot.transform, generatedItems[i], source);
        }
    }

    private void CreateItemOptionUIWindow(Transform root, ItemGenerationData generatedItem, GameObject source)
    {
        UIWindow optionWindowInstance = Instantiate(itemOptionWindow, root);

        optionWindowInstance.TryGetElement<Image>("Image").sprite = generatedItem.item.Icon;
        optionWindowInstance.TryGetElement<TextMeshProUGUI>("Name").text = generatedItem.item.Name;
        optionWindowInstance.TryGetElement<Image>("Border").color = itemSettings.GetRarityColor(generatedItem.item.Rarity);

        Button button = optionWindowInstance.TryGetElement<Button>("Button");
        button.onClick.AddListener(() => OnOptionSelected(source, generatedItem));
    }

    protected void DisableColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    protected void DestroyObject()
    {
        Destroy(gameObject);
    }

    [Serializable]
    private class ItemDropSlot : IWeighted
    {
        public ItemRarity rarity;
        public int weight;

        public int Weight => weight;

        public ItemDropSlot() { }

        public ItemDropSlot(ItemRarity rarity, int weight)
        {
            this.rarity = rarity;
            this.weight = weight;
        }
    }

    public class ItemGenerationData
    {
        public ItemData item;
        public ItemGenerator itemGenerator;

        public ItemGenerationData(ItemData item,  ItemGenerator itemGenerator)
        {
            this.item = item;
            this.itemGenerator = itemGenerator;
        }
    }
}