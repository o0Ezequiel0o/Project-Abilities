using UnityEngine;
using Zeke.TeamSystem;

public class EngineerTurret : MonoBehaviour
{
    [SerializeField] private LevelHandler levelHandler;

    private LevelHandler ownerLevelHandler;

    public void Initialize(GameObject owner)
    {
        TeamManager.ChangeTeams(gameObject, owner);
        ownerLevelHandler = owner.GetComponent<LevelHandler>();
    }

    private void Awake()
    {
        levelHandler.onExperienceReceived += ShareExperienceWithOwner;
    }

    private void ShareExperienceWithOwner(int experienceReceived)
    {
        if (ownerLevelHandler == null) return;
        ownerLevelHandler.GiveExperience(experienceReceived);
    }
}