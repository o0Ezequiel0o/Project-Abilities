using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemGeneratorDrops))]
public class ItemGeneratorDropsEditor : Editor
{
    private SerializedProperty raritySlots;

    private GUIStyle centeredLabel;
    private GUIStyle centeredField;
    private GUIStyle centeredHeader;

    private void OnEnable()
    {
        raritySlots = serializedObject.FindProperty("<RaritySlots>k__BackingField");

        centeredLabel = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };

        centeredField = new GUIStyle(EditorStyles.numberField)
        {
            alignment = TextAnchor.MiddleCenter
        };

        centeredHeader = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
    }

    private int GetTotalWeight()
    {
        int currentWeight = 0;

        for (int i = 0; i < raritySlots.arraySize; i++)
        {
            SerializedProperty slot = raritySlots.GetArrayElementAtIndex(i);
            currentWeight += Mathf.Max(0, slot.FindPropertyRelative("weight").intValue);
        }

        return currentWeight;
    }

    private void CreateTableHeader()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Rarity", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Weight", centeredHeader, GUILayout.Width(65));
        EditorGUILayout.LabelField("Chance", centeredHeader, GUILayout.Width(80));

        GUILayout.Space(22);

        EditorGUILayout.EndHorizontal();
    }

    private void CreateTableElements(int totalWeight)
    {
        for (int i = 0; i < raritySlots.arraySize; i++)
        {
            SerializedProperty slot = raritySlots.GetArrayElementAtIndex(i);
            SerializedProperty rarity = slot.FindPropertyRelative("rarity");
            SerializedProperty weight = slot.FindPropertyRelative("weight");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(rarity, GUIContent.none);

            weight.intValue = EditorGUILayout.IntField(Mathf.Max(1, weight.intValue), centeredField, GUILayout.Width(65));

            float chance = totalWeight > 0 ? Mathf.Max(0, weight.intValue) * 100f / totalWeight : 0f;

            EditorGUILayout.LabelField(totalWeight > 0 ? $"{chance:F2}%" : "N/A", centeredLabel, GUILayout.Width(80));

            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                raritySlots.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void CreateTableAddButton()
    {
        if (GUILayout.Button("Add Rarity Slot"))
        {
            int index = raritySlots.arraySize;

            raritySlots.InsertArrayElementAtIndex(index);

            SerializedProperty slot = raritySlots.GetArrayElementAtIndex(index);

            slot.FindPropertyRelative("rarity").enumValueIndex = 0;
            slot.FindPropertyRelative("weight").intValue = 1;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int totalWeight = GetTotalWeight();

        CreateTableHeader();
        CreateTableElements(totalWeight);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField($"Total Weight: {totalWeight} | Total Elements {raritySlots.arraySize}", EditorStyles.helpBox);

        EditorGUILayout.Space(5);

        CreateTableAddButton();

        serializedObject.ApplyModifiedProperties();
    }
}