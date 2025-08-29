using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolConditionalHideAttribute))]
public class BoolConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var condHAtt = (BoolConditionalHideAttribute)attribute;
        var enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool GetConditionalHideAttributeResult(BoolConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
        var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        return sourcePropertyValue.boolValue;
    }
}

[CustomPropertyDrawer(typeof(EnumConditionalHideAttribute))]
public class EnumConditionalPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldDisplay(property))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldDisplay(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool ShouldDisplay(SerializedProperty property)
    {
        EnumConditionalHideAttribute conditionalAttribute = (EnumConditionalHideAttribute)attribute;

        // 获取属性路径
        string propertyPath = property.propertyPath;

        // 找到最近的父对象路径
        string parentPath = propertyPath;
        int lastDotIndex = propertyPath.LastIndexOf('.');
        if (lastDotIndex > 0)
        {
            parentPath = propertyPath.Substring(0, lastDotIndex);
        }

        // 构建枚举属性的完整路径
        string enumPath = string.IsNullOrEmpty(parentPath) ?
            conditionalAttribute.enumFieldName :
            $"{parentPath}.{conditionalAttribute.enumFieldName}";

        SerializedProperty enumProperty = property.serializedObject.FindProperty(enumPath);

        if (enumProperty != null && enumProperty.propertyType == SerializedPropertyType.Enum)
        {
            // 检查当前枚举值是否在允许的值列表中
            foreach (int value in conditionalAttribute.enumValues)
            {
                if (enumProperty.enumValueIndex == value)
                {
                    return true;
                }
            }
        }

        return false;
    }
}