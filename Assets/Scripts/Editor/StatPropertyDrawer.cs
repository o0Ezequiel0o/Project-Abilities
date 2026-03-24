using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer
{
    const float SPACING = 2f;

    const float START_SHOW_PADDING = 3f;
    const float END_SHOW_PADDING = 3f;

    const float HORIZONTAL_PADDING = 10f;

    float FieldHeight => EditorGUIUtility.singleLineHeight + SPACING;
    float LastFieldHeight => EditorGUIUtility.singleLineHeight;

    Color contentBoxColor = new Color(0.3f, 0.3f, 0.3f);

    bool showContent = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (showContent) return FieldHeight * 3 + LastFieldHeight + START_SHOW_PADDING + END_SHOW_PADDING;
        return FieldHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect dropDownButtonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (EditorGUI.DropdownButton(dropDownButtonRect, label, FocusType.Passive))
        {
            showContent = !showContent;
        }

        if (showContent)
        {
            Rect contentRect = new Rect(position.x + 1f, position.y + FieldHeight, position.width - 2f, position.height - FieldHeight);
            EditorGUI.DrawRect(contentRect, contentBoxColor);

            Rect baseValueRect = new Rect(position.x + HORIZONTAL_PADDING * 0.5f, position.y + START_SHOW_PADDING + FieldHeight, position.width - HORIZONTAL_PADDING, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(baseValueRect, property.FindPropertyRelative("baseValue"));

            Rect increaseRect = new Rect(position.x + HORIZONTAL_PADDING * 0.5f, position.y + START_SHOW_PADDING + FieldHeight * 2f, position.width - HORIZONTAL_PADDING, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(increaseRect, property.FindPropertyRelative("increase"));

            Rect valueLimitsRect = new Rect(position.x + HORIZONTAL_PADDING * 0.5f, position.y + START_SHOW_PADDING + FieldHeight * 3f, position.width - HORIZONTAL_PADDING, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(valueLimitsRect, property.FindPropertyRelative("valueLimits"));
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}