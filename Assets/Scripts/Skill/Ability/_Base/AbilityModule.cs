using UnityEngine;

namespace Zeke.Abilities
{
    public abstract class AbilityModule
    {
        //add a default implementation for Initialize and move OnInitialization after this

        /// <summary>
        /// Called when the ability is initiliazed.
        /// </summary>
        public abstract void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability);

        /// <summary>
        /// Called when the ability is activated / used.
        /// </summary>
        public abstract void Activate(bool holding);

        /// <summary>
        /// Called when the ability finished it's duration, is disabled while active, or instantly if it has a duration of zero.
        /// </summary>
        public virtual void Deactivate() { }

        /// <summary>
        /// Called when something tries to activate the ability.
        /// </summary>
        public abstract bool CanActivate();

        /// <summary>
        /// Called when something tries to upgrade the ability.
        /// </summary>
        public abstract bool CanUpgrade();

        /// <summary>
        /// Runs in the update loop only if it's active.
        /// </summary>
        public virtual void UpdateActive() { }

        /// <summary>
        /// Runs in the update loop only if it's inactive.
        /// </summary>
        public virtual void UpdateInactive() { }

        /// <summary>
        /// Runs in the unity's Update loop.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Runs in the unity's Late Update loop.
        /// </summary>
        public virtual void LateUpdate() { }

        /// <summary>
        /// Called when the ability is upgraded.
        /// </summary>
        public virtual void Upgrade(int levels) { }

        /// <summary>
        /// Called when an ability is disabled and no longer upates active or inactive updates.
        /// </summary>
        public virtual void Disable() { }

        /// <summary>
        /// Called when an ability is destroyed.
        /// </summary>
        public virtual void Destroy() { }
    }
}