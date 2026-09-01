using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class EditMultiplierStatValue : AbilityModule
    {
        private readonly EditMultiplierStatValueData data;

        private readonly Stat amount;
        private readonly GetStatStrategy stat;

        private Stat statReference;

        private Stat.Multiplier multiplier;

        public EditMultiplierStatValue(EditMultiplierStatValueData data, GetStatStrategy stat, Stat amount)
        {
            this.data = data;
            this.stat = stat;
            this.amount = amount;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            statReference = stat.GetStat(source);
            multiplier = new Stat.Multiplier(amount.Value);

            if (statReference != null)
            {
                statReference.onStatUpdated += UpdateMultiplier;
            }
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (statReference == null) return;
            statReference.AddMultiplier(multiplier);
        }

        public override void Deactivate()
        {
            if (statReference == null || data.Permanent) return;
            statReference.RemoveMultiplier(multiplier);
        }

        public override void Upgrade()
        {
            amount.Upgrade();
        }

        private void UpdateMultiplier(Stat.StatUpdate _)
        {
            multiplier.UpdateMultiplier(amount.Value);
        }
    }
}