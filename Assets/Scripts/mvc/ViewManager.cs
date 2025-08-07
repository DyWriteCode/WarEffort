using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class ViewInfo
{
    public string PrefabName;
    public Transform parentTf;
    public BaseController controller;
    public int Sorting_Order;
}
public class ViewManager
{
    public Transform canvasTf;
    public Transform worldCanvasTf;

    Dictionary<int, IBaseView> _opens;
    Dictionary<int, IBaseView> _viewCache;
    Dictionary<int, ViewInfo> _views;

    public ViewManager()
    {
        canvasTf = GameObject.Find("Canvas").transform;
        worldCanvasTf = GameObject.Find("WorldCanvas").transform;

        _opens = new Dictionary<int, IBaseView>();
        _viewCache = new Dictionary<int, IBaseView>();
        _views = new Dictionary<int, ViewInfo>();
    }

    public void Register(int key,ViewInfo viewInfo)
    {
        if (!_views.ContainsKey(key))
        {
            _views.Add(key, viewInfo);
        }
    }

    public void Register(ViewType viewType,ViewInfo viewInfo)
    {
        Register((int)viewType, viewInfo);
    }

    public void UnRegister(int key)
    {
        if (_views.ContainsKey(key))
        {
            _views.Remove(key);
        }
    }

    public void RemoveView(int key)
    {
        _views.Remove(key);
        _viewCache.Remove(key);
        _opens.Remove(key);
    }

    public void  RemoveViewController(BaseController controller)
    {
        foreach(var item in _views)
        {
            if (item.Value.controller == controller)
            {
                RemoveView(item.Key);
            }
        }
    }

    public bool IsOpen(int key)
    {
        return _opens.ContainsKey(key);
    }

    public IBaseView GetView(int key)
    {
        if (_opens.ContainsKey(key))
        {
            return _opens[key];
        }
        if (_viewCache.ContainsKey(key))
        {
            return _viewCache[key];
        }
        return null;
    }

    public T GetView<T>(int key) where T : class,IBaseView
    {
        IBaseView view = GetView(key);
        if (view != null)
        {
            return view as T;
        }
        return null;
    }

    public void Destroy(int key)
    {
        IBaseView oldView = GetView(key);
        if (oldView != null)
        {
            UnRegister(key);
            oldView.DestroyView();
            _viewCache.Remove(key);
        }
    }

    public void Close(int key,params object[] args)
    {
        if (IsOpen(key) == false)
        {
            return;
        }
        IBaseView view = GetView(key);
        if (view != null)
        {
            _opens.Remove(key);
            view.Close(args);
            _views[key].controller.CloseView(view);
        }
    }

    public void CloseAll()
    {
        List<IBaseView> list = _opens.Values.ToList();
        Debug.Log(list.Count);
        for(int i=list.Count-1; i>=0; i--)
        {
            Close(list[i].ViewId);
        }
    }

    public void Open(int key,params object[] args)
    {
        IBaseView view = GetView(key);
        ViewInfo viewInfo = _views[key];

        Debug.Log(viewInfo.PrefabName);

        if (view == null)
        {
            string type = ((ViewType)key).ToString();
            GameObject uiObj = UnityEngine.Object.Instantiate(Resources.Load($"View/{viewInfo.PrefabName}"), viewInfo.parentTf) as GameObject;

            Canvas canvas = uiObj.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = uiObj.AddComponent<Canvas>();
            }
            if (!uiObj.GetComponent<GraphicRaycaster>())
            {
                uiObj.AddComponent<GraphicRaycaster>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = viewInfo.Sorting_Order;
            view = uiObj.AddComponent(Type.GetType(type)) as IBaseView;
            view.ViewId = key;
            view.Controller = viewInfo.controller;
            _viewCache.Add(key, view);
            viewInfo.controller.OnLoadView(view);
        }
        if (this._opens.ContainsKey(key))
        {
            return;
        }
        this._opens.Add(key, view);
        if (view.IsInit())
        {
            view.SetVisible(true);
            view.Open(args);
            viewInfo.controller.OpenView(view);
        }
        else
        {
            view.InitUI();
            view.InitData();
            view.Open(args);
            viewInfo.controller.OpenView(view);
        }
    }
    public void Open(ViewType type,params object[] args)
    {
        Open((int)type, args);
    }

    public void ShowHitNum(string num,Color color,Vector3 pos)
    {
        GameObject obj = UnityEngine.GameObject.Instantiate(Resources.Load("View/HitNum"),worldCanvasTf) as GameObject;
        obj.transform.position = pos;
        obj.transform.DOMove(pos + Vector3.up * 1.75f, 0.65f).SetEase(Ease.OutBack);
        UnityEngine.Object.Destroy(obj, 0.75f);

        Text hitTxt=obj.GetComponent<Text>();
        hitTxt.text = num;
        hitTxt.color = color;
    }
}
