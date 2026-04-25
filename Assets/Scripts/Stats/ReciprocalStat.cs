using System;
using UnityEngine;

[Serializable]
public struct ReciprocalStat : IStackStat
{
    [SerializeField] private float startExponent;

    [SerializeField] private float baseValue;
    [SerializeField] private float coefficient;

    public readonly float GetValue(int stacks)
    {
        return coefficient * Mathf.Pow(baseValue, startExponent + stacks);
    }
}
