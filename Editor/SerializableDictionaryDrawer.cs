#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace VodVas.InterfaceSerializer
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 25f;
        private const float Spacing = 2f;
        private SerializedProperty _keysProp, _valuesProp;
        private SerializedProperty _property; // Сохраняем ссылку на property
        private string _searchQuery = "";
        private List<int> _filteredIndices = new List<int>();
        private bool _needsRefresh = true;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            _property = property; // Сохраняем ссылку на property
            _keysProp = property.FindPropertyRelative("_keys");
            _valuesProp = property.FindPropertyRelative("_values");

            EditorGUI.BeginProperty(rect, label, property);
            rect.height = EditorGUIUtility.singleLineHeight;

            // Header
            EditorGUI.LabelField(rect, label);
            rect.y += rect.height + Spacing;

            // Search
            DrawSearchField(ref rect);

            // Sort Buttons
            DrawSortButtons(ref rect);

            // Elements
            DrawDictionaryElements(ref rect);

            // Add Button
            DrawAddButton(ref rect);

            EditorGUI.EndProperty();

            // Применяем изменения
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawSearchField(ref Rect rect)
        {
            var newSearch = EditorGUI.TextField(rect, "Search", _searchQuery);
            if (newSearch != _searchQuery)
            {
                _searchQuery = newSearch;
                _needsRefresh = true;
            }
            rect.y += rect.height + Spacing;
        }

        private void DrawSortButtons(ref Rect rect)
        {
            var width = rect.width / 2 - 5;
            if (GUI.Button(new Rect(rect.x, rect.y, width, rect.height), "Sort by Key"))
                Sort(true);
            if (GUI.Button(new Rect(rect.x + width + 10, rect.y, width, rect.height), "Sort by Value"))
                Sort(false);
            rect.y += rect.height + Spacing;
        }

        private void Sort(bool byKey)
        {
            var count = _keysProp.arraySize;
            var indices = new int[count];
            var keys = new object[count];
            var values = new object[count];

            for (int i = 0; i < count; i++)
            {
                indices[i] = i;
                keys[i] = GetPropertyValue(_keysProp.GetArrayElementAtIndex(i));
                values[i] = GetPropertyValue(_valuesProp.GetArrayElementAtIndex(i));
            }

            System.Array.Sort(indices, (a, b) =>
                (byKey ? keys[a] : values[a]).ToString().CompareTo((byKey ? keys[b] : values[b]).ToString()));

            ApplySortedIndices(indices);
            _needsRefresh = true;
        }

        private void ApplySortedIndices(int[] indices)
        {
            var tempKeys = new List<object>();
            var tempValues = new List<object>();

            foreach (var index in indices)
            {
                tempKeys.Add(GetPropertyValue(_keysProp.GetArrayElementAtIndex(index)));
                tempValues.Add(GetPropertyValue(_valuesProp.GetArrayElementAtIndex(index)));
            }

            _keysProp.ClearArray();
            _valuesProp.ClearArray();

            for (int i = 0; i < indices.Length; i++)
            {
                _keysProp.arraySize++;
                SetPropertyValue(_keysProp.GetArrayElementAtIndex(i), tempKeys[i]);
                _valuesProp.arraySize++;
                SetPropertyValue(_valuesProp.GetArrayElementAtIndex(i), tempValues[i]);
            }
        }

        private void DrawDictionaryElements(ref Rect rect)
        {
            if (_needsRefresh)
            {
                _filteredIndices.Clear();
                for (int i = 0; i < _keysProp.arraySize; i++)
                {
                    // Если поиск пустой, показываем все элементы
                    if (string.IsNullOrEmpty(_searchQuery))
                    {
                        _filteredIndices.Add(i);
                        continue;
                    }

                    var key = GetPropertyValueAsString(_keysProp.GetArrayElementAtIndex(i));
                    var value = GetPropertyValueAsString(_valuesProp.GetArrayElementAtIndex(i));

                    if (key.ToLower().Contains(_searchQuery.ToLower()) ||
                        value.ToLower().Contains(_searchQuery.ToLower()))
                    {
                        _filteredIndices.Add(i);
                    }
                }
                _needsRefresh = false;
            }

            var lineHeight = EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < _filteredIndices.Count; i++)
            {
                var elementRect = new Rect(
                    rect.x,
                    rect.y + i * (lineHeight + Spacing),
                    rect.width,
                    lineHeight
                );

                DrawElement(elementRect, _filteredIndices[i]);
            }
            rect.y += _filteredIndices.Count * (lineHeight + Spacing);
        }

        private void DrawElement(Rect rect, int index)
        {
            var key = _keysProp.GetArrayElementAtIndex(index);
            var value = _valuesProp.GetArrayElementAtIndex(index);

            var keyWidth = rect.width * 0.45f;
            var valueWidth = rect.width * 0.45f - ButtonWidth;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, keyWidth, rect.height),
                key, GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(rect.x + keyWidth + 5, rect.y, valueWidth, rect.height),
                value, GUIContent.none
            );

            if (GUI.Button(new Rect(
                rect.x + keyWidth + valueWidth + 10, rect.y,
                ButtonWidth, rect.height), "-"))
            {
                _keysProp.DeleteArrayElementAtIndex(index);
                _valuesProp.DeleteArrayElementAtIndex(index);
                _needsRefresh = true;
            }
        }

        private void DrawAddButton(ref Rect rect)
        {
            if (GUI.Button(rect, "+ Add"))
            {
                // Обновляем объект, чтобы убедиться, что все данные актуальны
                _property.serializedObject.Update();

                // Получаем индекс нового элемента
                int newIndex = _keysProp.arraySize;

                // Увеличиваем размер массивов
                _keysProp.arraySize++;
                _valuesProp.arraySize++;

                // Получаем ссылки на новые элементы
                SerializedProperty newKey = _keysProp.GetArrayElementAtIndex(newIndex);
                SerializedProperty newValue = _valuesProp.GetArrayElementAtIndex(newIndex);

                // Инициализируем новый ключ и значение с уникальными идентификаторами
                InitializeUniqueProperty(newKey, "Key_" + DateTime.Now.Ticks + "_" + newIndex);
                InitializeUniqueProperty(newValue, "Value_" + newIndex);

                // Применяем изменения и помечаем объект как измененный
                _property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_property.serializedObject.targetObject);

                // Сбрасываем поиск, чтобы новый элемент был виден
                _searchQuery = "";
                _needsRefresh = true;
            }
        }

        private void InitializeUniqueProperty(SerializedProperty prop, string defaultValue)
        {
            // Инициализируем в зависимости от типа свойства
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    // Для int ключей генерируем уникальное значение, зависящее от времени
                    if (defaultValue.StartsWith("Key_"))
                    {
                        prop.intValue = (int)(DateTime.Now.Ticks % int.MaxValue);
                    }
                    else
                    {
                        int.TryParse(defaultValue.Replace("Value_", ""), out int val);
                        prop.intValue = val;
                    }
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = defaultValue;
                    break;
                case SerializedPropertyType.Float:
                    // Для float генерируем уникальное значение
                    if (defaultValue.StartsWith("Key_"))
                    {
                        prop.floatValue = (float)(DateTime.Now.Ticks % int.MaxValue) / 1000f;
                    }
                    else
                    {
                        float.TryParse(defaultValue.Replace("Value_", ""), out float val);
                        prop.floatValue = val;
                    }
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = defaultValue.StartsWith("Key_");
                    break;
                case SerializedPropertyType.ObjectReference:
                    // Для Unity Object ссылок мы не можем установить значение
                    break;
                    // Здесь можно добавить обработку других типов по необходимости
            }
        }

        private object GetPropertyValue(SerializedProperty prop)
        {
            return prop.propertyType switch
            {
                SerializedPropertyType.Integer => prop.intValue,
                SerializedPropertyType.Boolean => prop.boolValue,
                SerializedPropertyType.Float => prop.floatValue,
                SerializedPropertyType.String => prop.stringValue,
                SerializedPropertyType.ObjectReference => prop.objectReferenceValue,
                _ => null
            };
        }

        private string GetPropertyValueAsString(SerializedProperty prop)
        {
            var value = GetPropertyValue(prop);
            return value?.ToString() ?? "null";
        }

        private void SetPropertyValue(SerializedProperty prop, object value)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = value is int i ? i : 0;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = value is bool b && b;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = value is float f ? f : 0f;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = value as string ?? "";
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = value as UnityEngine.Object;
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 +
                   (_filteredIndices.Count * (EditorGUIUtility.singleLineHeight + Spacing));
        }
    }
}
#endif
