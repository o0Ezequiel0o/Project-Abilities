using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class EditFlatStatValue : AbilityModule
    {
        [SerializeField] private bool permanent = false;
        [SerializeField] private Stat amount;
        [SerializeReferenceDropdown, SerializeReference] private GetStatStrategy stat;

        private Stat statReference;

        private float changedAmount = 0f;

        public EditFlatStatValue() { }

        public EditFlatStatValue(EditFlatStatValue original)
        {
            permanent = original.permanent;

            amount = original.amount.DeepCopy();
            stat = original.stat.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new EditFlatStatValue(this);

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
            if (statReference == null || permanent) return;
            statReference.ApplyFlatModifier(-changedAmount);
        }
    }
}