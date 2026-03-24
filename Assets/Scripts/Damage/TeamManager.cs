using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamManager : Singleton<TeamManager>
{
    private readonly Dictionary<GameObject, TeamIdentifier> entitiesWithTeam = new Dictionary<GameObject, TeamIdentifier>();
    private readonly Dictionary<Teams, List<GameObject>> enemiesOf = new Dictionary<Teams, List<GameObject>>();

    public static void Add(GameObject obj, TeamIdentifier identifier)
    {
        if (Instance.entitiesWithTeam.ContainsKey(obj)) return;

        Instance.entitiesWithTeam.Add(obj, identifier);
        identifier.onTeamChanged += Instance.OnTeamChanged;

        foreach(Teams team in Instance.enemiesOf.Keys)
        {
            if (team != identifier.Team || identifier.Team == Teams.IgnoreTeam)
            {
                Instance.enemiesOf[team].Add(obj);
            }
        }
    }

    public static void Remove(GameObject obj, TeamIdentifier identifier)
    {
        Instance.entitiesWithTeam.Remove(obj);
        identifier.onTeamChanged -= Instance.OnTeamChanged;

        foreach (Teams team in Instance.enemiesOf.Keys)
        {
            if (team != identifier.Team || identifier.Team == Teams.IgnoreTeam)
            {
                Instance.enemiesOf[team].Remove(obj);
            }
        }
    }

    public static Teams GetTeam(GameObject obj)
    {
        if (Instance.entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
        {
            return identifier.Team;
        }

        return Teams.IgnoreTeam;
    }

    public static GameObject GetRandomEnemy(GameObject obj)
    {
        Instance.entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier);

        List<GameObject> enemies = GetEnemies(identifier);

        int randomRoll = UnityEngine.Random.Range(0, enemies.Count);

        if (identifier.Team == Teams.IgnoreTeam)
        {
            if (enemies.Count <= 1) return null;

            if (enemies[randomRoll] == obj)
            {
                if (randomRoll < enemies.Count - 1)
                {
                    return enemies[randomRoll + 1];
                }
                if (randomRoll > 0)
                {
                    return enemies[randomRoll - 1];
                }
            }

            return enemies[randomRoll];
        }
        else if (enemies.Count > 0)
        {
            return enemies[randomRoll];
        }

        return null;
    }

    public static List<GameObject> GetEnemies(TeamIdentifier identifier)
    {
        if (identifier != null) return Instance.enemiesOf[identifier.Team];
        return Instance.enemiesOf[Teams.IgnoreTeam];
    }

    public static List<GameObject> GetEnemies(GameObject obj)
    {
        if (Instance.entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
        {
            return Instance.enemiesOf[identifier.Team];
        }

        return Instance.enemiesOf[Teams.IgnoreTeam];
    }

    public static bool IsEnemy(GameObject obj1, GameObject obj2)
    {
        return !IsAlly(obj1, obj2);
    }

    public static bool IsEnemy(Teams team, GameObject obj1)
    {
        return !IsAlly(obj1, team);
    }

    public static bool IsEnemy(GameObject obj1, Teams team)
    {
        return !IsAlly(obj1, team);
    }

    public static bool IsAlly(GameObject obj1, GameObject obj2)
    {
        if (obj1 == null || obj2 == null) return false;

        if (Instance.entitiesWithTeam.TryGetValue(obj1, out TeamIdentifier sourceTeam) && Instance.entitiesWithTeam.TryGetValue(obj2, out TeamIdentifier receiverTeam))
        {
            if (sourceTeam.Team == Teams.IgnoreTeam || receiverTeam.Team == Teams.IgnoreTeam)
            {
                return false;
            }
            if (sourceTeam.Team == receiverTeam.Team)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsAlly(Teams team, GameObject obj1)
    {
        return IsAlly(obj1, team);
    }

    public static bool IsAlly(GameObject obj1, Teams team)
    {
        if (obj1 == null || team == Teams.IgnoreTeam) return false;

        if (Instance.entitiesWithTeam.TryGetValue(obj1, out TeamIdentifier sourceTeam))
        {
            if (sourceTeam.Team == Teams.IgnoreTeam)
            {
                return false;
            }
            if (sourceTeam.Team == team)
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnInitialization()
    {
        foreach (Teams enumValue in Enum.GetValues(typeof(Teams)))
        {
            enemiesOf.Add(enumValue, new List<GameObject>());
        }
    }

    private void OnTeamChanged(GameObject obj, Teams oldTeam, Teams newTeam)
    {
        if (enemiesOf.TryGetValue(newTeam, out List<GameObject> newTeamList))
        {
            newTeamList.Remove(obj);
        }

        if (oldTeam != Teams.IgnoreTeam)
        {
            enemiesOf[oldTeam].Add(obj);
        }
    }
}