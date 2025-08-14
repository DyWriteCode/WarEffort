using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Singleton<T>
{
    private static readonly T instance = Activator.CreateInstance<T>();

    public static T Instance
    {
        get {
            if (instance != null)
            {
                return instance;
            }
            return Activator.CreateInstance<T>();
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Init(List<UnityEngine.Object> objects = null)
    {

    }

    public virtual void Init(List<System.Object> objects = null)
    {

    }

    public virtual void Init(List<System.Object> normalObjects = null, List<UnityEngine.Object> unityObjects = null)
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Update(float dt)
    {

    }

    public virtual void OnDestroy()
    {

    }

    public virtual void Awake()
    {

    }

    public virtual void Awake(List<UnityEngine.Object> objects = null)
    {

    }

    public virtual void Awake(List<System.Object> objects = null)
    {

    }

    public virtual void Awake(List<System.Object> normalObjects = null, List<UnityEngine.Object> unityObjects = null)
    {

    }
}
