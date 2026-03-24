using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBasicProjectile : GenericFireProjectile<BasicProjectile>
    {
        public FireBasicProjectile(GenericFireProjectile<BasicProjectile> original) : base(original) { }

        public override AbilityModule DeepCopy() => new FireBasicProjectile(this);
    }
}