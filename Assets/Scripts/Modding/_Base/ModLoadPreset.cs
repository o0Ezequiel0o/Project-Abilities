using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Modding
{
    [CreateAssetMenu(fileName = "New Mod Load Preset", menuName = "Modding/Mods/New Mod Load Preset", order = -2)]
    public class ModLoadPreset : ScriptableObject
    {
        [SerializeField] private List<Mod> mods;

        public IReadOnlyList<Mod> Mods => mods;

        public void LoadPreset(ModLoadPreset preset)
        {
            mods = new List<Mod>(preset.Mods);
        }
    }
}