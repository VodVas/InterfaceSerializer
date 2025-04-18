#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

namespace VodVas.InterfaceSerializer
{

    [CustomPropertyDrawer(typeof(InterfaceConstraintAttribute))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var constraint = (InterfaceConstraintAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);

            MonoBehaviour current = property.objectReferenceValue as MonoBehaviour;
            MonoBehaviour selected = EditorGUI.ObjectField(
                position,
                label,
                current,
                typeof(MonoBehaviour),
                true) as MonoBehaviour;

            ValidateSelection(selected, constraint.InterfaceType, property);

            EditorGUI.EndProperty();
        }

        private void ValidateSelection(MonoBehaviour selected, Type interfaceType, SerializedProperty property)
        {
            if (selected == null)
            {
                property.objectReferenceValue = null;
                return;
            }

            bool isValid = interfaceType.IsAssignableFrom(selected.GetType());

            if (!isValid)
            {
                Debug.LogError($"{selected.name} must implement {interfaceType.Name} interface!");
                property.objectReferenceValue = null;
                return;
            }

            property.objectReferenceValue = selected;
        }
    }
}
#endif