using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class EnumConditionalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 简单地绘制默认的序列化属性，让PropertyDrawer处理条件显示
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            // 跳过脚本字段
            if (iterator.name == "m_Script")
            {
                EditorGUILayout.PropertyField(iterator, true);
                continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}