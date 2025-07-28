#region v1
// v1 code
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Peque;
//using Peque.Machines;
//using System.Linq;

//public class Machine
//{
//    public enum Direction
//    {
//        Up = 270,
//        Left = 180,
//        Down = 90,
//        Right = 0,
//    }
//    [System.Serializable]
//    public enum Type
//    {
//        Belt = 1,
//        IronOreBroker = 2,
//        IronFoundry = 3,
//        IronSeller = 4,
//    }
//    public enum ExecutionType
//    {
//        Generator = 1,
//        Converter = 2,
//        Seller = 3,
//    }

//    public enum ConnectionType
//    {
//        Output = 0,
//        Input = 1,
//    }

//    public MachineInfo info {
//        get {
//            return GameGrid.Instance.GetMachineInfo(type);
//        }
//    }

//    public Vector3 position;
//    public Direction direction;
//    public Type type;
//    public GameObject gameObject;
//    public Dictionary<Vector3, ConnectionType> connections = new Dictionary<Vector3, ConnectionType>();
//    public Dictionary<Item.Type, int> storedItems = new Dictionary<Item.Type, int>();
//    public int ticksUntilNextExecution = 0;

//    public Machine (GameObject gameObject, Type type, Vector3 position) {
//        this.gameObject = gameObject;
//        this.type = type;
//        this.position = position;
//    }

//    public void delete () {
//        if (connections != null) {
//            foreach (Vector3 neighbor in connections.Keys) {
//                Machine neighborMachine = GameGrid.Instance.GetMachineAt(neighbor);

//                if (neighborMachine.type == Type.Belt) {
//                    ((Belt)neighborMachine).removeConnection(position, type);
//                } else {
//                    neighborMachine.removeConnection(position, type);
//                }
//            }
//        }

//        GameObject.Destroy(gameObject);

//        foreach(Vector3 pos in GameGrid.Instance.GetMachineBlocks(position, type)) {
//            GameGrid.Instance.grid.Remove(pos);
//        }
//        GameGrid.Instance.machines.Remove(position);
//    }

//    public void addConnection (Vector3 neighbor, ConnectionType connectionType) {
//        if (connections.ContainsKey(neighbor)) {
//            return;
//        }
//        connections.Add(neighbor, connectionType);
//        onUpdateConnection(neighbor);
//    }

//    /**
//     * This method receives the "main" grid position of neighbor machine,
//     * but since it may be linked from another of it's blocks
//     * we need to check each one individually
//    */
//    public void removeConnection (Vector3 neighbor, Type type) {
//        foreach (var pos in GameGrid.Instance.GetMachineBlocks(neighbor, type)) {
//            if (connections.ContainsKey(pos)) {
//                connections.Remove(pos);
//                onUpdateConnection(pos);
//            }
//        }
//    }

//    protected virtual void onUpdateConnection(Vector3 neighbor) {
//    }

//    public virtual void run()
//    {
//        ticksUntilNextExecution--;

//        if (ticksUntilNextExecution > 0)
//        {
//            return;
//        }

//        MachineInfo machineInfo = GameGrid.Instance.GetMachineInfo(type);

//        // reset timer
//        ticksUntilNextExecution = machineInfo.ticksBetweenExecution;

//        if (machineInfo.executionType != ExecutionType.Seller)
//        {
//            sendItemsToConveyorBelt(machineInfo);
//        }

//        if (!hasRequiredItemsToProduce(machineInfo))
//        {
//            return;
//        }

//        if (machineInfo.executionType != ExecutionType.Seller && !canStoreProducedItems(machineInfo))
//        {
//            return;
//        }

//        deductItemsToProduce(machineInfo);

