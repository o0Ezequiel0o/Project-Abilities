using UnityEngine;
using Zeke.Abilities;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class FacilityMushroomItem : Item
    {
        public override ItemData Data => data;
        private readonly FacilityMushroomItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly GameObjectPool<DamageAreaEffect> pool = new GameObjectPool<DamageAreaEffect>();

        public FacilityMushroomItem(FacilityMushroomItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.onAbilityUsed.Subscribe(OnAbilityUsed, data.TriggerOrder);
            }
        }

        public override void OnRemoved() { }

        public override void OnStacksAdded(int amount) { }

        public override void OnStacksRemoved(int amount) { }

        public override void OnUpdate() { }

        private void OnAbilityUsed(IAbility ability)
        {
            if (!RollProc(data.ProcChance.GetValue(stacks), 1f, itemHandler.Luck.ValueInt)) return;

            DamageAreaEffect areaEffect = pool.Get(data.Prefab);

            float radius = ability.CooldownTime;
            float damage = ability.CooldownTime * stacks;

            DamageData damageData = new DamageData(damage, data.ArmorPenetration, data.ProcCoefficient);
            areaEffect.CreateAreaEffect(data.Ticks, data.TickInterval, radius, damageData, source, TeamManager.GetTeam(source));

            areaEffect.gameObject.SetActive(true);
        }
    }
}