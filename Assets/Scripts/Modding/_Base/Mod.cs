using UnityEngine;

namespace Zeke.Modding
{
    public abstract class Mod : ScriptableObject
    {
        public abstract void OnRunStart();
    }
}