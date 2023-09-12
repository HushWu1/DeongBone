using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

    [CustomPropertyDrawer(typeof(TypeSelector))]
    public class TypeSelectorInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty baseTypeNameProp = property.FindPropertyRelative("baseTypeName");
            SerializedProperty selectedTypeProp = property.FindPropertyRelative("selectedType");

            Type baseType = Utility.Types.GAME_CSHARP_ASSEMBLY.GetType(baseTypeNameProp.stringValue);
            if (baseType == null)
            {
                EditorGUI.HelpBox(position, $"找不到所需的类型:{baseTypeNameProp.stringValue}", UnityEditor.MessageType.Warning);
            }
            else
            {
                List<GUIContent> allTypes = Utility.Types.GetAllSubclasses(baseType, false, Utility.Types.GAME_CSHARP_ASSEMBLY).ConvertAll((t) => { return new GUIContent(t.FullName); });
                allTypes.Insert(0, new GUIContent("<Null>"));
                int selectIndex = allTypes.FindIndex((t) => { return t.text == selectedTypeProp.stringValue; });
                selectIndex = Mathf.Max(selectIndex, 0);
                selectIndex = EditorGUI.Popup(position, label, selectIndex, allTypes.ToArray());
                selectedTypeProp.stringValue = allTypes[selectIndex].text;
            }

            EditorGUI.EndProperty();
        }
    }
