using UnityEngine;
using System;

public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
    }
}