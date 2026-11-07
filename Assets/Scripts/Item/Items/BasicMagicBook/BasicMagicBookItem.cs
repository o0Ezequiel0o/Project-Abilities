using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;
using System;

namespace Zeke.Items
{
    public class BasicMagicBookItem : Item
    {
        public override ItemData Data => data;
        private readonly BasicMagicBookItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly List<AbilityType> validUpgradeTypes = new List<AbilityType>();

        public BasicMagicBookItem(BasicMagicBookItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out LevelHandler levelHandler))
            {
                levelHandler.onLevelUp.Subscribe(OnLevelUp, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out LevelHandler levelHandler))
            {
                levelHandler.onLevelUp.Unsubscribe(OnLevelUp);
            }
        }

        private void OnLevelUp(LevelHandler.LevelUpInfo levelUpInfo)
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                validUpgradeTypes.Clear();

                foreach (AbilityType abilityType in Enum.GetValues(typeof(AbilityType)))
                {
                    if (abilityController.AbilityTypeSlots[abilityType].Abilities.Count > 0)
                    {
                        validUpgradeTypes.Add(abilityType);
                    }
                }

                if (validUpgradeTypes.Count <= 0) return;

                AbilityType randomAbilityType = validUpgradeTypes[UnityEngine.Random.Range(0, validUpgradeTypes.Count)];
                abilityController.AbilityTypeSlots[randomAbilityType].UpgradeAbilityType(Mathf.FloorToInt(data.Levels.GetValue(stacks)));
            }
        }
    }
}