using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Tooltips
{
    [CreateAssetMenu(fileName = "Tooltip Links Database", menuName = "Tooltip/Links Database", order = 1)]
    public class TooltipLinksDB : ScriptableObject
    {
        [SerializeField] private Dictionary<string, TooltipInfo> database = new Dictionary<string, TooltipInfo>();

        [Serializable]
        public struct TooltipInfo
        {
            [TextArea(5, 5)] public string description;
        }
    }
}