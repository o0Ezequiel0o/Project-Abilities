using UnityEngine;
using System;

public class TeamIdentifier : MonoBehaviour
{
    [SerializeField] private Teams team;

    public Teams Team
    {
        get
        {
            if (team != lastTeamValue)
            {
                onTeamChanged?.Invoke(gameObject, lastTeamValue, team);
                lastTeamValue = team;
            }

            return team;
        }

        set
        {
            team = value;
        }
    }

    public Action<GameObject, Teams, Teams> onTeamChanged;

    private Teams lastTeamValue = Teams.IgnoreTeam;

    void Awake()
    {
        lastTeamValue = team;
    }

    private void OnEnable()
    {
        TeamManager.Add(gameObject, this);
    }

    private void OnDisable()
    {
        if (TeamManager.IsNull) return;
        TeamManager.Remove(gameObject, this);
    }

    void OnDestroy()
    {
        if (TeamManager.IsNull) return;
        TeamManager.Remove(gameObject, this);
    }
}

public enum Teams
{
    Team1,
    Team2,
    Team3,
    Team4,
    Team5,
    Team6,
    Team7,
    Team8,
    IgnoreTeam,
}