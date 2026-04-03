using UnityEngine;

namespace Zeke.TeamSystem
{
    [DisallowMultipleComponent]
    public class TeamIdentifier : MonoBehaviour
    {
        [SerializeField] private Teams team;

        public Teams Team => team;

        public void ChangeTeam(Teams team)
        {
            TeamManager.ChangeTeams(gameObject, team);
        }

        private void OnEnable()
        {
            TeamManager.Add(gameObject, this);
        }

        private void OnDisable()
        {
            TeamManager.Remove(gameObject, this);
        }

        private void OnDestroy()
        {
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
}