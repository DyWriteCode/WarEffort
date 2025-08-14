#region v1 code
// v1
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace FactorySystem { 
//    public class GameGirdManager : MonoBehaviour
//    {
//        public enum Position
//        {
//            Top = 1,
//            Left = 2,
//            Bottom = 3,
//            Right = 4,
//        }

//        public static GameGirdManager Instance;
//        public Dictionary<Vector3, Machine> machines = new Dictionary<Vector3, Machine>();
//        public Dictionary<Vector3, Vector3> grid = new Dictionary<Vector3, Vector3>();
//        public Dictionary<System.Guid, Item> items = new Dictionary<System.Guid, Item>();
//        public Queue<System.Guid> itemsToCreate = new Queue<System.Guid>();
//        public Queue<System.Guid> itemsToMove = new Queue<System.Guid>();

//        public MachineInfo[] MachinesInfo;
//        public ItemInfo[] ItemsInfo;

//        public Material beltMaterial;
//        public GameObject floorTile;

//        public int money = 10000;

//        private HealthManager healthBarManager;
//        public GameObject healthBarPrefab;

//        private float moveTextureSpeed = 1;
//        private int ticksPerSecond = 6;
//        float moveX;

//        [SerializeField]
//        private int size = 1;

//        private void Awake() {
//            Instance = this;

//            processMachineInfo();

//            healthBarManager = gameObject.AddComponent<HealthManager>();
//            healthBarManager.healthBarPrefab = healthBarPrefab;
//        }

//        private void Start() {
//            InvokeRepeating("moveItemsInBelts", 0, 1.0f / ticksPerSecond);
//            InvokeRepeating("executeMachines", 0, 1.0f / ticksPerSecond);
//        }

//        void Update() {
//            animateConveyorBelt();
//            moveItemsInBelts();
//            createItems();
//            moveItems();
//        }

//        /*
//         * Reads pending items queue and creates them
//         */
//        private void createItems () {
//            while (itemsToCreate.Any()) {
//                System.Guid id = itemsToCreate.Dequeue();
//                Item item = items[id];
//                GameObject obj = Instantiate(item.info.prefab, item.position, Quaternion.Euler(-90, 0, 0), getMachineAt(item.parent).gameObject.transform);
//                item.transform = obj.transform;

//                // 创建血条
//                if (healthBarPrefab)
//                {
//                    healthBarManager.CreateHealthBarForItem(item, obj.transform);
//                }
//            }
//        }

//        //private void moveItems() {
//        //    while (itemsToMove.Any()) {
//        //        System.Guid id = itemsToMove.Dequeue();
//        //        Item item = items[id];
//        //        item.transform.parent = getMachineAt(item.parent).gameObject.transform;
//        //        item.transform.localPosition = item.position;
//        //    }
//        //}

//        private void moveItems()
//        {
//            while (itemsToMove.Count > 0)
//            {
//                System.Guid id = itemsToMove.Dequeue();

//                // 多重安全检查
//                if (!items.ContainsKey(id)) continue; // 物品已被移除
//                if (items[id] == null) continue; // 物品引用已释放
//                if (items[id].transform == null) continue; // transform已销毁

//                Item item = items[id];

//                // 检查父机器是否有效
//                if (!grid.ContainsKey(item.parent)) continue;
//                Vector3 machinePosition = grid[item.parent];
//                if (!machines.ContainsKey(machinePosition)) continue;

//                Machine parentMachine = machines[machinePosition];
//                if (parentMachine == null || parentMachine.gameObject == null) continue;

//                try
//                {
//                    // 安全设置父对象和位置
//                    item.transform.parent = parentMachine.gameObject.transform;
//                    item.transform.localPosition = item.position;
//                }
//                catch (System.NullReferenceException)
//                {
//                    // 捕获可能的空引用异常
//                    Debug.LogWarning($"移动物品时发生空引用: {id}");
//                }
//                catch (System.Exception e)
//                {
//                    Debug.LogError($"移动物品时出错: {id} - {e.Message}");
//                }
//            }
//        }

//        public void moveItem (System.Guid id, Vector3 position) {
//            items[id].position = position;
//            itemsToMove.Enqueue(id);
//        }

//        private void processMachineInfo () {
//            // make some data easier to access
//            foreach (MachineInfo machine in MachinesInfo) {
//                foreach (ItemUI item in machine._storageLimits) {
//                    machine.storageLimits.Add(item.type, item.quantity);
//                }
//                machine._storageLimits = null;
//            }
//        }

//        private void animateConveyorBelt () {
//            // animate conveyor belt texture
//            moveX = (moveTextureSpeed * Time.deltaTime) + moveX;
//            if (moveX > 1) moveX = 0;
//            beltMaterial.mainTextureOffset = new Vector2(moveX, 0);
//        }
//        private void moveItemsInBelts () {
//            foreach (Machine machine in machines.Values) {
//                if (machine.type != Machine.Type.Belt) {
//                    continue;
//                }
//            }
//        }

//        private void executeMachines () {
//            /*
//            var job = new MachineJob();
//            JobHandle handle = job.Schedule();
//            handle.Complete();
//            */
//            foreach (Machine machine in machines.Values)
//            {
//                machine.run();
//            }
//            /*
//            foreach (Machine machine in machines.Values) {
//               machine.run();
//            }*/
//        }

//        public Vector3 GetNearestPointOnGrid(Vector3 position)
//        {
//            position.y = 0.3f;
//            position -= transform.position;

//            int xCount = Mathf.RoundToInt(position.x / size);
//            int yCount = Mathf.RoundToInt(position.y / size);
//            int zCount = Mathf.RoundToInt(position.z / size);

//            Vector3 result = new Vector3(
//                (float)xCount * size,
//                (float)yCount * size,
//                (float)zCount * size);

//            result += transform.position;

//            return result;
//        }

//        public Vector3[] getNeighbors(Vector3 position) {
//            return new Vector3[] {
//                new Vector3(position.x - 1, position.y, position.z),
//                new Vector3(position.x + 1, position.y, position.z),
//                new Vector3(position.x, position.y, position.z - 1),
//                new Vector3(position.x, position.y, position.z + 1),
//            };
//        }

//        public Position getNeighborPosition (Vector3 position, Vector3 neighbor) {
//            if (neighbor.z == position.z) { // horizontal relation --
//                return (neighbor.x > position.x) ? Position.Right : Position.Left;
//            } else {
//                return (neighbor.z > position.z) ? Position.Top : Position.Bottom;
//            }
//        }

//        public bool isAvailable(Vector3 pos) {
//            return !grid.ContainsKey(pos);
//        }

//        public Machine getMachineAt (Vector3 pos) {
//            return machines[grid[pos]];
//        }

//        public void place (Machine item) {
//            if (!isAvailable(item.position) || item.info.price > money) {
//                return;
//            }
//            machines.Add(item.position, item);

//            foreach (Vector3 pos in getMachineBlocks(item.position, item.type)) {
//                grid.Add(pos, item.position);
//            }
//            money -= item.info.price;
//        }

//        public Vector3[] getMachineBlocks (Vector3 gridPosition, Machine.Type type) {
//            MachineInfo machineInfo = getMachineInfo(type);
//            Vector3[] blocks = new Vector3[machineInfo.x * machineInfo.y];
//            Vector3 extraBlock = gridPosition;

//            int i = 0;
//            int x = 0;
//            int y = 0;

//            while (x < machineInfo.x) {
//                y = 0;
//                extraBlock.z = gridPosition.z;

//                while (y < machineInfo.y) {
//                    blocks[i] = extraBlock;
//                    extraBlock.z++;
//                    y++;
//                    i++;
//                }

//                extraBlock.x++;
//                x++;
//            }
//            return blocks;
//        }

//        public MachineInfo getMachineInfo(Machine.Type type) {
//            foreach (MachineInfo machineInfo in MachinesInfo) {
//                if (machineInfo.type == type) {
//                    return machineInfo;
//                }
//            }
//            throw new System.Exception("Requested machine type not found: " + type);
//        }
//        public ItemInfo getItemInfo(Item.Type type) {
//            foreach (ItemInfo itemInfo in ItemsInfo) {
//                if (itemInfo.type == type) {
//                    return itemInfo;
//                }
//            }
//            throw new System.Exception("Requested item type not found: " + type);
//        }

//        // GameGirdManager.cs (添加监控)
//        void OnGUI()
//        {
//            GUILayout.Label($"待创建物品: {itemsToCreate.Count}");
//            GUILayout.Label($"待移动物品: {itemsToMove.Count}");

//            foreach (var machine in machines.Values)
//            {
//                if (machine.storedItems.Count > 0)
//                {
//                    string items = string.Join(",",
//                        machine.storedItems.Select(kv => $"{kv.Key}:{kv.Value}"));
//                    GUILayout.Label($"{machine.type} 库存: {items}");
//                }
//            }
//        }

//        public void ClearAllPlayerBuildings()
//        {
//            // 1. 创建待删除列表（避免修改正在遍历的集合）
//            List<Machine> machinesToDelete = new List<Machine>(machines.Values);
//            List<Item> itemsToDelete = new List<Item>(items.Values);

//            // 2. 删除所有物品实体
//            foreach (Item item in itemsToDelete)
//            {
//                if (item.transform != null)
//                {
//                    Destroy(item.transform.gameObject);
//                    Destroy(item.healthBar);
//                }
//                items.Remove(item.id);
//            }
//            itemsToCreate.Clear();
//            itemsToMove.Clear();

//            // 3. 删除所有机器
//            foreach (Machine machine in machinesToDelete)
//            {
//                // 删除机器游戏对象
//                if (machine.gameObject != null)
//                {
//                    Destroy(machine.gameObject);
//                }

//                // 清理网格占用
//                Vector3[] blocks = getMachineBlocks(machine.position, machine.type);
//                foreach (Vector3 block in blocks)
//                {
//                    grid.Remove(block);
//                }
//            }

//            // 4. 重置核心数据结构
//            machines.Clear();
//            grid.Clear();
//            items.Clear();
//        }

//        public void SafeRemoveItemFromQueues(System.Guid itemId)
//        {
//            // 从创建队列中移除
//            if (itemsToCreate.Contains(itemId))
//            {
//                Queue<System.Guid> newCreateQueue = new Queue<System.Guid>();
//                while (itemsToCreate.Count > 0)
//                {
//                    System.Guid currentId = itemsToCreate.Dequeue();
//                    if (currentId != itemId)
//                    {
//                        newCreateQueue.Enqueue(currentId);
//                    }
//                }
//                itemsToCreate = newCreateQueue;
//            }

//            // 从移动队列中移除
//            if (itemsToMove.Contains(itemId))
//            {
//                Queue<System.Guid> newMoveQueue = new Queue<System.Guid>();
//                while (itemsToMove.Count > 0)
//                {
//                    System.Guid currentId = itemsToMove.Dequeue();
//                    if (currentId != itemId)
//                    {
//                        newMoveQueue.Enqueue(currentId);
//                    }
//                }
//                itemsToMove = newMoveQueue;
//            }
//        }
//    }
//}
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactorySystem
{
    /// <summary>
    /// 游戏网格管理系统，负责管理所有机器、物品和网格状态
    /// </summary>
    public class GameGirdManager : BaseManager
    {
        #region Inspector Settings
        [SerializeField] private int gridSize = 1;
        [SerializeField] private Material beltMaterial;
        [SerializeField] private float beltMoveSpeed = 1f;

        // health manager
        // public GameObject healthBarPrefab;
        #endregion

        #region Runtime Data
        // 核心数据结构
        public Dictionary<Vector3, Machine> machines = new Dictionary<Vector3, Machine>();
        public Dictionary<Vector3, Vector3> gridOccupancy = new Dictionary<Vector3, Vector3>();
        public Dictionary<System.Guid, Item> items = new Dictionary<System.Guid, Item>();

        // 处理队列
        public Queue<System.Guid> itemsToCreate = new Queue<System.Guid>();
        public Queue<System.Guid> itemsToMove = new Queue<System.Guid>();

        private float beltTextureOffset;
        private GameObject parentObject = null;
        #endregion

        #region Initialization
        public GameGirdManager(GameObject parentObject, Material beltMaterial) 
        {
            this.parentObject = parentObject;
            this.beltMaterial = beltMaterial;
            // ecomoney manager
            // money = InitialMoney;

            // health manager
            // InitializeHealthBarManager();

            // machine manager
            // ProcessMachineInfo();
        }      
        #endregion

        #region Main Update Loop
        public override void Update()
        {
            AnimateConveyorBelt();
            ProcessBeltMovements();
        }

        /// <summary>
        /// 动画化传送带纹理
        /// </summary>
        private void AnimateConveyorBelt()
        {
            beltTextureOffset = (beltMoveSpeed * Time.deltaTime) + beltTextureOffset;
            if (beltTextureOffset > 1)
            {
                beltTextureOffset = 0;
            }
            beltMaterial.mainTextureOffset = new Vector2(beltTextureOffset, 0);
        }
        #endregion

        // item manager
        #region Item Management
        ///// <summary>
        ///// 创建待生成的物品
        ///// </summary>
        //private void CreatePendingItems()
        //{
        //    while (itemsToCreate.Count > 0)
        //    {
        //        System.Guid id = itemsToCreate.Dequeue();
        //        if (!items.TryGetValue(id, out Item item)) continue;

        //        CreateItemInstance(item);
        //    }
        //}

        ///// <summary>
        ///// 实例化物品游戏对象
        ///// </summary>
        //private void CreateItemInstance(Item item)
        //{
        //    // 获取父机器
        //    Machine parentMachine = GetMachineAt(item.parent);
        //    if (parentMachine == null || parentMachine.gameObject == null) return;

        //    // 实例化物品
        //    GameObject obj = Instantiate(
        //        item.info.prefab,
        //        item.position,
        //        Quaternion.Euler(-90, 0, 0),
        //        parentMachine.gameObject.transform
        //    );

        //    item.transform = obj.transform;

        //    // 创建血条
        //    if (healthBarPrefab != null)
        //    {
        //        healthBarManager.CreateHealthBarForItem(item, obj.transform);
        //    }

        //    // 添加物品碰撞组件
        //    ItemObject itemObject = obj.AddComponent<ItemObject>();
        //    itemObject.item = item;
        //}

        ///// <summary>
        ///// 移动待处理的物品
        ///// </summary>
        //private void MovePendingItems()
        //{
        //    // 创建临时队列避免修改正在迭代的集合
        //    var processingQueue = new Queue<System.Guid>(itemsToMove);
        //    itemsToMove.Clear();

        //    while (processingQueue.Count > 0)
        //    {
        //        System.Guid id = processingQueue.Dequeue();
        //        if (!items.TryGetValue(id, out Item item)) continue;

        //        MoveItemToParent(item);
        //    }
        //}

        ///// <summary>
        ///// 将物品移动到其父机器
        ///// </summary>
        //private void MoveItemToParent(Item item)
        //{
        //    // 验证父机器
        //    if (!gridOccupancy.TryGetValue(item.parent, out Vector3 machinePosition))
        //    {
        //        Debug.LogWarning($"物品的父位置无效: {item.parent}");
        //        return;
        //    }

        //    if (!machines.TryGetValue(machinePosition, out Machine parentMachine))
        //    {
        //        Debug.LogWarning($"父机器不存在: {machinePosition}");
        //        return;
        //    }

        //    if (parentMachine == null || parentMachine.gameObject == null)
        //    {
        //        Debug.LogWarning($"父机器游戏对象无效: {machinePosition}");
        //        return;
        //    }

        //    // 安全设置父对象和位置
        //    try
        //    {
        //        item.transform.SetParent(parentMachine.gameObject.transform, false);
        //        item.transform.localPosition = item.position;
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.LogError($"移动物品时出错: {e.Message}");
        //    }
        //}

        ///// <summary>
        ///// 请求移动物品到新位置
        ///// </summary>
        //public void RequestItemMove(System.Guid id, Vector3 position)
        //{
        //    if (items.TryGetValue(id, out Item item))
        //    {
        //        item.position = position;
        //        itemsToMove.Enqueue(id);
        //    }
        //}
        #endregion

        #region Machine Execution
        /// <summary>
        /// 处理传送带移动（目前为空，保留结构）
        /// </summary>
        private void ProcessBeltMovements()
        {
            // 预留传送带特定逻辑
        }
        #endregion

        #region Grid Operations
        /// <summary>
        /// 获取网格上最近的点
        /// </summary>
        public Vector3 GetNearestPointOnGrid(Vector3 position)
        {
            if (parentObject == null)
            {
                Debug.LogError("no parentObject");
                return new Vector3();
            };
            position.y = 0.3f;
            position -= parentObject.transform.position;

            int xCount = Mathf.RoundToInt(position.x / gridSize);
            int zCount = Mathf.RoundToInt(position.z / gridSize);

            Vector3 result = new Vector3(
                xCount * gridSize,
                0,
                zCount * gridSize);

            return result + parentObject.transform.position;
        }
        #endregion

        #region Data Access

        // item manager
        /// <summary>
        /// 获取物品信息
        /// </summary>
        //public ItemInfo GetItemInfo(Item.Type type)
        //{
        //    foreach (ItemInfo itemInfo in ItemsInfo)
        //    {
        //        if (itemInfo.type == type)
        //        {
        //            return itemInfo;
        //        }
        //    }
        //    throw new System.Exception($"未找到物品类型: {type}");
        //}

        // pollution manager
        ///// <summary>
        ///// 增加全局污染度
        ///// </summary>
        //public void AddPollution(float amount)
        //{
        //    globalPollution = Mathf.Min(maxPollution, globalPollution + amount);
        //    OnPollutionChanged?.Invoke(globalPollution);

        //    if (globalPollution >= pollutionWarningThreshold)
        //    {
        //        Debug.LogWarning($"污染度警告: {globalPollution}/{maxPollution}");
        //    }
        //}

        ///// <summary>
        ///// 减少全局污染度
        ///// </summary>
        //public void ReducePollution(float amount)
        //{
        //    globalPollution = Mathf.Max(0, globalPollution - amount);
        //    OnPollutionChanged?.Invoke(globalPollution);
        //}

        ///// <summary>
        ///// 获取当前污染度百分比
        ///// </summary>
        //public float GetPollutionPercentage()
        //{
        //    return globalPollution / maxPollution;
        //}
        #endregion

        #region Utility Methodss
        // item manager
        ///// <summary>
        ///// 从队列中安全移除物品
        ///// </summary>
        //public void SafeRemoveItemFromQueues(System.Guid itemId)
        //{
        //    // 从创建队列中移除
        //    itemsToCreate = new Queue<System.Guid>(itemsToCreate.Where(id => id != itemId));

        //    // 从移动队列中移除
        //    itemsToMove = new Queue<System.Guid>(itemsToMove.Where(id => id != itemId));
        //}

        ///// <summary>
        ///// 安全销毁物品
        ///// </summary>
        //public void SafeDestroyItem(System.Guid itemId)
        //{
        //    if (!items.TryGetValue(itemId, out Item item)) return;

        //    // 销毁游戏对象
        //    if (item.transform != null)
        //    {
        //        Destroy(item.transform.gameObject);
        //    }

        //    foreach (var machine in machines.Values)
        //    {
        //        if (machine is FactorySystem.Machines.Belt belt)
        //        {
        //            belt.RemoveItemFromBelt(itemId);
        //        }
        //    }

        //    // 销毁血条
        //    item.DestroyHealthBar();
        //    item.Destroy();

        //    // 从全局系统移除
        //    items.Remove(itemId);
        //    SafeRemoveItemFromQueues(itemId);
        //}
        #endregion

        #region Debug and UI
        #endregion
    }
}