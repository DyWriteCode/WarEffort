using UnityEngine;

namespace FactorySystem
{
    /// <summary>
    /// 机器工厂 - 负责机器 GameObject 的创建和配置
    /// </summary>
    public class MachineFactory
    {
        private Transform _machineParent; // 所有机器的父对象

        public MachineFactory()
        {
            // 创建并配置机器父对象
            var parentGo = new GameObject("Machines");
            _machineParent = parentGo.transform;
        }

        /// <summary>
        /// 创建机器 GameObject
        /// </summary>
        public GameObject CreateMachineGameObject(Machine.Type type, Vector3 position, Quaternion rotation)
        {
            // 获取机器预制体
            MachineInfo info = GameApp.MachineManager.GetMachineInfo(type);
            if (info.prefab == null)
            {
                Debug.LogError($"找不到机器类型 {type} 的预制体");
                return null;
            }

            // 实例化 GameObject
            GameObject machineGo = GameObject.Instantiate(
                info.prefab,
                position,
                rotation,
                _machineParent
            );

            // 配置基本属性
            machineGo.name = $"{type}_{position}";
            machineGo.transform.position = position;

            return machineGo;
        }

        /// <summary>
        /// 销毁机器 GameObject
        /// </summary>
        public void DestroyMachineGameObject(GameObject machineGo)
        {
            if (machineGo != null)
            {
                GameObject.Destroy(machineGo);
            }
        }
    }
}