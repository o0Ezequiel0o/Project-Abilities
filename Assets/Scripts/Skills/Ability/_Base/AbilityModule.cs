using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class AbilityModule
    {
        //add a default implementation for Initialize and move OnInitialization after this
        public abstract void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability);

        public abstract void Activate(bool holding);
        public virtual void Deactivate() { }

        public abstract bool CanActivate();
        public abstract bool CanUpgrade();

        public virtual void UpdateActive() { }
        public virtual void UpdateInactive() { }

        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void Upgrade() { }
        public virtual void Destroy() { }

        public virtual AbilityModule DeepCopy() { return null; }  //TODO: remove this
    }
}