using UnityEngine;
using System;

[Serializable]
public struct LinearStat : IStackStat
{
    [SerializeField] private float startValue;
    [SerializeField] private float increase;

    public readonly float GetValue(int stacks)
    {
        return startValue + (increase * stacks);
    }
}