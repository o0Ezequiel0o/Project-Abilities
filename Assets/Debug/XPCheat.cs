using System.Collections.Generic;
using UnityEngine;

public class XPCheat : MonoBehaviour
{
    [SerializeField] private int experience;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            IReadOnlyList<Player> players = GameInstance.Players;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].TryGetComponent(out LevelHandler levelHandler))
                {
                    levelHandler.GiveExperience(experience);
                }
            }
        }
    }
}