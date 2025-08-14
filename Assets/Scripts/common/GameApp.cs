using System.Collections;
using System.Collections.Generic;
using FactorySystem;
using UnityEngine;

public class GameApp : Singleton<GameApp>
{
    public static BGMManager BgmManager;
    public static ControllerManager ControllerManager;
    public static ViewManager viewManager;
    public static ConfigManager ConfigManager;
    public static CameraManager CameraManager;
    public static MessageCenter MessageCenter;
    public static TimerManager TimerManager;
    public static UserInuptManager UserInuptManager;
    public static GameGirdManager GameGridManager;
    public static EconomyManager EcomoneyManager;  
    public static MachineManager MachineManager;
    public static ItemManager ItemManager;
    public static HealthManager HealthManager;
    public static PollutionManager PollutionManager;

    public override void Awake(List<System.Object> objects, List<UnityEngine.Object> gameObjects)
    {
        ConfigManager = new ConfigManager();
        ConfigManager.LoadGameMainConfigs();
        // add game config table to the func init
        Init(objects, gameObjects);
    }

    public override void Init(List<System.Object> normalObjects = null, List<UnityEngine.Object> unityObject = null)
    {
        if (normalObjects != null && unityObject != null)
        {
            GameGridManager = new GameGirdManager((GameObject)unityObject[0], (Material)unityObject[1]);
            MachineManager = new MachineManager((MachineInfo[])normalObjects[0]);
            ItemManager = new ItemManager((ItemInfo[])normalObjects[1]);
            BgmManager = new BGMManager();
            HealthManager = new HealthManager((GameObject)unityObject[2]);
            EcomoneyManager = new EconomyManager((float)normalObjects[2]);
            PollutionManager = new PollutionManager((float)normalObjects[3], (float)normalObjects[4], (float)normalObjects[5]);
            ControllerManager = new ControllerManager();
            viewManager = new ViewManager();
            CameraManager = new CameraManager();
            MessageCenter = new MessageCenter();
            TimerManager = new TimerManager();
            UserInuptManager = new UserInuptManager();
        }
    }

    public override void Update(float dt)
    {
        UserInuptManager.Update();
        TimerManager.Update(dt);
        GameGridManager.Update();
        MachineManager.Update();
    }
}
