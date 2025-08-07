using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public Texture2D mouseIcon;
    private float dt;

    private static bool isLoaded = false;

    private void Awake()
    {
        if (isLoaded == true)
        {
            Destroy(gameObject);
        }
        else
        {
            isLoaded = true;
            DontDestroyOnLoad(gameObject);
            GameApp.Instance.init();

        }
    }
    void Start()
    {
        Cursor.SetCursor(mouseIcon, Vector2.zero, CursorMode.Auto);

        RegisterConfigs();

        GameApp.ConfigManager.LoadAllConfigs();

        RegisterModule();

        InitModule();
    }
    void RegisterModule()
    {
    }

    void InitModule()
    {
        GameApp.ControllerManager.InitAllModule();
    }
    void RegisterConfigs()
    {
    }
    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
