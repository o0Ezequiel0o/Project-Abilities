using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Death Rewards", menuName = "Death Rewards", order = 1)]
public class DeathRewardsData : ScriptableObject
{
    [SerializeReferenceDropdown, SerializeReference] private List<DeathReward> rewards;

    public List<DeathReward> Rewards => rewards;
}