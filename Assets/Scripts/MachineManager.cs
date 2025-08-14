using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactorySystem.Machines;

namespace FactorySystem
{
    /// <summary>
    /// 机器管理器 - 负责所有机器的创建、管理和操作
    /// </summary>
    public class MachineManager : BaseManager
    {
        #region Data structure
        // 机器ID到机器实例的映射
        private readonly Dictionary<Vector3, Machine> _machines = new Dictionary<Vector3, Machine>();

        // 网格位置到机器ID的映射
        private readonly Dictionary<Vector3, Vector3> _gridOccupancy = new Dictionary<Vector3, Vector3>();

        // 机器信息缓存
        private Dictionary<Machine.Type, MachineInfo> _machineInfoCache = new Dictionary<Machine.Type, MachineInfo>();

        public enum Position { Top = 1, Left = 2, Bottom = 3, Right = 4 }

        private MachineFactory _machineFactory;
        #endregion

        #region Init
        /// <summary>
        /// 初始化机器管理器
        /// </summary>
        /// <param name="machinesInfo">所有机器配置信息</param>
        public MachineManager(MachineInfo[] machinesInfo)
        {
            // 处理机器信息
            ProcessMachineInfo(machinesInfo);
            _machineFactory = new MachineFactory();
        }

        /// <summary>
        /// 处理机器信息，准备运行时使用
        /// </summary>
        private void ProcessMachineInfo(MachineInfo[] machinesInfo)
        {
            foreach (MachineInfo machine in machinesInfo)
            {
                // 转换存储限制为字典格式
                machine.storageLimits = new Dictionary<Item.Type, int>();
                foreach (ItemUI item in machine._storageLimits)
                {
                    machine.storageLimits.Add(item.type, item.quantity);
                }
                machine._storageLimits = null; // 释放原始数据

                // 添加到缓存
                _machineInfoCache[machine.type] = machine;
            }
        }
        #endregion

        #region GetMachineInfo
        /// <summary>
        /// 获取机器信息
        /// </summary>
        /// <param name="type">机器类型</param>
        /// <returns>机器信息实例</returns>
        public MachineInfo GetMachineInfo(Machine.Type type)
        {
            if (_machineInfoCache.TryGetValue(type, out MachineInfo info))
            {
                return info;
            }

            throw new System.Exception($"未找到机器类型: {type}");
        }

        /// <summary>
        /// 获取所有机器信息
        /// </summary>
        public IEnumerable<MachineInfo> GetAllMachineInfo()
        {
            return _machineInfoCache.Values;
        }
        #endregion

        #region Machine lifecycle management
        /// <summary>
        /// 创建并放置机器
        /// </summary>
        /// <param name="gameObject">机器游戏对象</param>
        /// <param name="type">机器类型</param>
        /// <param name="position">放置位置</param>
        /// <param name="direction">机器方向</param>
        /// <returns>创建的机器实例</returns>
        public Machine CreateAndPlaceMachine(Machine.Type type, Vector3 position, Machine.Direction direction = Machine.Direction.Right)
        {
            if (IsPositionOccupied(position, type))
            {
                Debug.LogWarning($"位置已被占用: {position}");
                return null;
            }

            // 使用工厂创建 GameObject
            Quaternion rotation = Quaternion.Euler(0, (float)direction, 0);
            GameObject machineGo = _machineFactory.CreateMachineGameObject(type, position, rotation);

            if (machineGo == null) return null;

            Machine machine = CreateMachineInstance(machineGo, type, position, direction);
            PlaceMachine(machine);
            return machine;
        }

        /// <summary>
        /// 创建机器实例
        /// </summary>
        private Machine CreateMachineInstance(GameObject gameObject, Machine.Type type, Vector3 position, Machine.Direction direction)
        {
            return type switch
            {
                Machine.Type.AttackMachine => new AttackMachine(gameObject),
                Machine.Type.CleanerMachine => new CleanerMachine(gameObject),
                Machine.Type.Belt => new Belt(gameObject) { direction = direction },
                _ => new Machine(gameObject, type, position)
            };
        }

        /// <summary>
        /// 放置机器到网格
        /// </summary>
        public void PlaceMachine(Machine machine)
        {
            if (IsPositionOccupied(machine.position) == false)
            {
                Debug.LogWarning($"位置已被占用: {machine.position}");
            }

            _machines[machine.position] = machine;

            // 标记占用的网格位置
            foreach (Vector3 blockPos in GetMachineBlocks(machine.position, machine.type))
            {
                _gridOccupancy[blockPos] = machine.position;
            }

            if (GameApp.EcomoneyManager.CanAfford(machine.info.price)) 
            {
                GameApp.EcomoneyManager.SpendMoney(machine.info.price);
            }
        }

        /// <summary>
        /// 删除机器
        /// </summary>
        /// <param name="position">机器位置</param>
        public void DeleteMachine(Vector3 position)
        {
            if (!_machines.TryGetValue(position, out Machine machine)) return;

            // 清理所有连接
            CleanupMachineConnections(machine);

            // 清理网格占用
            ClearGridOccupancy(machine);

            // 销毁游戏对象
            if (machine.gameObject != null)
            {
                machine.gameObject.AddComponent<DestroyObj>().DestroyObject(0);
            }

            // 从管理器移除
            _machines.Remove(position);
        }

