using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zeke.UI
{
    [CustomEditor(typeof(UIWindow))]
    public class UIWindowEditor : Editor
    {
        private GUIStyle boxStyle;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                CreateEditorHelpBox();
            }

            boxStyle ??= new GUIStyle(EditorStyles.helpBox);

            CreateTableHeader();

            EditorGUILayout.BeginVertical(boxStyle);

            UIWindow window = (UIWindow)target;

            if (Application.isPlaying)
            {
                CreateRuntimeTable(window);
            }
            else
            {
                CreateEditorTable(window);
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateEditorHelpBox()
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField($"Editor Preview (Hierarchy Scan)", EditorStyles.helpBox);

            EditorGUILayout.Space(5);
        }

        private void CreateTableHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Key (String)", EditorStyles.toolbarButton, GUILayout.Width(150));
            GUILayout.Label("UIElement GameObject", EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();
        }

        private void CreateRuntimeTable(UIWindow window)
        {
            IReadOnlyDictionary<string, UIElement> elementsDict = window.Elements;

            if (elementsDict.Count == 0)
            {
                EditorGUILayout.HelpBox("No active UI Elements registered in dictionary.", MessageType.Info);
            }
            else
            {
                foreach (var pair in elementsDict)
                {
                    DrawElementRow(pair.Key, pair.Value);
                }
            }
        }

        private void CreateEditorTable(UIWindow window)
        {
            List<UIElement> elementsList = new List<UIElement>();
            GetNonNestedChildren(window.transform, window.transform, elementsList);

            if (elementsList.Count == 0)
            {
                EditorGUILayout.HelpBox("No UI Elements found under this window.", MessageType.Info);
            }
            else
            {
                foreach (var element in elementsList)
                {
                    if (element == null) continue;
                    string key = string.IsNullOrEmpty(element.Name) ? "(Unnamed)" : element.Name;
                    DrawElementRow(key, element);
                }
            }
        }

        private void DrawElementRow(string key, UIElement element)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;
            EditorGUILayout.TextField(key, GUILayout.Width(150));

            GUI.enabled = true;
            EditorGUILayout.ObjectField(element != null ? element.gameObject : null, typeof(GameObject), true);

            EditorGUILayout.EndHorizontal();
        }

        private void GetNonNestedChildren(Transform rootWindowTransform, Transform currentParent, List<UIElement> results)
        {
            for (int i = 0; i < currentParent.childCount; i++)
            {
                Transform child = currentParent.GetChild(i);

                if (child != rootWindowTransform && child.TryGetComponent<UIWindow>(out _))
                {
                    continue;
                }

                if (child.TryGetComponent<UIElement>(out var uiElement))
                {
                    results.Add(uiElement);
                }

                GetNonNestedChildren(rootWindowTransform, child, results);
            }
        }
    }
}