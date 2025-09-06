using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class EnumConditionalHideAttribute : PropertyAttribute
{
    public string enumFieldName;
    public int[] enumValues;
    public bool HideEntirely { get; set; } = true; // 新增：控制是完全隐藏还是折叠

    public EnumConditionalHideAttribute(string enumFieldName, params object[] enumValues)
    {
        this.enumFieldName = enumFieldName;
        this.enumValues = new int[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            this.enumValues[i] = (int)enumValues[i];
        }
        HideEntirely = true;
    }

    public EnumConditionalHideAttribute(string enumFieldName, bool hideEntirely, params object[] enumValues)
    {
        this.enumFieldName = enumFieldName;
        this.enumValues = new int[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            this.enumValues[i] = (int)enumValues[i];
        }
        HideEntirely = hideEntirely;
    }
}