        /// <summary>
        /// 清理机器所有连接
        /// </summary>
        private void CleanupMachineConnections(Machine machine)
        {
            foreach (Vector3 neighbor in machine.connections.Keys.ToList())
            {
                if (!_machines.TryGetValue(neighbor, out Machine neighborMachine)) continue;

                if (neighborMachine.type == Machine.Type.Belt)
                {
                    ((Belt)neighborMachine).RemoveConnection(machine.position, machine.type);
                }
                else
                {
                    neighborMachine.RemoveConnection(machine.position, machine.type);
                }
            }
        }

        /// <summary>
        /// 清理网格占用
        /// </summary>
        private void ClearGridOccupancy(Machine machine)
        {
            foreach (Vector3 pos in GetMachineBlocks(machine.position, machine.type))
            {
                _gridOccupancy.Remove(pos);
            }
        }

        /// <summary>
        /// 清空所有机器
        /// </summary>
        public void ClearAllMachines()
        {
            // 创建副本避免修改集合
            var machinesCopy = new List<Machine>(_machines.Values);

            // 删除所有机器
            foreach (Machine machine in machinesCopy)
            {
                DeleteMachine(machine.position);
            }

            // 重置核心数据结构
            _machines.Clear();
            _gridOccupancy.Clear();
        }

        public override void Update()
        {
            ExecuteAllMachines();
        }
        #endregion

        #region Machine query operations
        /// <summary>
        /// 获取指定位置的机器
        /// </summary>
        public Machine GetMachineAt(Vector3 position)
        {
            return _machines.TryGetValue(position, out Machine machine) ? machine : null;
        }

        /// <summary>
        /// 获取所有机器
        /// </summary>
        public IEnumerable<Machine> GetAllMachines()
        {
            return _machines.Values;
        }

        /// <summary>
        /// 检查位置是否被占用
        /// </summary>
        public bool IsPositionOccupied(Vector3 position, Machine.Type type)
        {
            foreach (Vector3 blockPos in GetMachineBlocks(position, type))
            {
                if (_gridOccupancy.ContainsKey(blockPos))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPositionOccupied(Vector3 position)
        {
            return !_gridOccupancy.ContainsKey(position);
        }

        /// <summary>
        /// 获取机器占用的所有网格块
        /// </summary>
        public Vector3[] GetMachineBlocks(Vector3 gridPosition, Machine.Type type)
        {
            MachineInfo machineInfo = GetMachineInfo(type);
            List<Vector3> blocks = new List<Vector3>();
            Vector3 currentBlock = gridPosition;

            for (int x = 0; x < machineInfo.x; x++)
            {
                for (int y = 0; y < machineInfo.y; y++)
                {
                    blocks.Add(currentBlock);
                    currentBlock.z++;
                }
                currentBlock.x++;
                currentBlock.z = gridPosition.z;
            }

            return blocks.ToArray();
        }

        /// <summary>
        /// 获取指定位置的所有邻居
        /// </summary>
        public List<Vector3> GetNeighbors(Vector3 position)
        {
            return new List<Vector3>
            {
                position + Vector3.left,
                position + Vector3.right,
                position + Vector3.back, // Unity 中 back 是 z 负方向
                position + Vector3.forward
            };
        }

        /// <summary>
        /// 获取邻居相对于当前位置的方向
        /// </summary>
        public Position GetNeighborPosition(Vector3 position, Vector3 neighbor)
        {
            if (neighbor.z == position.z)
            {
                return (neighbor.x > position.x) ? Position.Right : Position.Left;
            }
            return (neighbor.z > position.z) ? Position.Top : Position.Bottom;
        }
        #endregion

        #region Machine connection management
        /// <summary>
        /// 添加机器连接
        /// </summary>
        public void AddConnection(Machine fromMachine, Machine toMachine, Machine.ConnectionType connectionType)
        {
            fromMachine.AddConnection(toMachine.position, connectionType);
            toMachine.AddConnection(fromMachine.position,
                connectionType == Machine.ConnectionType.Input
                    ? Machine.ConnectionType.Output
                    : Machine.ConnectionType.Input);
        }

        /// <summary>
        /// 移除机器连接
        /// </summary>
        public void RemoveConnection(Machine fromMachine, Machine toMachine)
        {
            fromMachine.RemoveConnection(toMachine.position, toMachine.type);
            toMachine.RemoveConnection(fromMachine.position, fromMachine.type);
        }
        #endregion

        #region Machine running management
        /// <summary>
        /// 执行所有机器的逻辑
        /// </summary>
        public void ExecuteAllMachines()
        {
            foreach (Machine machine in _machines.Values)
            {
                if (machine != null && machine.ShouldExecute())
                {
                    machine.Run();
                }
            }
        }
        #endregion
    }
}