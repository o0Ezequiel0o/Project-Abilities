using UnityEngine;

namespace Zeke.Items
{
    public class TomeItem : Item
    {
        public override ItemData Data => data;
        private readonly TomeItemData data;

        private readonly GameObject source;

        private readonly Stat.Multiplier experienceMultiplier;

        public TomeItem(TomeItemData data, ItemHandler _, GameObject source)
        {
            this.data = data;
            this.source = source;

            experienceMultiplier = new Stat.Multiplier(1f);
        }

        public override void OnAdded()
        {
            if (source.TryGetComponent(out LevelHandler levelHandler))
            {
                levelHandler.ExperienceMultiplier.AddMultiplier(experienceMultiplier);
            }

            experienceMultiplier.UpdateMultiplier(data.XPMult.GetValue(stacks));
        }

        public override void OnRemoved()
        {
            experienceMultiplier.UpdateMultiplier(data.XPMult.GetValue(stacks));

            if (source.TryGetComponent(out LevelHandler levelHandler))
            {
                levelHandler.ExperienceMultiplier.RemoveMultiplier(experienceMultiplier);
            }
        }

        public override void OnStackAdded()
        {
            experienceMultiplier.UpdateMultiplier(data.XPMult.GetValue(stacks));
        }

        public override void OnStackRemoved()
        {
            experienceMultiplier.UpdateMultiplier(data.XPMult.GetValue(stacks));
        }
    }
}