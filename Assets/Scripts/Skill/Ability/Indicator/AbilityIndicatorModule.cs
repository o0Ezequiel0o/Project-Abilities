using UnityEngine;

using static Zeke.Abilities.Indicators.AbilityIndicatorModuleData;

namespace Zeke.Abilities.Indicators
{
    public class AbilityIndicatorModule
    {
        public readonly AbilityIndicatorModuleData data;

        private Transform sourceTransform;
        private Transform spawnTransform;

        private AttackIndicator attackIndicatorInstance;

        public AbilityIndicatorModule() { }

        public AbilityIndicatorModule(AbilityIndicatorModuleData data)
        {
            this.data = data;
        }

        public void Initialize(GameObject source, Transform spawn, AbilityIndicatorSettings settings)
        {
            sourceTransform = source.transform;
            spawnTransform = spawn;

            SpawnAttackIndicator(settings);
        }

        public void Update(float currentTime)
        {
            if (attackIndicatorInstance == null) return;

            if (currentTime >= data.ShowTime)
            {
                if (currentTime < data.HideTime)
                {
                    UpdateActive(currentTime);
                }
                else
                {
                    UpdateInactive(currentTime);
                }
            }
        }

        public void LateUpdate()
        {
            if (data.SpawnPosition == StartingPoint.Source)
            {
                Follow(sourceTransform);
            }
            else
            {
                Follow(spawnTransform);
            }
        }

        public void Enable()
        {
            if (attackIndicatorInstance == null) return;
            attackIndicatorInstance.gameObject.SetActive(true);
        }

        public void Disable()
        {
            if (attackIndicatorInstance == null) return;
            attackIndicatorInstance.gameObject.SetActive(false);
        }

        public void Reset(GameObject source, Transform castTransform)
        {
            sourceTransform = source.transform;
            spawnTransform = castTransform;
        }

        public void Destroy()
        {
            if (attackIndicatorInstance == null) return;
            GameObject.Destroy(attackIndicatorInstance.gameObject);
        }

        private void UpdateActive(float currentTime)
        {
            attackIndicatorInstance.UpdateFill(GetTimePercentage(currentTime));

            if (!attackIndicatorInstance.gameObject.activeSelf)
            {
                attackIndicatorInstance.gameObject.SetActive(true);
            }
        }

        private void UpdateInactive(float currentTime)
        {
            if (attackIndicatorInstance.gameObject.activeSelf)
            {
                attackIndicatorInstance.gameObject.SetActive(false);
            }
        }

        private void Follow(Transform target)
        {
            Vector3 position = target.position + (spawnTransform.rotation * data.Offset);
            Quaternion rotation = Quaternion.Euler(0f, 0f, spawnTransform.rotation.eulerAngles.z - data.AngleOffset);

            attackIndicatorInstance.transform.SetPositionAndRotation(position, rotation);
        }

        private void SpawnAttackIndicator(AbilityIndicatorSettings settings)
        {
            attackIndicatorInstance = data.AttackIndicator.CreateInstance(settings);

            attackIndicatorInstance.transform.SetParent(Run.Instance.WorldCanvas.transform);
            attackIndicatorInstance.gameObject.SetActive(false);
        }

        private float GetTimePercentage(float currentTime)
        {
            return Mathf.InverseLerp(data.ShowTime, data.HideTime, currentTime);
        }
    }
}