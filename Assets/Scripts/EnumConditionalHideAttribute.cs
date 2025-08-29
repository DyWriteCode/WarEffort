using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class EnumConditionalHideAttribute : PropertyAttribute
{
    public string enumFieldName;
    public int[] enumValues;

    public EnumConditionalHideAttribute(string enumFieldName, params object[] enumValues)
    {
        this.enumFieldName = enumFieldName;

        // 将枚举值转换为整数
        this.enumValues = new int[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            this.enumValues[i] = (int)enumValues[i];
        }
    }
}