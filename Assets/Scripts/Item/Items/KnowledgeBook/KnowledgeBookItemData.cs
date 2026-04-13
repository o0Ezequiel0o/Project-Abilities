using UnityEngine;

[CreateAssetMenu(fileName = "Book of Knowledge", menuName = "ScriptableObjects/Items/Items/KnowledgeBook", order = 1)]
public class KnowledgeBookItemData : ItemData
{
    [field: SerializeField] public StackStat ExperienceMultiplier { get; private set; }
    [field: SerializeField] public StackStat ExtraMultiplierPerTome { get; private set; }

    [field: Space]

    [field: SerializeField] public ItemData TomeItem { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new KnowledgeBookItem(this, itemHandler, source);
    }
}