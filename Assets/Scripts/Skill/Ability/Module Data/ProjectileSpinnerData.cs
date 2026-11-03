using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ProjectileSpinnerData : GenericSpinnerData<SpinnerProjectile>
    {
        [SerializeField] private Stat damage;
        [SerializeField] private Stat pierce;

        [field: Space]

        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;

        public override AbilityModule CreateModule()
        {
            return new ProjectileSpinner(this, distance.DeepCopy(), amount.DeepCopy(), speed.DeepCopy(), damage.DeepCopy(), pierce.DeepCopy());
        }
    }
}