using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Ascendancy
{
    [RequireComponent(typeof(AbilityController))]
    public class AscendancyHandler : MonoBehaviour
    {
        [Header("Dependency")]
        [SerializeField] private AscendanciesData ascendanciesData;
        [SerializeField] private LevelHandler levelHandler;

        public Action<AscendanceElementInfo> onAscendancyReady;

        private AbilityController controller;

        private readonly HashSet<AbilityData> ascendenciesReadied = new HashSet<AbilityData>();

        public void ApplyAscension(AbilityData key, AscendancyData selected)
        {
            controller.ReplaceAbility(key, selected.AbilityData).Destroy();
        }

        public void CheckForReadyAscendancies()
        {
            List<IAbility> abilities = controller.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (ascendenciesReadied.Contains(abilities[i].Data)) continue;

                if (ascendanciesData.Elements.TryGetValue(abilities[i].Data, out AscendanceElement element))
                {
                    if (element.RequiredLevel > levelHandler.Level) continue;
                    onAscendancyReady?.Invoke(new AscendanceElementInfo(abilities[i].Data, element));

                    ascendenciesReadied.Add(abilities[i].Data);
                }
            }
        }

        private void Reset()
        {
            levelHandler = GetComponentInChildren<LevelHandler>();
        }

        private void Awake()
        {
            controller = GetComponent<AbilityController>();
            levelHandler.onLevelUp.Subscribe(OnLevelUp);
        }

        private void OnDestroy()
        {
            levelHandler.onLevelUp.Unsubscribe(OnLevelUp);
        }

        private void OnLevelUp(LevelHandler.LevelUpInfo levelUpInfo)
        {
            CheckForReadyAscendancies();
        }

        public readonly struct AscendanceElementInfo
        {
            public readonly AbilityData key;
            public readonly AscendanceElement element;

            public AscendanceElementInfo(AbilityData key, AscendanceElement element)
            {
                this.key = key;
                this.element = element;
            }
        }
    }
}