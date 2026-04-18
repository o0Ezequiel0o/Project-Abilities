using UnityEngine;
using System;

[Serializable]
public struct ExponentialStat : IStackStat
{
    [SerializeField] private float startValue;
    [SerializeField] private float baseValue;

    public readonly float GetValue(int stacks)
    {
        return startValue + Mathf.Pow(baseValue, stacks);
    }
}