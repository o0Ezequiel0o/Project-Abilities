using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SpawnEngineerTurret : GenericSummon<EngineerTurret>
    {
        public SpawnEngineerTurret(SpawnEngineerTurret original) : base(original) { }

        public override AbilityModule DeepCopy() => new SpawnEngineerTurret(this);

        public override void Activate(bool holding)
        {
            if (summons.Count >= maxSummons.ValueInt)
            {
                DestroySummon(summons[0]);
            }

            if (TrySpawnSummon(prefab, out EngineerTurret engineerTurret))
            {
                engineerTurret.SetData(source);
            }
        }

        public override void Destroy()
        {
            DestroyAllSummoned();
        }
    }
}