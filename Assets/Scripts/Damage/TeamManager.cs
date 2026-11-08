using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.TeamSystem
{
    public class TeamManager : GameService<TeamManager>
    {
        private readonly Dictionary<GameObject, TeamIdentifier> entitiesWithTeam = new Dictionary<GameObject, TeamIdentifier>();
        private readonly Dictionary<Teams, List<GameObject>> targeteables = new Dictionary<Teams, List<GameObject>>();

        private readonly Teams[] teams = (Teams[])Enum.GetValues(typeof(Teams));

        public void Add(GameObject obj, TeamIdentifier identifier)
        {
            if (entitiesWithTeam.ContainsKey(obj)) return;

            entitiesWithTeam.Add(obj, identifier);

            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i] != identifier.Team || identifier.Team == Teams.IgnoreTeam)
                {
                    targeteables[teams[i]].Add(obj);
                }
            }
        }

        public void Remove(GameObject obj, TeamIdentifier identifier)
        {
            entitiesWithTeam.Remove(obj);

            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i] != identifier.Team || identifier.Team == Teams.IgnoreTeam)
                {
                    targeteables[teams[i]].Remove(obj);
                }
            }
        }

        public void ChangeTeams(GameObject obj, GameObject targetObj)
        {
            ChangeTeams(obj, GetTeam(targetObj));
        }

        public void ChangeTeams(GameObject obj, Teams newTeam)
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

        public Teams GetTeam(GameObject obj)
        {
            if (entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
            {
                return identifier.Team;
            }

            return Teams.IgnoreTeam;
        }

        public GameObject GetRandomEnemy(GameObject obj)
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

        public List<GameObject> GetEnemies(TeamIdentifier identifier)
        {
            if (identifier != null) return targeteables[identifier.Team];
            return targeteables[Teams.IgnoreTeam];
        }

        public List<GameObject> GetEnemies(GameObject obj)
        {
            if (entitiesWithTeam.TryGetValue(obj, out TeamIdentifier identifier))
            {
                return targeteables[identifier.Team];
            }

            return targeteables[Teams.IgnoreTeam];
        }

        public bool IsEnemy(GameObject obj1, GameObject obj2)
        {
            return !IsAlly(obj1, obj2);
        }

        public bool IsEnemy(Teams team, GameObject obj1)
        {
            return !IsAlly(obj1, team);
        }

        public bool IsEnemy(GameObject obj1, Teams team)
        {
            return !IsAlly(obj1, team);
        }

        public bool IsAlly(GameObject obj1, GameObject obj2)
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

        public bool IsAlly(Teams team, GameObject obj1)
        {
            return IsAlly(obj1, team);
        }

        public bool IsAlly(GameObject obj1, Teams team)
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

        protected override void InitializeService()
        {
            for (int i = 0; i < teams.Length; i++)
            {
                targeteables.Add(teams[i], new List<GameObject>());
            }
        }

        private bool HasTeamIdentifier(GameObject obj)
        {
            return entitiesWithTeam.ContainsKey(obj);
        }
    }
}