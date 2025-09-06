using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class BoolConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField { get; private set; }
    public bool HideEntirely { get; set; } = true; 

    public BoolConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
        HideEntirely = true;
    }

    public BoolConditionalHideAttribute(string conditionalSourceField, bool hideEntirely)
    {
        ConditionalSourceField = conditionalSourceField;
        HideEntirely = hideEntirely;
    }
}