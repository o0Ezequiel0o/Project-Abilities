using UnityEngine;

public class EngineerTurret : MonoBehaviour
{
    [SerializeField] private LevelHandler levelHandler;
    [SerializeField] private TeamIdentifier teamIdentifier;

    private LevelHandler ownerLevelHandler;

    public void Initialize(GameObject owner)
    {
        teamIdentifier.Team = TeamManager.GetTeam(owner);
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