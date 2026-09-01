using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GenericSpinnerData<T> : AbilityModuleData where T : Component
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }

        [Space]

        [SerializeField] protected Stat distance;
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat speed;

        public override AbilityModule CreateModule()
        {
            return new GenericSpinner<T>(this, distance.DeepCopy(), amount.DeepCopy(), speed.DeepCopy());
        }
    }
}