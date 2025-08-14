using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseView 
{
    bool IsInit();

    bool IsShow();

    void InitUI();

    void InitData();

    void Open(params object[] args);

    void Close(params object[] args);

    void DestroyView();

    void ApplyFunc(string eventName, params object[] args);

    void ApplyControllerFunc(int controllerKey, string eventName, params object[] args);

    void SetVisible(bool value);

    int ViewId { get; set; }

    BaseController Controller { get; set; }

}
