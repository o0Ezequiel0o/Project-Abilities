using System.Collections.Generic;
using UnityEngine;

namespace Zeke.TeamSystem
{
    public static class TeamManager
    {
        private static readonly Dictionary<GameObject, TeamIdentifier> entitiesWithTeam = new Dictionary<GameObject, TeamIdentifier>();
        private static readonly Dictionary<Teams, List<GameObject>> targeteables = new Dictionary<Teams, List<GameObject>>()
        {
            {Teams.Team1, new List<GameObject>()},
            {Teams.Team2, new List<GameObject>()},
            {Teams.Team3, new List<GameObject>()},
            {Teams.Team4, new List<GameObject>()},
            {Teams.Team5, new List<GameObject>()},
            {Teams.Team6, new List<GameObject>()},
            {Teams.Team7, new List<GameObject>()},
            {Teams.Team8, new List<GameObject>()},
            {Teams.IgnoreTeam, new List<GameObject>()}
        };

        public static void Add(GameObject obj, TeamIdentifier identifier)
        {
            if (entitiesWithTeam.ContainsKey(obj)) return;

            entitiesWithTeam.Add(obj, identifier);

            foreach (Teams team in targeteables.Keys)
            {
                if (team != identifier.Team || identifier.Team == Teams.IgnoreTeam)
                {
                    targeteables[team].Add(obj);
                }
            }
        }

        public static void Remove(GameObject obj, TeamIdentifier identifier)
        {
            entitiesWithTeam.Remove(obj);

            foreach (Teams team in targeteables.Keys)
            {
                if (team != identifier.Team || identifier.Team == Teams.IgnoreTeam)
                {
                    targeteables[team].Remove(obj);
                }
            }
        }

        public static void ChangeTeams(GameObject obj, GameObject targetObj)
        {
            ChangeTeams(obj, GetTeam(targetObj));
        }

        public static void ChangeTeams(GameObject obj, Teams newTeam)
        {
            if (!HasTeamIdentifier(obj)) return;

            Teams oldTeam = GetTeam(obj);

            if (oldTeam == newTeam) return;

            if (oldTeam != Teams.IgnoreTeam)
            {
                targeteables[oldTeam].Add(obj);
            }

            if (newTeam != Teams.IgnoreTeam && targeteables.TryGetValue(newTeam, out List<GameObject> enemiesList))
            {
                enemiesList.Remove(obj);
            }
        }

        public static Teams GetTeam(GameObject obj)
        {
            if (entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
            {
                return identifier.Team;
            }

            return Teams.IgnoreTeam;
        }

        public static GameObject GetRandomEnemy(GameObject obj)
        {
            entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier);

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
            if (identifier != null) return targeteables[identifier.Team];
            return targeteables[Teams.IgnoreTeam];
        }

        public static List<GameObject> GetEnemies(GameObject obj)
        {
            if (entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
            {
                return targeteables[identifier.Team];
            }

            return targeteables[Teams.IgnoreTeam];
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

            if (entitiesWithTeam.TryGetValue(obj1, out TeamIdentifier sourceTeam) && entitiesWithTeam.TryGetValue(obj2, out TeamIdentifier receiverTeam))
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

            if (entitiesWithTeam.TryGetValue(obj1, out TeamIdentifier sourceTeam))
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

        private static bool HasTeamIdentifier(GameObject obj)
        {
            return entitiesWithTeam.ContainsKey(obj);
        }
    }
}