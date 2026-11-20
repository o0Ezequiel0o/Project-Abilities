using UnityEngine;

namespace Zeke.Modding
{
    public class ModBootstrapper : GameService<ModBootstrapper>
    {
        [SerializeField] private ModLoadPreset modLoadPreset;

        private void Awake()
        {
            for (int i = 0; i < modLoadPreset.Mods.Count; i++)
            {
                modLoadPreset.Mods[i].OnGameInitialization();
            }
        }
    }
}