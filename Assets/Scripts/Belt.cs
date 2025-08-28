#region v1 code
// v1
//using System.Collections.Generic;
//using UnityEngine;

//namespace FactorySystem.Machines
//{
//    public class Belt : Machine
//    {
//        public Dictionary<System.Guid, Item.Type> items = new Dictionary<System.Guid, Item.Type>();
//        public Dictionary<int, System.Guid> itemPositions = new Dictionary<int, System.Guid>();
//        public Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>() {
//            {1, new Vector3(-0.458f, 1, 0)},
//            {2, new Vector3(-0.151f, 1, 0)},
//            {3, new Vector3(0.194f, 1, 0)},
//            {4, new Vector3(0.44f, 1, 0)},
//        };

//        public bool hasFreeSlots
//        {
//            get
//            {
//                return !itemPositions.ContainsKey(1);
//            }
//        }

//        public Belt(GameObject gameObject) : base(gameObject, Type.Belt, gameObject.transform.position)
//        {
//        }

//        public void addItem(Item.Type type)
//        {
//            Debug.Log("0");
//            if (!hasFreeSlots)
//            {
//                Debug.LogError("Trying to add new item to a busy belt " + type);
//                return;
//            }

//            Vector3 spawnPos = position;
//            spawnPos.y = 1;

//            Item item = new Item();
//            item.id = System.Guid.NewGuid();
//            item.type = type;
//            item.position = spawnPos;
//            item.parent = position;

//            GameGirdManager.Instance.items.Add(item.id, item);
//            GameGirdManager.Instance.itemsToCreate.Enqueue(item.id);

//            GameObject obj = GameGirdManager.Instantiate(GameGirdManager.Instance.getItemInfo(type).prefab, spawnPos, Quaternion.Euler(-90, 0, 0), gameObject.transform);
//            item.transform = obj.transform;
//            addToItems(item.id, type);
//        }

//        public void addToItems(System.Guid item, Item.Type type)
//        {
//            items.Add(item, type);
//            itemPositions.Add(1, item);
//            Debug.Log("-1");
//            GameGirdManager.Instance.items[item].parent = position;
//            GameGirdManager.Instance.items[item].position = positions[1];
//            GameGirdManager.Instance.itemsToMove.Enqueue(item);

//            if (GameGirdManager.Instance.items[item].healthBar)
//            {
//                Transform transform = GameGirdManager.Instance.items[item].transform;
//                GameGirdManager.Instance.items[item].healthBar.GetComponent<HealthBar>().target = transform;
//            }
//        }

//        public override void run()
//        {
//            // start moving last item
//            if (itemPositions.ContainsKey(4))
//            {
//                Item.Type itemType = items[itemPositions[4]];
//                foreach (KeyValuePair<Vector3, ConnectionType> connection in connections)
//                {
//                    if (connection.Value == ConnectionType.Input)
//                    {
//                        continue;
//                    }

//                    Machine neighborMachine = GameGirdManager.Instance.getMachineAt(connection.Key);

//                    if (neighborMachine.type == Type.Belt)
//                    {
//                        Belt belt = (Belt)neighborMachine;
//                        if (belt.hasFreeSlots)
//                        {
//                            try
//                            {
//                                belt.addToItems(itemPositions[4], itemType);

//                                // unlink from this belt
//                                items.Remove(itemPositions[4]);
//                                itemPositions.Remove(4);
//                            }
//                            catch { }
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        // check if item can be stored in that machine
//                        if (!neighborMachine.info.storageLimits.ContainsKey(itemType) || !neighborMachine.canStoreItem(itemType))
//                        {
//                            continue;
//                        }

//                        if (!neighborMachine.storedItems.ContainsKey(itemType))
//                        {
//                            neighborMachine.storedItems.Add(itemType, 0);
//                        }
//                        neighborMachine.storedItems[itemType]++;

