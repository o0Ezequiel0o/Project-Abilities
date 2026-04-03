using UnityEngine.UIElements;
using UnityEditor;

namespace Zeke.Abilities.Indicators
{
    [CustomPropertyDrawer(typeof(AbilityIndicatorModule))]
    public class AbilityIndicatorModulePropertyDrawer : PropertyDrawer
    {
        public VisualTreeAsset visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = visualTreeAsset.CloneTree();
            return root;
        }
    }
}