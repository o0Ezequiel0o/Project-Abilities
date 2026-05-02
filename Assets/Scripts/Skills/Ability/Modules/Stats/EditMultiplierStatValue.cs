using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class EditMultiplierStatValue : AbilityModule
    {
        [SerializeField] private bool permanent = false;
        [SerializeField] private Stat amount;
        [SerializeReferenceDropdown, SerializeReference] private GetStatStrategy stat;

        private Stat statReference;

        private Stat.Multiplier multiplier;

        public EditMultiplierStatValue() { }

        public EditMultiplierStatValue(EditMultiplierStatValue original)
        {
            permanent = original.permanent;

            amount = original.amount.DeepCopy();
            stat = original.stat.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new EditMultiplierStatValue(this);

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
            if (statReference == null || permanent) return;
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