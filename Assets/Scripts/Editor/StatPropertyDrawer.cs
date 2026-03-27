using UnityEngine.UIElements;
using UnityEditor;

[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer
{
    public VisualTreeAsset visualTreeAsset;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = visualTreeAsset.CloneTree();
        root.Q<Foldout>("RootFoldout").text = property.displayName;
        return root;
    }
}