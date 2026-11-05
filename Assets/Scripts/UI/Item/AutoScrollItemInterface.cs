using UnityEngine;
using Zeke.UI;

namespace Zeke.Items
{
    public class AutoScrollItemInterface : AutoScrollList
    {
        [SerializeField] private ItemsInterface itemsInterface;

        [Space]

        [SerializeField] private float stopMovementTime = 1f;

        protected override float CurrentScrollSpeed
        {
            get
            {
                if (timer <= 0f)
                {
                    return scrollSpeed;
                }

                return 0f;
            }
        }

        private float timer = 0f;

        protected override void Reset()
        {
            base.Reset();

            itemsInterface = GetComponentInChildren<ItemsInterface>();
        }

        private void OnEnable()
        {
            itemsInterface.onItemModified += OnItemModified;
        }

        private void OnDisable()
        {
            itemsInterface.onItemModified -= OnItemModified;
        }

        protected override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
        }

        private void OnItemModified(UIWindow window)
        {
            timer = stopMovementTime;
        }
    }
}