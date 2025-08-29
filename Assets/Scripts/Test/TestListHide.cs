using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListHide : MonoBehaviour
{
    public TestCondition[] conditions;
    public bool test = true;
    [BoolConditionalHide(nameof(test))]
    public int testint = 0;
}
