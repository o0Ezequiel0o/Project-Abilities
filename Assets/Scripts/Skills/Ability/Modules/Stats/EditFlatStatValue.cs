using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class EditFlatStatValue : AbilityModule
    {
        private readonly EditFlatStatValueData data;

        private readonly Stat amount;
        private readonly GetStatStrategy stat;

        private Stat statReference;

        private float changedAmount = 0f;

        public EditFlatStatValue(EditFlatStatValueData data, GetStatStrategy stat, Stat amount)
        {
            this.data = data;
            this.stat = stat;
            this.amount = amount;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            statReference = stat.GetStat(source);
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (statReference == null) return;

            changedAmount = amount.Value;
            statReference.ApplyFlatModifier(changedAmount);
        }

        public override void Deactivate()
        {
            if (statReference == null || data.Permanent) return;
            statReference.ApplyFlatModifier(-changedAmount);
        }

        public override void Upgrade()
        {
            amount.Upgrade();
        }
    }
}