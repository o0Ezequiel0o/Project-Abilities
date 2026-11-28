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

        public override void Initialize()
        {
            levelHandler = source.GetComponent<LevelHandler>();
        }

        public override void OnRemoved()
        {
            levelHandler.ExperienceMultiplier.ApplyFlatModifier(-flatModifier);
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateFlatModifier();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateFlatModifier();
        }

        private void UpdateFlatModifier()
        {
            if (levelHandler == null) return;

            float oldFlatModifier = flatModifier;
            flatModifier = data.XPFlatMult.GetValue(stacks);

            levelHandler.ExperienceMultiplier.ApplyFlatModifier(-oldFlatModifier, flatModifier);
        }
    }
}