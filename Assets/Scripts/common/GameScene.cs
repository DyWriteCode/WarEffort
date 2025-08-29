using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySystem
{
    public class GameScene : MonoBehaviour
    {
        public bool NoUseConfigurationTable = true;

        [Header("Game Settings")]
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public float InitialMoney = 10000;
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public Texture2D MouseIcon;
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public Material BeltMaterial;

        [Header("Game Object")]
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public GameObject HealthBarPrefab;

        [Header("Data Assets")]
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public MachineInfo[] MachinesInfo;
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public ItemInfo[] ItemsInfo;

        [Header("Pollution System")]
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public float GlobalPollution = 0;
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
        public float MaxPollution = 0;
        [BoolConditionalHide(nameof(NoUseConfigurationTable))]
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
            Cursor.SetCursor(MouseIcon, Vector3.zero, CursorMode.Auto);
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