using UnityEngine;
using Zeke.Abilities;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class BombBagItem : Item
    {
        public override ItemData Data => data;
        private readonly BombBagItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float timer = 0f;

        private readonly GameObjectPool<BombItemBomb> bombs = new GameObjectPool<BombItemBomb>();

        public BombBagItem(BombBagItemData data, ItemHandler itemHandler, GameObject source)
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

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.onAbilityUsed.Unsubscribe(OnAbilityUsed);
            }
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
        }

        private void OnAbilityUsed(IAbility ability)
        {
            if (timer < data.Cooldown) return;

            if (ability.Data.AbilityType == data.AbilityType)
            {
                SpawnBomb();
                timer = 0f;
            }
        }

        private void SpawnBomb()
        {
            float damage = data.Damage.GetValue(stacks);
            BombItemBomb bomb = bombs.Get(data.BombPrefab);

            bomb.transform.SetPositionAndRotation(source.transform.position, Quaternion.identity);
            bomb.StartFuse(data.Fuse, damage, data.Radius, source, TeamManager.GetTeam(source));

            bomb.gameObject.SetActive(true);
        }
    }
}