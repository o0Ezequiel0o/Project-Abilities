using UnityEngine;

public class KnowledgeBookItem : Item
{
    public override ItemData Data => data;
    private readonly KnowledgeBookItemData data;

    private readonly ItemHandler itemHandler;
    private readonly GameObject source;

    private readonly Stat.Multiplier experienceMultiplier;
    private float extraTomeExperience;

    public KnowledgeBookItem(KnowledgeBookItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;

        experienceMultiplier = new Stat.Multiplier(1f);
    }

    public override void OnAdded()
    {
        if (source.TryGetComponent(out LevelHandler levelHandler))
        {
            levelHandler.ExperienceMultiplier.AddMultiplier(experienceMultiplier);
        }

        extraTomeExperience = data.ExtraMultiplierPerTome.CalculateValue(GetTomesAmount());
        experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);

        itemHandler.onItemAdded += OnItemAdded;
        itemHandler.onItemRemoved += OnItemRemoved;
        itemHandler.onItemStacksUpdated += OnItemStacksUpdated;
    }

    public override void OnRemoved()
    {
        if (source.TryGetComponent(out LevelHandler levelHandler))
        {
            levelHandler.ExperienceMultiplier.RemoveMultiplier(experienceMultiplier);
        }
    }

    public override void OnStackAdded()
    {
        experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);
    }

    public override void OnStackRemoved()
    {
        experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);
    }

    private int GetTomesAmount()
    {
        int tomes = 0;

        if (itemHandler.TryGetItem(data.TomeItem, out Item item))
        {
            tomes = item.stacks;
        }

        return tomes;
    }

    private void OnItemAdded(ItemData item)
    {
        if (item == data.TomeItem)
        {
            extraTomeExperience = data.ExtraMultiplierPerTome.CalculateValue(GetTomesAmount());
            experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);
        }
    }

    private void OnItemRemoved(ItemData item)
    {
        if (item == data.TomeItem)
        {
            extraTomeExperience = data.ExtraMultiplierPerTome.CalculateValue(GetTomesAmount());
            experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);
        }
    }

    private void OnItemStacksUpdated(ItemData item, int _)
    {
        if (item == data.TomeItem)
        {
            extraTomeExperience = data.ExtraMultiplierPerTome.CalculateValue(GetTomesAmount());
            experienceMultiplier.UpdateMultiplier(data.ExperienceMultiplier.CalculateValue(stacks) + extraTomeExperience);
        }
    }
}