using UnityEngine;
using System;

namespace Zeke.Abilities.Indicators
{
    [Serializable]
    public class AbilityIndicatorModule
    {
        [SerializeField] private AttackIndicatorData attackIndicator;

        [SerializeField] private StartingPoint spawnPosition;
        [SerializeField] private Vector2 offset;
        [SerializeField] private float angleOffset;

        [SerializeField] private float showTime;
        [SerializeField] private float hideTime;

        public float ShowTime => showTime;
        public float HideTime => hideTime;

        private Transform sourceTransform;
        private Transform spawnTransform;

        private AttackIndicator attackIndicatorInstance;

        public AbilityIndicatorModule() { }

        public AbilityIndicatorModule(AbilityIndicatorModule original)
        {
            offset = original.offset;
            angleOffset = original.angleOffset;

            showTime = original.showTime;
            hideTime = original.hideTime;

            attackIndicator = original.attackIndicator.DeepCopy();
        }

        public AbilityIndicatorModule DeepCopy() => new AbilityIndicatorModule(this);

        public void Initialize(GameObject source, Transform spawn, AbilityIndicatorSettings settings)
        {
            sourceTransform = source.transform;
            spawnTransform = spawn;

            SpawnAttackIndicator(settings);
        }

        public void Update(float currentTime)
        {
            if (attackIndicatorInstance == null) return;

            if (currentTime >= showTime)
            {
                if (currentTime < hideTime)
                {
                    UpdateActive(currentTime);
                }
                else
                {
                    UpdateUnactive(currentTime);
                }
            }
        }

        public void LateUpdate()
        {
            if (spawnPosition == StartingPoint.Source)
            {
                Follow(sourceTransform);
            }
            else
            {
                Follow(spawnTransform);
            }
        }

        public void Reset()
        {
            if (attackIndicatorInstance == null) return;
            attackIndicatorInstance.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            if (attackIndicatorInstance == null) return;
            GameObject.Destroy(attackIndicatorInstance.gameObject);
        }

        private void UpdateActive(float currentTime)
        {
            if (!attackIndicatorInstance.gameObject.activeSelf)
            {
                attackIndicatorInstance.gameObject.SetActive(true);
            }

            attackIndicatorInstance.UpdateFill(GetTimePercentage(currentTime));
        }

        private void UpdateUnactive(float currentTime)
        {
            if (attackIndicatorInstance.gameObject.activeSelf)
            {
                attackIndicatorInstance.gameObject.SetActive(false);
            }
        }

        private void Follow(Transform target)
        {
            Vector3 position = target.position + (spawnTransform.rotation * offset);
            Quaternion rotation = Quaternion.Euler(0f, 0f, spawnTransform.rotation.eulerAngles.z - angleOffset);

            attackIndicatorInstance.transform.SetPositionAndRotation(position, rotation);
        }

        private void SpawnAttackIndicator(AbilityIndicatorSettings settings)
        {
            attackIndicatorInstance = attackIndicator.CreateInstance(settings);

            attackIndicatorInstance.transform.SetParent(GameInstance.WorldCanvas.transform);
            attackIndicatorInstance.gameObject.SetActive(false);
        }

        private float GetTimePercentage(float currentTime)
        {
            return Mathf.InverseLerp(showTime, hideTime, currentTime);
        }

        private enum StartingPoint
        {
            [InspectorName("Cast Position")]
            CastPosition,
            [InspectorName("Source Center")]
            Source
        }
    }
}