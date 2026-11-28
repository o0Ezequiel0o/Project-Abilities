using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class AbilityModuleData
    {
        public abstract AbilityModule CreateModule();
    }
}