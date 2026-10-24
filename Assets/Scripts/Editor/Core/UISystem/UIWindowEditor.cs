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
            UIWindow window = (UIWindow)target;

            if (!Application.isPlaying)
            {
                SyncEditorDictionary(window);
            }

            DrawDefaultInspector();

            boxStyle ??= new GUIStyle(EditorStyles.helpBox);

            CreateTableHeader(window);

            EditorGUILayout.BeginVertical(boxStyle);

            CreateTableElements(window);

            EditorGUILayout.EndVertical();
        }

        private void CreateTableHeader(UIWindow window)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label("Key", EditorStyles.toolbarButton, GUILayout.Width(160));
            GUILayout.Label("UIElement", EditorStyles.toolbarButton);
            GUILayout.Label(window.Elements.Count.ToString(), EditorStyles.toolbarButton, GUILayout.Width(34));

            EditorGUILayout.EndHorizontal();
        }

        private void CreateTableElements(UIWindow window)
        {
            IReadOnlyDictionary<string, UIElement> elementsDict = window.Elements;

            if (elementsDict == null || elementsDict.Count == 0)
            {
                EditorGUILayout.HelpBox("No UI Elements found under this window.", MessageType.Info);
            }
            else
            {
                foreach (var pair in elementsDict)
                {
                    DrawElementRow(pair.Key, pair.Value);
                }
            }
        }

        private void SyncEditorDictionary(UIWindow window)
        {
            Dictionary<string, UIElement> builtDictionary = new Dictionary<string, UIElement>();
            List<UIElement> elementsList = new List<UIElement>();

            GetNonNestedChildren(window.transform, window.transform, elementsList);

            foreach (var element in elementsList)
            {
                if (element != null && !string.IsNullOrEmpty(element.Name))
                {
                    if (!builtDictionary.ContainsKey(element.Name))
                    {
                        builtDictionary.Add(element.Name, element);
                    }
                }
            }

            if (!AreDictionariesEqual(window.Elements, builtDictionary))
            {
                window.SetDictionary(builtDictionary);
                EditorUtility.SetDirty(window);
            }
        }

        private bool AreDictionariesEqual(IReadOnlyDictionary<string, UIElement> current, Dictionary<string, UIElement> upcoming)
        {
            if (current.Count != upcoming.Count)
            {
                return false;
            }

            foreach (var kvp in upcoming)
            {
                if (!current.TryGetValue(kvp.Key, out UIElement existingElement))
                {
                    return false;
                }

                if (existingElement != kvp.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private void DrawElementRow(string key, UIElement element)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;

            EditorGUILayout.TextField(key, GUILayout.Width(150));
            EditorGUILayout.ObjectField(element != null ? element : null, typeof(UIElement), true);

            GUI.enabled = true;

            if (element != null)
            {
                GUIContent locateIcon = EditorGUIUtility.IconContent("d_ViewToolZoom");
                locateIcon.tooltip = "Ping object in hierarchy";

                if (GUILayout.Button(locateIcon, GUILayout.Width(25), GUILayout.Height(18)))
                {
                    Selection.activeGameObject = element.gameObject;
                    EditorGUIUtility.PingObject(element.gameObject);
                }
            }

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