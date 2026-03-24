using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Limits))]
public class LimitsPropertyDrawer : PropertyDrawer
{
    const int LABELS_AMOUNT = 2;
    const int FIELDS_AMOUNT = 2;

    const float LABEL_WIDTH = 26f;
    const float LABEL_SPACING = 3f;
    const float FIELD_SPACING = 3f;

    const float END_PADDING = 6f;
    const float WIDTH_MULTIPLIER = 1f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive), label);

        float maxWidthToUse = position.width * WIDTH_MULTIPLIER;
        float propertyFieldWidth = Mathf.Max(0, (maxWidthToUse - LABEL_WIDTH * LABELS_AMOUNT - (FIELD_SPACING * (FIELDS_AMOUNT - 1)) - END_PADDING) / FIELDS_AMOUNT);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Vector2 currentDrawPosition = new Vector2(position.x, position.y);

        Rect minLabelRect = new Rect(currentDrawPosition.x, currentDrawPosition.y, LABEL_WIDTH, position.height);
        currentDrawPosition = new Vector2(currentDrawPosition.x + LABEL_WIDTH + LABEL_SPACING, currentDrawPosition.y);

        Rect minFieldRect = new Rect(currentDrawPosition.x, currentDrawPosition.y, propertyFieldWidth, position.height);
        currentDrawPosition = new Vector2(currentDrawPosition.x + propertyFieldWidth + FIELD_SPACING, currentDrawPosition.y);

        Rect maxLabelRect = new Rect(currentDrawPosition.x, currentDrawPosition.y, LABEL_WIDTH, position.height);
        currentDrawPosition = new Vector2(currentDrawPosition.x + LABEL_WIDTH + LABEL_SPACING, currentDrawPosition.y);

        Rect maxFieldRect = new Rect(currentDrawPosition.x, currentDrawPosition.y, propertyFieldWidth, position.height);

        EditorGUI.LabelField(minLabelRect, "Min");
        EditorGUI.LabelField(maxLabelRect, "Max");

        EditorGUI.PropertyField(minFieldRect, property.FindPropertyRelative("min"), GUIContent.none);
        EditorGUI.PropertyField(maxFieldRect, property.FindPropertyRelative("max"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}