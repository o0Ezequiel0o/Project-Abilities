using UnityEngine.UIElements;
using UnityEditor;

[CustomPropertyDrawer(typeof(Limits))]
public class LimitsPropertyDrawer : PropertyDrawer
{
    public VisualTreeAsset visualTreeAsset;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = visualTreeAsset.CloneTree();
        string propertyLabel = property.displayName;
        root.Q<Foldout>("RootFoldout").text = propertyLabel;
        return root;
    }
}