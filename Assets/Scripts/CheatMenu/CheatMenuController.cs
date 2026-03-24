using System.Collections.Generic;
using UnityEngine;

public class CheatMenuController : Singleton<CheatMenuController>
{
    [SerializeField] private List<CheatMenuCommand> cheats;

    protected override void OnInitialization()
    {

    }
}