using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    private void Start()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsInactive.Include);

        for (int i = 0; i < players.Length; i++)
        {
            MovePlayerToPoint(players[i]);
        }
    }

    private void OnEnable()
    {
        GameInstance.onPlayerSpawned += MovePlayerToPoint;
    }

    private void OnDisable()
    {
        GameInstance.onPlayerSpawned -= MovePlayerToPoint;
    }

    private void MovePlayerToPoint(Player player)
    {
        player.transform.position = transform.position;
    }
}