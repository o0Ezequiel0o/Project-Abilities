using UnityEngine;
using System;

[Serializable]
public class StatData : MonoBehaviour
{
    [SerializeField] private float baseValue = 1f;
    [SerializeField] private float increase = 0f;
    [SerializeField] private Limits valueLimits;

    public Stat CreateStat()
    {
        return new Stat(baseValue, increase, valueLimits.Min, valueLimits.Max);
    }
}