using UnityEngine;

public class EngineerTurret : MonoBehaviour
{
    [SerializeField] private LevelHandler levelHandler;
    [SerializeField] private TeamIdentifier teamIdentifier;

    private LevelHandler ownerLevelHandlerCache;

    public void SetData(GameObject owner)
    {
        teamIdentifier.Team = TeamManager.GetTeam(owner);
        ownerLevelHandlerCache = owner.GetComponent<LevelHandler>();
    }

    private void Awake()
    {
        levelHandler.onExperienceReceived += ShareExperienceWithOwner;
    }

    private void ShareExperienceWithOwner(int experienceReceived)
    {
        if (ownerLevelHandlerCache == null) return;
        ownerLevelHandlerCache.GiveExperience(experienceReceived);
    }
}