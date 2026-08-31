using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class SummonData : AbilityModuleData
    {
        [field: Header("Summon")]
        [field: SerializeField] public GameObject Summon { get; private set; }
        [SerializeReferenceDropdown, SerializeReference] private List<SummonModuleData> modules = new List<SummonModuleData>(); //{ new JoinSourceTeam() };

        [Header("Spawning")]
        [SerializeField] private Stat maxSummons;

        [field: Space]

        [field: SerializeField] public bool FixedRotation { get; private set; }
        [field: SerializeField] public float SpawnBlockRadius { get; private set; }
        [field: SerializeField] public float SpawnDistance { get; private set; }
        [field: SerializeField] public LayerMask SpawnBlockLayers { get; private set; }

        private readonly List<SummonModule> instanceModules = new List<SummonModule>();

        public override AbilityModule CreateModule()
        {
            instanceModules.Clear();

            for (int i = 0; i < modules.Count; i++)
            {
                instanceModules.Add(modules[i].CreateSummonModule());
            }

            return new Summon(this, instanceModules, maxSummons.DeepCopy());
        }
    }
}