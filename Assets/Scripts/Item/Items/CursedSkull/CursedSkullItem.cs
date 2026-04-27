using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

namespace Zeke.Items
{
    public class CursedSkullItem : Item
    {
        public override ItemData Data => data;
        private readonly CursedSkullItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public CursedSkullItem(CursedSkullItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onKill.Subscribe(source, OnKill, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onKill.Unsubscribe(source, OnKill);
        }

        private void OnKill(DamageEvent damageEvent)
        {
            if (!RollProc(data.Chance, 1f, itemHandler.Luck.ValueInt)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            GameObject summon = GameObject.Instantiate(data.Prefab, damageEvent.Receiver.transform.position, Quaternion.identity);

            if (summon.TryGetComponent(out TeamIdentifier teamIdentifier))
            {
                teamIdentifier.ChangeTeam(TeamManager.GetTeam(source));
            }

            int extraLevels = Mathf.FloorToInt(data.ExtraLevels.GetValue(stacks));

            if (summon.TryGetComponent(out LevelHandler levelHandler))
            {
                for (int i = 0; i < extraLevels; i++)
                {
                    levelHandler.GiveExperience(levelHandler.ExperienceRequired);
                }
            }
        }
    }
}