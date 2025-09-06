using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolConditionalHideAttribute))]
public class BoolConditionalHidePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var condHAtt = (BoolConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!enabled && condHAtt.HideEntirely)
        {
            return -EditorGUIUtility.standardVerticalSpacing; // 完全隐藏
        }

        if (!enabled && property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            Debug.Log(nameof(property));

            // 对于数组/列表，即使条件不满足也要显示折叠标题
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var condHAtt = (BoolConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        
        if (!enabled)
        {
            GUI.enabled = false;
            if (condHAtt.HideEntirely)
            {
                // 完全隐藏
                return;
            }

            if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                Debug.Log(nameof(property));
                // 对于数组/列表，显示折叠标题但内容为空
                HandleArrayDisplay(position, property, label);
                GUI.enabled = true;
                return;
            }
        }

        if (!enabled)
        {
            GUI.enabled = false;
        }
        // 正常显示属性
        EditorGUI.PropertyField(position, property, label, true);
        if (!enabled)
        {
            GUI.enabled = false;
        }
    }

    private void HandleArrayDisplay(Rect position, SerializedProperty property, GUIContent label)
    {
        // 只显示折叠标题，不显示内容
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label,
            true
        );

        // 即使展开也不显示数组内容
        if (property.isExpanded)
        {
            property.isExpanded = false; // 强制保持折叠状态
        }
    }

    private bool GetConditionalHideAttributeResult(BoolConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        try
        {
            string propertyPath = property.propertyPath;
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
            var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null && sourcePropertyValue.propertyType == SerializedPropertyType.Boolean)
            {
                return sourcePropertyValue.boolValue;
            }

            return true; // 如果找不到条件字段，默认显示
        }
        catch
        {
            return true; // 出错时默认显示
        }
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
            var condHAtt = (EnumConditionalHideAttribute)attribute;

            if (!condHAtt.HideEntirely)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }

            if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                // 对于数组/列表，即使条件不满足也要显示折叠标题
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldDisplay(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            var condHAtt = (EnumConditionalHideAttribute)attribute;

            if (!condHAtt.HideEntirely && property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                // 对于数组/列表，显示折叠标题但内容为空
                HandleArrayDisplay(position, property, label);
            }
            // 其他情况完全隐藏
        }
    }

    private void HandleArrayDisplay(Rect position, SerializedProperty property, GUIContent label)
    {
        // 只显示折叠标题，不显示内容
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label,
            true
        );

        // 即使展开也不显示数组内容
        if (property.isExpanded)
        {
            property.isExpanded = false; // 强制保持折叠状态
        }
    }

    private bool ShouldDisplay(SerializedProperty property)
    {
        EnumConditionalHideAttribute conditionalAttribute = (EnumConditionalHideAttribute)attribute;

        try
        {
            string propertyPath = property.propertyPath;
            string parentPath = propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                parentPath = propertyPath.Substring(0, lastDotIndex);
            }

            string enumPath = string.IsNullOrEmpty(parentPath) ?
                conditionalAttribute.enumFieldName :
                $"{parentPath}.{conditionalAttribute.enumFieldName}";

            SerializedProperty enumProperty = property.serializedObject.FindProperty(enumPath);

            if (enumProperty != null && enumProperty.propertyType == SerializedPropertyType.Enum)
            {
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
        catch
        {
            return true; // 出错时默认显示
        }
    }
}