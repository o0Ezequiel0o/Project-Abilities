using UnityEngine;

namespace Zeke.Items
{
    public abstract class Item
    {
        public static bool RollProc(float chance, float coefficient, int luck)
        {
            if (coefficient == 0 || chance == 0) return false;

            bool rollSucess = chance * coefficient > Random.Range(0f, 100f - Mathf.Epsilon);

            if (luck < 0 && rollSucess)
            {
                return RollProc(chance, coefficient, luck + 1);
            }
            if (luck > 0 && !rollSucess)
            {
                return RollProc(chance, coefficient, luck - 1);
            }

            return rollSucess;
        }

        public abstract ItemData Data { get; }

        public int stacks;

        public virtual void OnAdded() { }

        public virtual void OnRemoved() { }

        public virtual void OnStackAdded() { }

        public virtual void OnStackRemoved() { }

        public virtual void OnUpdate() { }
    }
}