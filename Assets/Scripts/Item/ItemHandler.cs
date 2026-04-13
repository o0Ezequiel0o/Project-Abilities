using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemHandler : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private ItemSettings itemSettings;

    [field: Header("Settings")]
    [field: SerializeField] public int Luck { get; private set; }

    public List<Item> Items => items;
    public List<ItemData> ItemsData => itemsData;

    public Action<ItemData> onItemAdded;
    public Action<ItemData> onItemRemoved;

    public Action<ItemData, int> onItemStacksUpdated;

    private List<Item> items = new List<Item>();

    private readonly List<ItemData> itemsData = new List<ItemData>();

    public void AddItems(List<ItemData> itemsData)
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            AddItem(itemsData[i]);
        }
    }

    public void AddItems(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            for (int x = 0; x < items[i].stacks; x++)
            {
                AddItem(items[i].Data);
            }
        }
    }

    public void AddItem(ItemData itemData)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Data == itemData)
            {
                UpdateItemStacks(items[i]);
                onItemStacksUpdated?.Invoke(itemData, items[i].stacks);
                return;
            }
        }

        AddNewItem(itemData);
        onItemAdded?.Invoke(itemData);
    }

    public void AddItemStacks(ItemData itemData, int stacks)
    {
        if (!TryGetItem(itemData, out Item item))
        {
            AddNewItem(itemData);
            onItemAdded?.Invoke(itemData);
            item = items[^1];
            stacks -= 1;
        }

        for (int i = 0; i < stacks; i++)
        {
            UpdateItemStacks(item);
            onItemStacksUpdated?.Invoke(itemData, item.stacks);
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Data == itemData)
            {
                if (items[i].stacks == 1)
                {
                    items[i].OnRemoved();
                    items.RemoveAt(i);
                    onItemRemoved?.Invoke(itemData);

                    itemsData.Remove(itemData);
                }
                else
                {
                    items[i].stacks -= 1;
                    items[i].OnStackRemoved();
                    onItemStacksUpdated?.Invoke(itemData, items[i].stacks);
                }

                return;
            }
        }
    }

    public bool TryGetItem(ItemData itemData, out Item item)
    {
        item = null;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Data == itemData)
            {
                item = items[i];
                return true;
            }
        }

        return false;
    }

    public void ClearItems()
    {
        items.Clear();
        itemsData.Clear();
    }

    private void Awake()
    {
        SubscribeToEvents();
        SubscribeToGlobalEvents();
    }

    private void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnUpdate();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromGlobalEvents();
    }

    private void AddNewItem(ItemData itemData)
    {
        items.Add(itemData.CreateItem(this, gameObject));
        items[^1].stacks = 1;
        items[^1].OnAdded();
        UpdateItemsOrder();

        itemsData.Add(itemData);
    }

    private void UpdateItemStacks(Item item)
    {
        item.OnStackAdded();
        item.stacks += 1;
    }

    private void UpdateItemsOrder()
    {
        List<Item> orderedInventory = new List<Item>();

        for (int i = 0; i < itemSettings.TriggerOrder.Count; i++)
        {
            if (orderedInventory.Count == items.Count)
            {
                break;
            }

            for (int x = 0; x < items.Count; x++)
            {
                if (itemSettings.TriggerOrder[i] == items[x].Data)
                {
                    orderedInventory.Add(items[x]);
                    break;
                }
            }
        }

        items = orderedInventory;
    }

    private void SubscribeToEvents()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onHealthReceived += OnHealthReceived;
            damageable.onDamageTaken += OnDamageTaken;
            damageable.onTakeDamage += OnTakeDamage;
            damageable.onHitTaken += OnHitTaken;
            damageable.onDeath += OnDeath;
        }
    }

    private void SubscribeToGlobalEvents()
    {
        Damageable.HealEvent.onHealthHealed.Subscribe(gameObject, OnHealthHealed);
        Damageable.DamageEvent.onDamageDealt.Subscribe(gameObject, OnDamageDealt);
        Damageable.DamageEvent.onDealDamage.Subscribe(gameObject, OnDealDamage);
        Damageable.DamageEvent.onKill.Subscribe(gameObject, OnKill);
        Damageable.DamageEvent.onHit.Subscribe(gameObject, OnHit);
    }

    private void UnsubscribeFromGlobalEvents()
    {
        Damageable.HealEvent.onHealthHealed.Unsubscribe(gameObject, OnHealthHealed);
        Damageable.DamageEvent.onDamageDealt.Unsubscribe(gameObject, OnDamageDealt);
        Damageable.DamageEvent.onDealDamage.Unsubscribe(gameObject, OnDealDamage);
        Damageable.DamageEvent.onKill.Unsubscribe(gameObject, OnKill);
        Damageable.DamageEvent.onHit.Unsubscribe(gameObject, OnHit);
    }

    private void OnHealthHealed(Damageable.HealEvent healingEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnHealthHealed(healingEvent);
        }
    }

    private void OnHealthReceived(Damageable.HealEvent healingEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnHealthReceived(healingEvent);
        }
    }

    private void OnHit(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnHit(damageEvent);
        }
    }

    private void OnHitTaken(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnHitTaken(damageEvent);
        }
    }

    private void OnDealDamage(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnDealDamage(damageEvent);
        }
    }

    private void OnTakeDamage(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnTakeDamage(damageEvent);
        }
    }

    private void OnDamageDealt(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnDamageDealt(damageEvent);
        }
    }

    private void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnDamageTaken(damageEvent);
        }
    }

    private void OnKill(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnKill(damageEvent);
        }
    }

    private void OnDeath(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnDeath(damageEvent);
        }
    }
}