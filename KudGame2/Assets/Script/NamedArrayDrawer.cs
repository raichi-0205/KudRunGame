using UnityEngine;
using UnityEditor;
using System.Linq;

#if UNITY_EDITOR
namespace Kud.Editor
{
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class NamedArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            string[] names = ((NamedArrayAttribute)attribute).names;
            int pos = int.Parse(property.propertyPath.Split('[', ']').Where(c => !string.IsNullOrEmpty(c)).Last());
            if (pos < names.Length)
            {
                label.text = names[pos];
            }
            EditorGUI.PropertyField(rect, property, new GUIContent(names[pos]), includeChildren: true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }
    }
}
#endif