//        switch (machineInfo.executionType)
//        {
//            case ExecutionType.Converter:
//                foreach (ItemUI item in machineInfo.itemsThatProduces)
//                {
//                    if (!storedItems.ContainsKey(item.type))
//                    {
//                        storedItems.Add(item.type, 0);
//                    }
//                    storedItems[item.type] += item.quantity;
//                }
//                break;
//            case ExecutionType.Generator:
//                foreach (ItemUI item in machineInfo.itemsThatProduces)
//                {
//                    if (!storedItems.ContainsKey(item.type))
//                    {
//                        storedItems.Add(item.type, 0);
//                    }
//                    storedItems[item.type] += item.quantity;
//                }
//                break;
//            case ExecutionType.Seller:
//                GameGrid.Instance.money += info.moneyThatGenerates;
//                foreach (var conn in connections.Where(c => c.Value == ConnectionType.Input))
//                {
//                    Machine inputMachine = GameGrid.Instance.GetMachineAt(conn.Key);
//                    if (inputMachine is Belt inputBelt)
//                    {
//                        // 销毁传送带最后一个物品
//                        if (inputBelt.itemPositions.TryGetValue(4, out Guid itemId))
//                        {
//                            if (GameGrid.Instance.items.TryGetValue(itemId, out Item item))
//                            {
//                                GameObject.Destroy(item.transform.gameObject);
//                                GameObject.Destroy(item.healthBar);
//                                GameGrid.Instance.items.Remove(itemId);
//                            }
//                            inputBelt.items.Remove(itemId);
//                            inputBelt.itemPositions.Remove(4);
//                        }
//                    }
//                }
//                break;
//        }
//    }


//    private void sendItemsToConveyorBelt(MachineInfo machineInfo)
//    {
//        bool hasItemsToSend = false;

//        foreach (ItemUI item in machineInfo.itemsThatProduces)
//        {
//            if (storedItems.ContainsKey(item.type) && storedItems[item.type] > 0)
//            {
//                hasItemsToSend = true;
//                break;
//            }
//        }

//        if (!hasItemsToSend)
//        {
//            return;
//        }

//        // know how many output connections has to divide the output between
//        int outputConnections = 0;

//        foreach (KeyValuePair<Vector3, ConnectionType> connection in connections)
//        {
//            if (connection.Value == ConnectionType.Output && ((Belt)GameGrid.Instance.GetMachineAt(connection.Key)).hasFreeSlots)
//            {
//                outputConnections++;
//            }
//        }

//        if (outputConnections < 1)
//        {
//            return;
//        }

//        // TEMPORAL
//        storedItems[machineInfo.itemsThatProduces.First().type]--;

//        foreach (KeyValuePair<Vector3, ConnectionType> connection in connections)
//        {
//            Belt belt = (Belt)GameGrid.Instance.GetMachineAt(connection.Key);
//            if (connection.Value == ConnectionType.Output && belt.hasFreeSlots)
//            {
//                belt.addItem(machineInfo.itemsThatProduces.First().type);
//                break;
//            }
//        }
//    }

