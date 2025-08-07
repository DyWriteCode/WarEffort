using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControllerManager 
{
    private Dictionary<int, BaseController> _modules;

    public ControllerManager()
    {
        _modules = new Dictionary<int, BaseController>();
    }

    public void Register(ControllerType type,BaseController controller)
    {
        Register((int)type, controller);
    }

    public void Register(int controllerKey, BaseController controller)
    {
        if (!_modules.ContainsKey(controllerKey)){
            _modules.Add(controllerKey, controller);
        }
    }

    public void InitAllModule()
    {
        foreach(var item in _modules)
        {
            item.Value.Init();
        }
    }

    public void UnRegister(int controllerKey)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            _modules.Remove(controllerKey);
        }
    }

    public void Clear()
    {
        _modules.Clear();
    }

    public void ClearAllModules()
    {
        List<int>keys=_modules.Keys.ToList();
        for(int i = 0; i < keys.Count; i++)
        {
            _modules[keys[i]].Destroy();
            _modules.Remove(keys[i]);
        }
    }

    public void ApplyFunc(int controllerKey, string eventName, System.Object[] args)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            _modules[controllerKey].ApplyFunc(eventName, args);
        }
    }

    public BaseModel GetControllerModel(int controllerKey)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            return _modules[controllerKey].GetModel();
        }
        else
        {
            return null;
        }
    }
}

