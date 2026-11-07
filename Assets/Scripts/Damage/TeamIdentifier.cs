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
            TeamManager.Instance.ChangeTeams(gameObject, team);
            this.team = team;
        }

        private void OnEnable()
        {
            TeamManager.Instance.Add(gameObject, this);
        }

        private void OnDisable()
        {
            TeamManager.Instance.Remove(gameObject, this);
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