//    private bool hasRequiredItemsToProduce(MachineInfo machineInfo)
//    {
//        foreach (ItemUI item in machineInfo.requiredItemsToProduce)
//        {
//            if (!storedItems.ContainsKey(item.type) || storedItems[item.type] < item.quantity)
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    private void deductItemsToProduce (MachineInfo machineInfo) {
//        foreach (ItemUI item in machineInfo.requiredItemsToProduce)
//        {
//            storedItems[item.type] -= item.quantity;

//            foreach (var conn in connections.Where(c => c.Value == ConnectionType.Input))
//            {
//                Machine inputMachine = GameGrid.Instance.GetMachineAt(conn.Key);
//                if (inputMachine is Belt inputBelt)
//                {
//                    // 查找匹配类型的物品
//                    var itemEntry = inputBelt.items.FirstOrDefault(x => x.Value == item.type);
//                    if (itemEntry.Key != default)
//                    {
//                        Guid itemId = itemEntry.Key;
//                        if (GameGrid.Instance.items.TryGetValue(itemId, out Item itemObj))
//                        {
//                            // 在销毁物品的地方添加以下逻辑
//                            if (GameGrid.Instance.itemsToMove.Contains(itemId))
//                            {
//                                // 创建临时队列，移除待移动物品
//                                Queue<System.Guid> newQueue = new Queue<System.Guid>();
//                                while (GameGrid.Instance.itemsToMove.Count > 0)
//                                {
//                                    System.Guid currentId = GameGrid.Instance.itemsToMove.Dequeue();
//                                    if (currentId != itemId)
//                                    {
//                                        newQueue.Enqueue(currentId);
//                                    }
//                                }
//                                GameGrid.Instance.itemsToMove = newQueue;
//                            }
//                            GameGrid.Instance.SafeRemoveItemFromQueues(itemId);

//                            Item item_obj = null;

//                            // 然后销毁物品
//                            if (GameGrid.Instance.items.TryGetValue(itemId, out item_obj))
//                            {
//                                GameObject.Destroy(item_obj.transform.gameObject);
//                                item_obj.DestroyHealthBar();
//                                GameGrid.Instance.items.Remove(itemId);
//                            }
//                        }
//                        inputBelt.items.Remove(itemId);
//                        // 同时从位置字典移除
//                        var posEntry = inputBelt.itemPositions.FirstOrDefault(p => p.Value == itemId);
//                        if (posEntry.Key != 0)
//                        {
//                            inputBelt.itemPositions.Remove(posEntry.Key);
//                        }
//                    }
//                }
//            }
//        }
//    }

//    public bool canStoreItem (Item.Type type, int quantity = 1) {
//        int currentQuantity = storedItems.ContainsKey(type) ? storedItems[type] : 0;
//        if ((currentQuantity + quantity) > info.storageLimits[type]) {
//            return false;
//        }
//        return true;
//    }

//    private bool canStoreProducedItems(MachineInfo machineInfo) {
//        switch (machineInfo.executionType) {
//            case ExecutionType.Converter:
//            case ExecutionType.Generator:
//                foreach (ItemUI item in machineInfo.itemsThatProduces) {
//                    if (!canStoreItem(item.type, item.quantity)) {
//                        return false;
//                    }
//                }
//                break;
//        }
//        return true;
//    }
//}
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Peque;
using Peque.Machines;

/// <summary>
/// 机器基类，表示游戏中的各种生产设备
/// </summary>
public class Machine
{
    #region Enums
    /// <summary>
    /// 机器方向枚举
    /// </summary>
    public enum Direction
    {
        Up = 270,
        Left = 180,
        Down = 90,
        Right = 0,
    }

    /// <summary>
    /// 机器类型枚举
    /// </summary>
    [System.Serializable]
    public enum Type
    {
        Belt = 1,
        IronOreBroker = 2,
        IronFoundry = 3,
        IronSeller = 4,
        AttackMachine = 5,
        CleanerMachine = 6,
    }

    /// <summary>
    /// 机器执行类型枚举
    /// </summary>
    public enum ExecutionType
    {
        Generator = 1,
        Converter = 2,
        Seller = 3,
        Building = 4,
    }

    /// <summary>
    /// 连接类型枚举
    /// </summary>
    public enum ConnectionType
    {
        Output = 0,
        Input = 1,
    }
    #endregion

    #region Properties and Fields
    /// <summary>机器位置</summary>
    public Vector3 position;
    /// <summary>机器方向</summary>
    public Direction direction;
    /// <summary>机器类型</summary>
    public Type type;
    /// <summary>关联的游戏对象</summary>
    public GameObject gameObject;

    /// <summary>连接字典：邻居位置 -> 连接类型</summary>
    public Dictionary<Vector3, ConnectionType> connections = new Dictionary<Vector3, ConnectionType>();

    /// <summary>存储的物品：物品类型 -> 数量</summary>
    public Dictionary<Item.Type, int> storedItems = new Dictionary<Item.Type, int>();

    /// <summary>距离下次执行的剩余时间单位</summary>
    public int ticksUntilNextExecution = 0;

    /// <summary>
    /// 获取机器信息
    /// </summary>
    public MachineInfo info => GameGrid.Instance.GetMachineInfo(type);
    #endregion

    #region Constructor
    public Machine(GameObject gameObject, Type type, Vector3 position)
    {
        this.gameObject = gameObject;
        this.type = type;
        this.position = position;
    }
    #endregion

    #region Connection Management
    /// <summary>
    /// 添加连接
    /// </summary>
    public void AddConnection(Vector3 neighbor, ConnectionType connectionType)
    {
        if (connections.ContainsKey(neighbor)) return;

        connections.Add(neighbor, connectionType);
        OnConnectionUpdated(neighbor);
    }

    /// <summary>
    /// 移除连接
    /// </summary>
    public void RemoveConnection(Vector3 neighbor, Type neighborType)
    {
        foreach (var pos in GameGrid.Instance.GetMachineBlocks(neighbor, neighborType))
        {
            if (connections.ContainsKey(pos))
            {
                connections.Remove(pos);
                OnConnectionUpdated(pos);
            }
        }
    }

    /// <summary>
    /// 当连接更新时调用（子类可重写）
    /// </summary>
    protected virtual void OnConnectionUpdated(Vector3 neighbor) { }
    #endregion

    #region Core Machine Logic
    /// <summary>
    /// 机器主运行逻辑
    /// </summary>
    public virtual void Run()
    {
        // 更新执行计时器
        if (--ticksUntilNextExecution > 0) return;

        // 重置计时器
        ticksUntilNextExecution = info.ticksBetweenExecution;

        // 非销售机尝试发送物品到传送带
        if (info.executionType != ExecutionType.Seller)
        {
            SendItemsToConveyorBelt();
        }

        // 检查生产条件
        if (!CanProduce()) return;

        // 消耗原料
        ConsumeInputItems();

        // 执行生产逻辑
        ProduceOutputItems();

        float pollutionAmount = info.pollutionPerRun * info.pollutionFactor;
        GameGrid.Instance.AddPollution(pollutionAmount);
    }

    /// <summary>
    /// 检查是否可以生产
    /// </summary>
    private bool CanProduce()
    {
        // 检查是否有足够原料
        if (!HasRequiredInputItems()) return false;

        // 检查是否有空间存储产品（销售机除外）
        return info.executionType == ExecutionType.Seller || CanStoreOutputItems();
    }

    /// <summary>
    /// 消耗输入物品
    /// </summary>
    private void ConsumeInputItems()
    {
        foreach (ItemUI requirement in info.requiredItemsToProduce)
        {
            // 减少库存
            storedItems[requirement.type] -= requirement.quantity;

            // 尝试从连接的传送带移除物理物品
            TryRemovePhysicalItemsFromBelts(requirement);
        }
    }

    /// <summary>
    /// 生产输出物品
    /// </summary>
    private void ProduceOutputItems()
    {
        switch (info.executionType)
        {
            case ExecutionType.Converter:
            case ExecutionType.Generator:
                AddItemsToStorage(info.itemsThatProduces);
                break;

            case ExecutionType.Seller:
                ExecuteSellingProcess();
                break;
        }
    }
    #endregion

    #region Production Logic
    /// <summary>
    /// 检查是否有足够的输入物品
    /// </summary>
    private bool HasRequiredInputItems()
    {
        foreach (ItemUI requirement in info.requiredItemsToProduce)
        {
            if (!storedItems.ContainsKey(requirement.type) ||
                storedItems[requirement.type] < requirement.quantity)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检查是否可以存储输出物品
    /// </summary>
    private bool CanStoreOutputItems()
    {
        foreach (ItemUI product in info.itemsThatProduces)
        {
            if (!CanStoreItem(product.type, product.quantity))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 添加物品到存储
    /// </summary>
    private void AddItemsToStorage(ItemUI[] itemsToAdd)
    {
        foreach (ItemUI item in itemsToAdd)
        {
            if (!storedItems.ContainsKey(item.type))
            {
                storedItems.Add(item.type, 0);
            }
            storedItems[item.type] += item.quantity;
        }
    }

    /// <summary>
    /// 执行销售过程
    /// </summary>
    private void ExecuteSellingProcess()
    {
        // 增加资金
        GameGrid.Instance.money += info.moneyThatGenerates;

        // 销毁输入传送带上的物品
        foreach (var conn in connections.Where(c => c.Value == ConnectionType.Input))
        {
            if (GameGrid.Instance.GetMachineAt(conn.Key) is Belt inputBelt)
            {
                DestroyLastItemOnBelt(inputBelt);
            }
        }
    }
    #endregion

    #region Belt Interaction
    /// <summary>
    /// 尝试从连接的传送带移除物理物品
    /// </summary>
    private void TryRemovePhysicalItemsFromBelts(ItemUI requirement)
    {
        foreach (var conn in connections.Where(c => c.Value == ConnectionType.Input))
        {
            if (GameGrid.Instance.GetMachineAt(conn.Key) is Belt inputBelt)
            {
                RemoveMatchingItemsFromBelt(inputBelt, requirement.type);
            }
        }
    }

    /// <summary>
    /// 从传送带移除匹配物品
    /// </summary>
    private void RemoveMatchingItemsFromBelt(Belt belt, Item.Type itemType)
    {
        // 查找匹配类型的物品
        var matchingItem = belt.items.FirstOrDefault(x => x.Value == itemType);
        if (matchingItem.Key == default) return;

        System.Guid itemId = matchingItem.Key;

        // 安全销毁物品
        GameGrid.Instance.SafeDestroyItem(itemId);

        // 从传送带移除引用
        belt.items.Remove(itemId);
        RemoveItemFromBeltPositions(belt, itemId);
    }

    /// <summary>
    /// 从传送带位置字典移除物品
    /// </summary>
    private void RemoveItemFromBeltPositions(Belt belt, System.Guid itemId)
    {
        var positionEntry = belt.itemPositions.FirstOrDefault(p => p.Value == itemId);
        if (positionEntry.Key != 0)
        {
            belt.itemPositions.Remove(positionEntry.Key);
        }
    }

    /// <summary>
    /// 销毁传送带上的最后一个物品
    /// </summary>
    private void DestroyLastItemOnBelt(Belt belt)
    {
        if (!belt.itemPositions.TryGetValue(4, out Guid itemId)) return;

        if (GameGrid.Instance.items.TryGetValue(itemId, out Item item))
        {
            GameObject.Destroy(item.transform.gameObject);
            item.DestroyHealthBar();
            GameGrid.Instance.items.Remove(itemId);
        }

        belt.items.Remove(itemId);
        belt.itemPositions.Remove(4);
    }

    /// <summary>
    /// 发送物品到传送带
    /// </summary>
    private void SendItemsToConveyorBelt()
    {
        // 检查是否有可发送的物品
        if (!HasItemsToSend()) return;

        // 检查是否有有效的输出连接
        var validOutputs = GetValidOutputConnections();
        if (validOutputs.Count == 0) return;

        // 发送物品到输出连接
        SendItemToOutput(validOutputs.First().Key);
    }

    /// <summary>
    /// 检查是否有可发送的物品
    /// </summary>
    private bool HasItemsToSend()
    {
        foreach (ItemUI product in info.itemsThatProduces)
        {
            if (storedItems.ContainsKey(product.type) && storedItems[product.type] > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取有效的输出连接
    /// </summary>
    private List<KeyValuePair<Vector3, ConnectionType>> GetValidOutputConnections()
    {
        return connections
            .Where(conn =>
                conn.Value == ConnectionType.Output &&
                GameGrid.Instance.GetMachineAt(conn.Key) is Belt neighborBelt &&
                neighborBelt.hasFreeSlots)
            .ToList();
    }

    /// <summary>
    /// 发送物品到输出连接
    /// </summary>
    private void SendItemToOutput(Vector3 outputPosition)
    {
        Belt outputBelt = (Belt)GameGrid.Instance.GetMachineAt(outputPosition);
        Item.Type itemType = info.itemsThatProduces.First().type;

        // 减少库存并添加到传送带
        storedItems[itemType]--;
        outputBelt.addItem(itemType);
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// 删除机器及其所有连接
    /// </summary>
    public void Delete()
    {
        // 清理所有连接
        CleanupConnections();

        // 销毁游戏对象
        if (gameObject != null)
        {
            GameObject.Destroy(gameObject);
        }

        // 清理网格占用
        ClearGridOccupancy();

        // 从网格管理器中移除
        GameGrid.Instance.machines.Remove(position);
    }

    /// <summary>
    /// 清理所有连接
    /// </summary>
    private void CleanupConnections()
    {
        if (connections == null) return;

        foreach (Vector3 neighbor in connections.Keys.ToList())
        {
            Machine neighborMachine = GameGrid.Instance.GetMachineAt(neighbor);
            if (neighborMachine == null) continue;

            if (neighborMachine.type == Type.Belt)
            {
                ((Belt)neighborMachine).RemoveConnection(position, type);
            }
            else
            {
                neighborMachine.RemoveConnection(position, type);
            }
        }
    }

    /// <summary>
    /// 清理网格占用
    /// </summary>
    private void ClearGridOccupancy()
    {
        foreach (Vector3 pos in GameGrid.Instance.GetMachineBlocks(position, type))
        {
            GameGrid.Instance.gridOccupancy.Remove(pos);
        }
    }

    /// <summary>
    /// 检查是否可以存储指定类型和数量的物品
    /// </summary>
    public bool CanStoreItem(Item.Type type, int quantity = 1)
    {
        // 获取当前数量
        int currentQuantity = storedItems.TryGetValue(type, out int count) ? count : 0;

        // 检查存储限制
        return info.storageLimits.TryGetValue(type, out int limit) &&
               (currentQuantity + quantity) <= limit;
    }

    public virtual bool ShouldExecute()
    {
        return true; // 默认所有机器都应执行
    }
    #endregion
}