//                        System.Guid itemId = itemPositions[4];
//                        if (GameGirdManager.Instance.items.TryGetValue(itemId, out Item item))
//                        {
//                            // 在销毁物品的地方添加以下逻辑
//                            if (GameGirdManager.Instance.itemsToMove.Contains(itemId))
//                            {
//                                // 创建临时队列，移除待移动物品
//                                Queue<System.Guid> newQueue = new Queue<System.Guid>();
//                                while (GameGirdManager.Instance.itemsToMove.Count > 0)
//                                {
//                                    System.Guid currentId = GameGirdManager.Instance.itemsToMove.Dequeue();
//                                    if (currentId != itemId)
//                                    {
//                                        newQueue.Enqueue(currentId);
//                                    }
//                                }
//                                GameGirdManager.Instance.itemsToMove = newQueue;
//                            }
//                            GameGirdManager.Instance.SafeRemoveItemFromQueues(itemId);

//                            // 然后销毁物品
//                            if (GameGirdManager.Instance.items.TryGetValue(itemId, out item))
//                            {
//                                GameObject.Destroy(item.transform.gameObject);
//                                item.DestroyHealthBar();
//                                GameGirdManager.Instance.items.Remove(itemId);
//                            }
//                        }

//                        // unlink from this belt
//                        items.Remove(itemPositions[4]);
//                        //GameGirdManager.Destroy(itemPositions[4].gameObject);
//                        itemPositions.Remove(4);
//                    }
//                }
//            }


//            if (itemPositions.ContainsKey(3) && !itemPositions.ContainsKey(4))
//            {
//                GameGirdManager.Instance.moveItem(itemPositions[3], positions[4]);
//                itemPositions.Add(4, itemPositions[3]);
//                itemPositions.Remove(3);
//            }

//            if (itemPositions.ContainsKey(2) && !itemPositions.ContainsKey(3))
//            {
//                GameGirdManager.Instance.moveItem(itemPositions[2], positions[3]);
//                itemPositions.Add(3, itemPositions[2]);
//                itemPositions.Remove(2);
//            }
//            if (itemPositions.ContainsKey(1) && !itemPositions.ContainsKey(2))
//            {
//                GameGirdManager.Instance.moveItem(itemPositions[1], positions[2]);
//                itemPositions.Add(2, itemPositions[1]);
//                itemPositions.Remove(1);
//            }
//        }

//        protected override void onUpdateConnection(Vector3 neighbor)
//        {
//            refreshBarriers();
//        }

//        public void refreshBarriers()
//        {
//            gameObject.transform.Find(Direction.Up.ToString()).gameObject.SetActive(true);
//            gameObject.transform.Find(Direction.Down.ToString()).gameObject.SetActive(true);
//            gameObject.transform.Find(Direction.Left.ToString()).gameObject.SetActive(true);
//            gameObject.transform.Find(Direction.Right.ToString()).gameObject.SetActive(true);

//            foreach (Vector3 neighbor in connections.Keys)
//            {
//                refreshBarrier(neighbor);
//            }
//        }

//        public void refreshBarrier(Vector3 neighbor)
//        {
//            Direction barrierToRemove = Direction.Right;
//            GameGirdManager.Position neighborPosition = GameGirdManager.Instance.getNeighborPosition(position, neighbor);

