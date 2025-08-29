using UnityEngine;
using System;

public class BoolConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;

    public BoolConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
    }
}