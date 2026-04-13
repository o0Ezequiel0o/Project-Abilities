using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Indicators;

namespace Zeke.Abilities
{
    public class Ability : IAbility
    {
        public AbilityData Data => data;
        public AbilityIndicatorData IndicatorData => data.IndicatorData;

        public int Level { get; private set; }

        public int Charges { get; private set; }
        public int MaxCharges => maxCharges.ValueInt;

        public float ChargePercentage
        {
            get
            {
                if (CooldownTime <= 0f || Charges == MaxCharges) return 1f;
                return Mathf.Clamp01(CooldownTimer / CooldownTime);
            }
        }

        public float DurationPercentage
        {
            get
            {
                if (DurationTime <= 0f)
                {
                    return 1f;
                }

                return Mathf.Clamp01(DurationTimer / DurationTime);
            }
        }

        public float CooldownTime => cooldownTime.Value * CooldownMultiplier;
        public float CooldownTimer { get; protected set; }

        public float DurationTime => durationTime.Value;
        public float DurationTimer { get; protected set; }

        public bool DurationActive { get; protected set; }

        public bool HasCharges => Charges > 0;

        private bool UsesDuration => DurationTime > 0f;

        private readonly AbilityController controller;

        private readonly Stat cooldownTime;
        private readonly Stat durationTime;
        private readonly Stat maxCharges;

        private readonly Transform spawn;
        private readonly GameObject source;

        private readonly AbilityData data;
        private readonly List<AbilityModule> modules;

        private float CooldownMultiplier => controller.abilityCooldownMultiplier[Data.AbilityType].Value;

        private int queuedUpgrades = 0;

        public Ability(GameObject source, AbilityData data, AbilityController controller, Transform spawn, Stat cooldownTime, Stat durationTime, Stat maxCharges)
        {
            this.data = data;
            this.spawn = spawn;
            this.source = source;
            this.controller = controller;
            modules = new List<AbilityModule>();

            this.cooldownTime = cooldownTime.DeepCopy();
            this.durationTime = durationTime.DeepCopy();
            this.maxCharges = maxCharges.DeepCopy();
        }

        public void Initialize()
        {
            Level = 1;
            Charges = 1;

            DurationTimer = 0f;
            DurationActive = false;

            CooldownTimer = CooldownTime;
        }

        public void AddModule(AbilityModule module)
        {
            AbilityModule newModule = module.DeepCopy();
            newModule.OnInitialization(controller, spawn, source, this);
            modules.Add(newModule);
        }

        public void AddModules(List<AbilityModule> modules)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                AddModule(modules[i]);
            }
        }

        public void SetCharges(int amount)
        {
            Charges = Mathf.Clamp(amount, 0, MaxCharges);

            if (Charges == MaxCharges)
            {
                CooldownTimer = CooldownTime;
            }
        }

        public void SetDuration(int amount)
        {
            DurationTimer = Mathf.Max(0f, amount);
        }

        public void SetCooldownTimer(float amount)
        {
            if (Charges == MaxCharges) return;
            
            CooldownTimer = Mathf.Clamp(amount, 0f, CooldownTime);

            if (CooldownTimer >= CooldownTime)
            {
                SetCharges(Charges + 1);

                if (Charges < MaxCharges)
                {
                    CooldownTimer = 0f;
                }
            }
        }

        public bool TryActivate(bool holding)
        {
            if (CanActivate() && CanActivateBase())
            {
                Activate(holding);

                if (!DurationActive)
                {
                    Deactivate();
                }

                return true;
            }
            else if (DurationActive && data.CanManuallyDeactivate)
            {
                TryDeactivate();
            }

            return false;
        }

        public void TryDeactivate()
        {
            if (DurationActive)
            {
                Deactivate();
            }
        }

        public void QueueUpgrade()
        {
            if (CanUpgrade())
            {
                Upgrade();
            }
            else
            {
                queuedUpgrades += 1;
            }
        }

        public void Update()
        {
            CheckQueuedUpgrades();

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Update();
            }

            if (DurationActive)
            {
                UpdateActiveBase();
                UpdateActive();
            }
            else
            {
                UpdateUnactive();
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].LateUpdate();
            }
        }

        public bool CanActivate()
        {
            if (CooldownTimer < CooldownTime)
            {
                return false;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].CanActivate())
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanUpgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].CanUpgrade())
                {
                    return false;
                }
            }

            return true;
        }

        public void Destroy()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Destroy();
            }
        }

        private void Activate(bool holding)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Activate(holding);
            }

            if (Charges == MaxCharges)
            {
                CooldownTimer = 0f;
            }

            SetCharges(Charges - 1);

            if (UsesDuration)
            {
                DurationActive = true;
            }
        }

        private void Deactivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Deactivate();
            }

            if (UsesDuration)
            {
                DurationTimer = 0f;
                DurationActive = false;
            }
        }

        private void UpdateUnactive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].UpdateUnactive();
            }
        }

        private void UpdateActive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].UpdateActive();
            }
        }

        private void Upgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Upgrade();
            }

            cooldownTime.Upgrade();
            durationTime.Upgrade();
            maxCharges.Upgrade();

            Level += 1;
        }

        private void UpdateActiveBase()
        {
            DurationTimer += Time.deltaTime;

            if (DurationTimer > DurationTime)
            {
                Deactivate();
            }
        }

        private bool CanActivateBase()
        {
            return HasCharges && !DurationActive;
        }

        private void CheckQueuedUpgrades()
        {
            if (queuedUpgrades == 0) return;
            if (!CanUpgrade()) return;

            for (int i = 0; i < queuedUpgrades; i++)
            {
                Upgrade();
            }

            queuedUpgrades = 0;
        }
    }
}