//            switch (neighborPosition)
//            {
//                case GameGirdManager.Position.Right:
//                    switch (direction)
//                    {
//                        case Direction.Up:
//                            barrierToRemove = Direction.Right;
//                            break;
//                        case Direction.Down:
//                            barrierToRemove = Direction.Left;
//                            break;
//                        case Direction.Right:
//                            barrierToRemove = Direction.Up;
//                            break;
//                        case Direction.Left:
//                            barrierToRemove = Direction.Down;
//                            break;
//                    }
//                    break;
//                case GameGirdManager.Position.Left:
//                    switch (direction)
//                    {
//                        case Direction.Up:
//                            barrierToRemove = Direction.Left;
//                            break;
//                        case Direction.Down:
//                            barrierToRemove = Direction.Right;
//                            break;
//                        case Direction.Right:
//                            barrierToRemove = Direction.Down;
//                            break;
//                        case Direction.Left:
//                            barrierToRemove = Direction.Up;
//                            break;
//                    }
//                    break;
//                case GameGirdManager.Position.Top:
//                    switch (direction)
//                    {
//                        case Direction.Up:
//                            barrierToRemove = Direction.Up;
//                            break;
//                        case Direction.Down:
//                            barrierToRemove = Direction.Down;
//                            break;
//                        case Direction.Right:
//                            barrierToRemove = Direction.Left;
//                            break;
//                        case Direction.Left:
//                            barrierToRemove = Direction.Right;
//                            break;
//                    }
//                    break;
//                case GameGirdManager.Position.Bottom:
//                    switch (direction)
//                    {
//                        case Direction.Up:
//                            barrierToRemove = Direction.Down;
//                            break;
//                        case Direction.Down:
//                            barrierToRemove = Direction.Up;
//                            break;
//                        case Direction.Right:
//                            barrierToRemove = Direction.Right;
//                            break;
//                        case Direction.Left:
//                            barrierToRemove = Direction.Left;
//                            break;
//                    }
//                    break;
//            }

//            gameObject.transform.Find(barrierToRemove.ToString()).gameObject.SetActive(false);
//        }
//    }
//}
#endregion

