using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Trigger Order", menuName = "ScriptableObjects/Items/ItemTriggerOrder", order = 1)]
public class ItemTriggerOrder : ScriptableObject
{
    [field: SerializeField] public List<ItemData> Order { get; private set; }
}