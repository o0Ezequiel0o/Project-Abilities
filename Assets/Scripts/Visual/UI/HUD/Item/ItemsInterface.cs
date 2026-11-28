using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;
using Zeke.UI;

public abstract class ItemsInterface : MonoBehaviour
{
    [SerializeField] protected UIWindow window;
    [SerializeField] protected UIWindow itemSlotPrefab;

    public UIWindow Window => window;

    protected readonly Dictionary<ItemData, UIWindow> itemSlots = new Dictionary<ItemData, UIWindow>();

    public abstract UIWindow AddItem(ItemData itemData, int stacks);

    public abstract UIWindow RemoveItem(ItemData itemData);

    public abstract UIWindow UpdateItemStacks(ItemHandler.ItemStackUpdate stackUpdate);

    protected string GetStacksString(int stacks) => "x" + stacks.ToString();
}