// v2
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FactorySystem.Machines
{
    /// <summary>
    /// 传送带机器类，负责物品在传送带上的移动和传输
    /// </summary>
    public class Belt : Machine
    {
        // 物品字典：物品ID -> 物品类型
        public Dictionary<System.Guid, Item.Type> items = new Dictionary<System.Guid, Item.Type>();

        // 位置字典：槽位ID -> 物品ID
        public Dictionary<int, System.Guid> itemPositions = new Dictionary<int, System.Guid>();

        // 槽位位置映射：槽位ID -> 局部位置（相对于传送带）
        public Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>() 
        {
            {1, new Vector3(-0.458f, 1, 0)},  // 入口位置（最左端）
            {2, new Vector3(-0.151f, 1, 0)},   // 中间位置1
            {3, new Vector3(0.194f, 1, 0)},    // 中间位置2
            {4, new Vector3(0.44f, 1, 0)},     // 出口位置（最右端）
        };

        /// <summary>
        /// 进度跟踪字典
        /// </summary>
        private Dictionary<System.Guid, float> itemProgress = new Dictionary<System.Guid, float>();
        private Dictionary<System.Guid, Vector3> itemStartPositions = new Dictionary<System.Guid, Vector3>();
        private Dictionary<System.Guid, Vector3> itemTargetPositions = new Dictionary<System.Guid, Vector3>();
        private Dictionary<System.Guid, float> itemMoveStartTimes = new Dictionary<System.Guid, float>();
        private Dictionary<System.Guid, float> itemSpeedVariations = new Dictionary<System.Guid, float>();
        private Dictionary<System.Guid, float> itemMoveTimers = new Dictionary<System.Guid, float>();


        /// <summary>
        /// 检查传送带是否有空闲槽位（位置1是否空闲）
        /// </summary>
        public bool hasFreeSlots => !itemPositions.ContainsKey(1);

        public Belt(GameObject gameObject) : base(gameObject, Type.Belt, gameObject.transform.position)
        {
        }

        /// <summary>
        /// 在传送带上添加新物品
        /// </summary>
        /// <param name="type">物品类型</param>
        public void addItem(Item.Type type, List<System.Object> normalObjects = null, List<UnityEngine.Object> unityObjects = null)
        {
            // 检查是否有空闲槽位
            if (!hasFreeSlots)
            {
                Debug.LogError($"传送带已满，无法添加新物品: {type}");
                return;
            }

            // 创建物品实例
            Vector3 spawnPos = position;
            spawnPos.y = 1;  // 确保物品在传送带上方

            Item item = GameApp.ItemManager.CreateItem(type, spawnPos, position, 100, 100);

            // 实例化物品游戏对象
            GameObject obj = GameObject.Instantiate(
                GameApp.ItemManager.GetItemInfo(type).prefab,
                spawnPos,
                Quaternion.Euler(-90, 0, 0),
                gameObject.transform
            );

            // 创建血条
            if (GameApp.HealthManager.healthBarPrefab != null)
            {
                GameApp.HealthManager.CreateHealthBarForItem(item, obj.transform);
            }

            item.transform = obj.transform;

            // 添加物品碰撞组件
            ItemObject itemObject = obj.AddComponent<ItemObject>();
            itemObject.item = item;

            // 将物品添加到传送带系统
            addToItems(item.id, type);
        }

        /// <summary>
        /// 将物品添加到传送带内部管理系统
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="type">物品类型</param>
        private void addToItems(System.Guid itemId, Item.Type type)
        {
            // 添加到传送带物品字典
            items.Add(itemId, type);

            // 将物品放置在第1个槽位
            itemPositions.Add(1, itemId);

            // 初始化物品进度
            itemProgress[itemId] = 0f;

            float speedVariation = Random.Range(0.9f, 1.1f);
            itemSpeedVariations[itemId] = speedVariation;

            // 更新物品父对象和位置
            GameApp.ItemManager.Items[itemId].parent = position;
            GameApp.ItemManager.Items[itemId].position = positions[1];
            GameApp.ItemManager.ItemsToMove.Enqueue(itemId);

            // 更新血条目标位置
            if (GameApp.ItemManager.Items[itemId].healthBar != null)
            {
                HealthBar healthBar = GameApp.ItemManager.Items[itemId].healthBar.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.target = GameApp.ItemManager.Items[itemId].transform;
                }
            }
        }

        /// <summary>
        /// 传送带主运行逻辑，每帧调用
        /// </summary>
        public override void Run()
        {
            // 处理出口位置的物品（槽位4）
            ProcessExitPositionItem();

            // 移动传送带上的物品
            // MoveItemsOnBelt();

            // 移动传送带上的物品（基于速度）
            // MoveItemsOnBeltWithSpeed();

            // 平滑移动
            MoveItemsSmoothly();
        }

        /// <summary>
        /// 处理出口位置的物品（槽位4）
        /// </summary>
        private void ProcessExitPositionItem()
        {
            if (!itemPositions.TryGetValue(4, out System.Guid itemId))
            {
                return;
            }

            Item.Type itemType = items[itemId];
            bool itemProcessed = false;

            // 遍历所有输出连接
            foreach (KeyValuePair<Vector3, ConnectionType> connection in connections)
            {
                // 跳过输入连接
                if (connection.Value == ConnectionType.Input)
                {
                    continue;
                }

                Machine neighborMachine = GameApp.MachineManager.GetMachineAt(connection.Key);
                if (neighborMachine == null)
                {
                    continue;
                }

                // 连接到另一条传送带
                if (neighborMachine.type == Type.Belt)
                {
                    if (TryTransferToBelt(neighborMachine, itemId, itemType))
                    {
                        itemProcessed = true;
                        break; // 成功转移后跳出循环
                    }
                }
                // 连接到其他机器（如转换器、销售机等）
                else
                {
                    if (TryTransferToMachine(neighborMachine, itemType))
                    {
                        // 成功转移后销毁物品
                        SafeDestroyItem(itemId);
                        RemoveItemFromBelt(itemId);
                        itemProcessed = true;
                        break; // 成功转移后跳出循环
                    }
                }
            }

            // 如果没有机器接收物品，保持物品在当前位置
            if (!itemProcessed)
            {
                // 可以添加日志或未来可能的处理逻辑
            }
        }

        /// <summary>
        /// 尝试将物品转移到另一条传送带
        /// </summary>
        private bool TryTransferToBelt(Machine neighborMachine, System.Guid itemId, Item.Type itemType)
        {
            Belt belt = (Belt)neighborMachine;
            if (!belt.hasFreeSlots)
            {
                return false;
            }

            try
            {
                // 将物品转移到相邻传送带
                belt.addToItems(itemId, itemType);

                // 从当前传送带移除物品
                RemoveItemFromBelt(itemId);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"传送物品到传送带失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 尝试将物品转移到其他机器
        /// </summary>
        private bool TryTransferToMachine(Machine machine, Item.Type itemType)
        {
            // 检查机器是否能存储该物品
            if (!machine.info.storageLimits.ContainsKey(itemType) ||
                !machine.CanStoreItem(itemType))
            {
                return false;
            }

            // 初始化物品类型存储（如果不存在）
            if (!machine.storedItems.ContainsKey(itemType))
            {
                machine.storedItems.Add(itemType, 0);
            }

            // 增加物品存储数量
            machine.storedItems[itemType]++;
            return true;
        }

        /// <summary>
        /// 安全销毁物品（从所有队列和系统中移除）
        /// </summary>
        private void SafeDestroyItem(System.Guid itemId)
        {
            if (GameApp.ItemManager.Items.TryGetValue(itemId, out Item item))
            {
                // 从移动队列中移除
                GameApp.ItemManager.SafeRemoveItemFromQueues(itemId);

                // 销毁游戏对象
                if (item.transform != null)
                {
                    GameObject.Destroy(item.transform.gameObject);
                }

                // 销毁血条
                item.DestroyHealthBar();

                // 从全局物品系统移除
                GameApp.ItemManager.Items.Remove(itemId);
            }
        }

        /// <summary>
        /// 从传送带移除物品引用
        /// </summary>
        public void RemoveItemFromBelt(System.Guid itemId)
        {
            // 从物品字典移除
            if (items.ContainsKey(itemId))
            {
                items.Remove(itemId);
            }

            if (itemProgress.ContainsKey(itemId))
            {
                itemProgress.Remove(itemId);
            }

            // 从位置字典移除
            var positionEntry = itemPositions.FirstOrDefault(p => p.Value == itemId);
            if (positionEntry.Key != 0)
            {
                itemPositions.Remove(positionEntry.Key);
            }
        }

        //public void SafeRemoveItemFromBelt(System.Guid itemId)
        //{
        //    // 从物品字典移除
        //    if (items.ContainsKey(itemId))
        //    {
        //        items.Remove(itemId);
        //    }

        //    // 从位置字典移除
        //    var positionEntry = itemPositions.FirstOrDefault(p => p.Value == itemId);
        //    if (positionEntry.Key != 0)
        //    {
        //        itemPositions.Remove(positionEntry.Key);
        //    }
        //}

        /// <summary>
        /// 移动传送带上的物品
        /// </summary>
        private void MoveItemsOnBelt()
        {
            // 从后往前移动物品（避免覆盖）
            TryMoveItem(3, 4); // 槽位3 -> 槽位4
            TryMoveItem(2, 3); // 槽位2 -> 槽位3
            TryMoveItem(1, 2); // 槽位1 -> 槽位2
        }

        /// <summary>
        /// 基于速度移动传送带上的物品
        /// </summary>
        private void MoveItemsOnBeltWithSpeed()
        {
            // 获取时间增量（秒）
            float deltaTime = Time.deltaTime;

            // 从后往前处理物品（避免覆盖）
            for (int slot = 4; slot >= 1; slot--)
            {
                if (itemPositions.TryGetValue(slot, out System.Guid itemId))
                {
                    Item item = GameApp.ItemManager.Items[itemId];

                    // 计算基于速度的进度增量
                    float progressIncrement = item.Info.MoveSpeed * deltaTime * 5;
                    itemProgress[itemId] += progressIncrement;

                    // 当进度达到1时，尝试移动到下一个槽位
                    if (itemProgress[itemId] >= 1f)
                    {
                        TryMoveItemWithSpeed(itemId, slot, slot + 1);
                        itemProgress[itemId] = 0f; // 重置进度
                    }
                }
            }
        }

        /// <summary>
        /// 平滑移动传送带上的物品
        /// </summary>
        private void MoveItemsSmoothly()
        {
            float deltaTime = Time.deltaTime;

            // 处理每个槽位的物品移动
            for (int slot = 1; slot <= 4; slot++)
            {
                if (!itemPositions.TryGetValue(slot, out System.Guid itemId))
                {
                    continue;
                }

                if (!GameApp.ItemManager.Items.TryGetValue(itemId, out Item item))
                {
                    continue;
                }

                // 如果物品正在移动中，更新移动进度
                if (itemMoveTimers.ContainsKey(itemId))
                {
                    itemMoveTimers[itemId] += deltaTime * item.Info.MoveSpeed * itemSpeedVariations[itemId] * 2;
                    float progress = Mathf.Clamp01(itemMoveTimers[itemId]);

                    // 使用平滑的插值函数（如EaseOut）
                    float smoothProgress = SmoothStep(progress);

                    // 计算当前位置
                    Vector3 currentPos = Vector3.Lerp(
                        itemStartPositions[itemId],
                        itemTargetPositions[itemId],
                        smoothProgress
                    );

                    // 更新物品位置
                    item.transform.localPosition = currentPos;

                    // 移动完成
                    if (progress >= 1f)
                    {
                        CompleteItemMove(itemId, slot);
                    }
                }
                else if (slot < 4 && !itemPositions.ContainsKey(slot + 1))
                {
                    // 准备移动到下一个槽位
                    PrepareItemMove(itemId, slot, slot + 1);
                }
            }
        }

        /// <summary>
        /// 平滑步进函数（替代Mathf.SmoothStep以获得更多控制）
        /// </summary>
        private float SmoothStep(float t)
        {
            // 使用二次缓出函数，使移动末尾减速
            return 1f - (1f - t) * (1f - t);
        }

        /// <summary>
        /// 准备物品移动
        /// </summary>
        private void PrepareItemMove(System.Guid itemId, int currentSlot, int targetSlot)
        {
            if (!GameApp.ItemManager.Items.TryGetValue(itemId, out Item item))
            {
                return;
            }

            itemStartPositions[itemId] = positions[currentSlot];
            itemTargetPositions[itemId] = positions[targetSlot];
            itemMoveTimers[itemId] = 0f;
        }

        /// <summary>
        /// 完成物品移动
        /// </summary>
        private void CompleteItemMove(System.Guid itemId, int currentSlot)
        {
            // 更新槽位映射
            int targetSlot = currentSlot + 1;
            itemPositions.Remove(currentSlot);
            itemPositions.Add(targetSlot, itemId);

            // 清理移动数据
            itemStartPositions.Remove(itemId);
            itemTargetPositions.Remove(itemId);
            itemMoveTimers.Remove(itemId);

            // 确保物品精确到达目标位置
            if (GameApp.ItemManager.Items.TryGetValue(itemId, out Item item))
            {
                item.transform.localPosition = positions[targetSlot];
                item.position = positions[targetSlot];
            }
        }

        /// <summary>
        /// 尝试将物品移动到下一个槽位
        /// </summary>
        private void TryMoveItemWithSpeed(System.Guid itemId, int currentSlot, int targetSlot)
        {
            // 检查目标槽位是否有效且为空
            if (targetSlot > 4 || itemPositions.ContainsKey(targetSlot))
            {
                return;
            }

            // 更新物品位置
            GameApp.ItemManager.RequestItemMove(itemId, positions[targetSlot]);

            // 更新槽位映射
            itemPositions.Remove(currentSlot);
            itemPositions.Add(targetSlot, itemId);
        }

        /// <summary>
        /// 尝试将物品从一个槽位移动到另一个槽位
        /// </summary>
        /// <param name="fromSlot">源槽位</param>
        /// <param name="toSlot">目标槽位</param>
        private void TryMoveItem(int fromSlot, int toSlot)
        {
            // 检查源槽位是否有物品且目标槽位为空
            if (itemPositions.ContainsKey(fromSlot) && !itemPositions.ContainsKey(toSlot))
            {
                System.Guid itemId = itemPositions[fromSlot];
                // 更新物品位置
                GameApp.ItemManager.RequestItemMove(itemId, positions[toSlot]);

                // 更新槽位映射
                itemPositions.Add(toSlot, itemId);
                itemPositions.Remove(fromSlot);
            }
        }

        /// <summary>
        /// 当连接更新时刷新屏障
        /// </summary>
        protected override void OnConnectionUpdated(Vector3 neighbor)
        {
            refreshBarriers();
        }

        /// <summary>
        /// 刷新所有连接屏障
        /// </summary>
        public void refreshBarriers()
        {
            // 启用所有方向的屏障
            EnableBarrier(Direction.Up);
            EnableBarrier(Direction.Down);
            EnableBarrier(Direction.Left);
            EnableBarrier(Direction.Right);

            // 根据连接状态刷新每个屏障
            foreach (Vector3 neighbor in connections.Keys)
            {
                refreshBarrier(neighbor);
            }
        }

        /// <summary>
        /// 启用指定方向的屏障
        /// </summary>
        private void EnableBarrier(Direction direction)
        {
            Transform barrier = gameObject.transform.Find(direction.ToString());
            if (barrier != null)
            {
                barrier.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 刷新单个邻居的屏障
        /// </summary>
        public void refreshBarrier(Vector3 neighbor)
        {
            Direction barrierToRemove = GetBarrierToRemove(neighbor);
            DisableBarrier(barrierToRemove);
        }

        /// <summary>
        /// 根据邻居位置确定需要移除的屏障
        /// </summary>
        private Direction GetBarrierToRemove(Vector3 neighbor)
        {
            MachineManager.Position neighborPosition = GameApp.MachineManager.GetNeighborPosition(position, neighbor);

            // 根据当前方向和邻居位置确定需要移除的屏障
            switch (neighborPosition)
            {
                case MachineManager.Position.Right:
                    switch (direction)
                    {
                        case Direction.Up: return Direction.Right;
                        case Direction.Down: return Direction.Left;
                        case Direction.Right: return Direction.Up;
                        case Direction.Left: return Direction.Down;
                    }
                    break;

                 case MachineManager.Position.Left:
                    switch (direction)
                    {
                        case Direction.Up: return Direction.Left;
                        case Direction.Down: return Direction.Right;
                        case Direction.Right: return Direction.Down;
                        case Direction.Left: return Direction.Up;
                    }
                    break;

                case MachineManager.Position.Top:
                    switch (direction)
                    {
                        case Direction.Up: return Direction.Up;
                        case Direction.Down: return Direction.Down;
                        case Direction.Right: return Direction.Left;
                        case Direction.Left: return Direction.Right;
                    }
                    break;

                case MachineManager.Position.Bottom:
                    switch (direction)
                    {
                        case Direction.Up: return Direction.Down;
                        case Direction.Down: return Direction.Up;
                        case Direction.Right: return Direction.Right;
                        case Direction.Left: return Direction.Left;
                    }
                    break;
            }

            return Direction.Right; // 默认值
        }

        /// <summary>
        /// 禁用指定方向的屏障
        /// </summary>
        private void DisableBarrier(Direction barrierDirection)
        {
            Transform barrier = gameObject.transform.Find(barrierDirection.ToString());
            if (barrier != null)
            {
                barrier.gameObject.SetActive(false);
            }
        }
    }
}
