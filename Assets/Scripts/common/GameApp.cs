using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameApp : Singleton<GameApp>
{
    public static BGMManage bgmManage;
    public static ControllerManager ControllerManager;
    public static ViewManager viewManager;
    public static ConfigManager ConfigManager;
    public static CameraManager CameraManager;
    public static MessageCenter MessageCenter;
    public static TimerManager TimerManager;
    public static GameDataManager GameDataManager; 
    public static UserInuptManager UserInuptManager;

    public override void init()
    {
        bgmManage = new BGMManage();
        ControllerManager = new ControllerManager();
        viewManager = new ViewManager();
        ConfigManager = new ConfigManager();
        CameraManager = new CameraManager();
        MessageCenter = new MessageCenter();
        TimerManager = new TimerManager();
        GameDataManager=new GameDataManager();
        UserInuptManager=new UserInuptManager();
    }

    public override void Update(float dt)
    {
        UserInuptManager.Update();
        TimerManager.OnUpdate(dt);
    }
}
