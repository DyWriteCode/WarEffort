using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySystem
{
    public class GameScene : MonoBehaviour
    {
        [Header("Game Settings")]
        public float InitialMoney = 10000;
        public Texture2D MouseIcon;
        public Material BeltMaterial;

        [Header("Game Object")]
        public GameObject HealthBarPrefab;

        [Header("Data Assets")]
        public MachineInfo[] MachinesInfo;
        public ItemInfo[] ItemsInfo;

        [Header("Pollution System")]
        public float GlobalPollution = 0;
        public float MaxPollution = 0;
        public float PollutionWarningThreshold = 0;

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
                GameApp.Instance.Awake(new List<System.Object>() 
                {
                    this.MachinesInfo,
                    this.ItemsInfo,
                    this.InitialMoney,
                    GlobalPollution,
                    MaxPollution,
                    PollutionWarningThreshold
                }, 
                new List<UnityEngine.Object>() 
                { 
                    gameObject, 
                    BeltMaterial,
                    HealthBarPrefab 
                });
            }
        }
        void Start()
        {
            Cursor.SetCursor(MouseIcon, Vector2.zero, CursorMode.Auto);
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
}