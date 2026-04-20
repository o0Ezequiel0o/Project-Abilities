using UnityEngine;

namespace Zeke.Items
{
    public class TomeItem : Item
    {
        public override ItemData Data => data;
        private readonly TomeItemData data;

        private readonly GameObject source;
        private LevelHandler levelHandler;

        private float flatModifier = 0f;

        public TomeItem(TomeItemData data, ItemHandler _, GameObject source)
        {
            this.data = data;
            this.source = source;
        }

        public override void OnAdded()
        {
            levelHandler = source.GetComponent<LevelHandler>();
            UpdateFlatModifier();
        }

        public override void OnRemoved()
        {
            RemoveFlatModifier();
        }

        public override void OnStackAdded()
        {
            UpdateFlatModifier();
        }

        public override void OnStackRemoved()
        {
            UpdateFlatModifier();
        }

        private void UpdateFlatModifier()
        {
            if (levelHandler == null) return;

            float oldFlatModifier = flatModifier;
            flatModifier = data.XPFlatMult.GetValue(stacks);

            levelHandler.ExperienceMultiplier.ApplyFlatModifier(-oldFlatModifier);
            levelHandler.ExperienceMultiplier.ApplyFlatModifier(flatModifier);
        }

        private void RemoveFlatModifier()
        {
            levelHandler.ExperienceMultiplier.ApplyFlatModifier(-flatModifier);
        }
    }
}