using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class AbilityModule
    {
        public abstract void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability);

        public abstract AbilityModule DeepCopy();

        public abstract void Activate(bool holding);
        public virtual void Deactivate() { }

        public abstract bool CanActivate();
        public abstract bool CanUpgrade();

        public virtual void UpdateActive() { }
        public virtual void UpdateUnactive() { }

        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void Upgrade() { }
        public virtual void Destroy() { }
    }
}