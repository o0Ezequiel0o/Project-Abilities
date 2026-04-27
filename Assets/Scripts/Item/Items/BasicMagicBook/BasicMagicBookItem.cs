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

        private readonly List<IAbility> abilities = new List<IAbility>();

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
                levelHandler.onLevelUp.Subscribe(OnLevelUp, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out LevelHandler levelHandler))
            {
                levelHandler.onLevelUp.Unsubscribe(OnLevelUp);
            }
        }

        private void OnLevelUp(int level)
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilities.Clear();

                foreach (AbilityType abilityType in (AbilityType[])Enum.GetValues(typeof(AbilityType)))
                {
                    if (abilityController.TryGetAbility(abilityType, out IAbility ability))
                    {
                        if (ability == null) continue;
                        abilities.Add(ability);
                    }
                }

                if (abilities.Count <= 0) return;

                IAbility randomAbility = abilities[UnityEngine.Random.Range(0, abilities.Count)];

                for (int i = 0; i < data.Levels.GetValue(stacks); i++)
                {
                    randomAbility.QueueUpgrade();
                }
            }
        }
    }
}