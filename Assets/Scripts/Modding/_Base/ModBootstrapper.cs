using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Modding
{
    public class ModBootstrapper : GameService<ModBootstrapper>
    {
        //Make ScriptableObject
        [SerializeField] private List<Mod> mods;

        private void Awake()
        {
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnGameInitialization();
            }
        }
    }
}