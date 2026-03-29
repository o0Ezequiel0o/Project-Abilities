using UnityEditor;
using UnityEngine;

namespace Zeke.TeamSystem
{
    [CustomEditor(typeof(TeamIdentifier))]
    public class TeamIdentifierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            DrawDefaultInspector();
            EditorGUI.EndDisabledGroup();
        }
    }
}