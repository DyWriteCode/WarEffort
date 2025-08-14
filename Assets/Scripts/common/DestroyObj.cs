using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    public float timer;

    public void DestroyObject(float timer = 0f)
    {
        Destroy(gameObject, timer);
    }
}
