// Assets/Scripts/ItemManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using FactorySystem;
using FactorySystem.Machines;
using UnityEngine;

/// <summary>
/// 物品管理器，负责集中处理所有物品相关操作
/// </summary>
public class ItemManager : BaseManager
{
    #region Properties and Fields
    /// <summary>物品信息字典：物品类型 -> 物品信息</summary>
    private Dictionary<Item.Type, ItemInfo> _itemInfoDict = new Dictionary<Item.Type, ItemInfo>();

    /// <summary>全局物品字典：物品ID -> 物品实例</summary>
    public Dictionary<System.Guid, Item> Items { get; private set; } = new Dictionary<System.Guid, Item>();

    /// <summary>待创建物品队列</summary>
    public Queue<System.Guid> ItemsToCreate { get; private set; } = new Queue<System.Guid>();

    /// <summary>待移动物品队列</summary>
    public Queue<System.Guid> ItemsToMove { get; private set; } = new Queue<System.Guid>();
    #endregion

    #region Initialization
    public ItemManager(ItemInfo[] itemInfos)
    {
        ProcessItemInfo(itemInfos);
    }

    /// <summary>
    /// 处理物品信息
    /// </summary>
    private void ProcessItemInfo(ItemInfo[] itemInfos)
    {
        if (itemInfos != null)
        {
            foreach (ItemInfo itemInfo in itemInfos)
            {
                _itemInfoDict[itemInfo.type] = itemInfo;
            }
        }
    }
    #endregion

    #region Item Information Access
    /// <summary>
    /// 获取物品信息
    /// </summary>
    public ItemInfo GetItemInfo(Item.Type type)
    {
        if (_itemInfoDict.TryGetValue(type, out ItemInfo info))
        {
            return info;
        }
        throw new Exception($"物品类型未找到: {type}");
    }
    #endregion

    #region Item Lifecycle Management
    /// <summary>
    /// 创建新物品
    /// </summary>
    public Item CreateItem(Item.Type type, Vector3 position, Vector3 parent)
    {
        Item item = new Item
        {
            id = System.Guid.NewGuid(),
            type = type,
            position = position,
            parent = parent,
            maxHp = 100, // 默认值，可根据物品信息扩展
            Hp = 100
        };

        // 添加到全局系统
        Items.Add(item.id, item);
        ItemsToCreate.Enqueue(item.id);

        return item;
    }

    /// <summary>
    /// 安全销毁物品
    /// </summary>
    public void SafeDestroyItem(System.Guid itemId)
    {
        if (!Items.TryGetValue(itemId, out Item item)) return;

        // 销毁游戏对象
        if (item.transform != null)
        {
            GameObject.Destroy(item.transform.gameObject);
        }

        // 从传送带移除物品
        RemoveItemFromBelts(itemId);

        // 销毁血条
        item.DestroyHealthBar();
        item.Destroy();

        // 从全局系统移除
        Items.Remove(itemId);
        SafeRemoveItemFromQueues(itemId);
    }

    /// <summary>
    /// 从所有传送带移除物品
    /// </summary>
    private void RemoveItemFromBelts(System.Guid itemId)
    {
        foreach (Machine machine in GameApp.MachineManager.GetAllMachines())
        {
            if (machine is Belt belt)
            {
                belt.RemoveItemFromBelt(itemId);
            }
        }
    }

    /// <summary>
    /// 从队列中安全移除物品
    /// </summary>
    public void SafeRemoveItemFromQueues(System.Guid itemId)
    {
        // 从创建队列中移除
        ItemsToCreate = new Queue<System.Guid>(ItemsToCreate.Where(id => id != itemId));

        // 从移动队列中移除
        ItemsToMove = new Queue<System.Guid>(ItemsToMove.Where(id => id != itemId));
    }
    #endregion

    #region Item Movement
    /// <summary>
    /// 请求移动物品到新位置
    /// </summary>
    public void RequestItemMove(System.Guid id, Vector3 position)
    {
        if (Items.TryGetValue(id, out Item item))
        {
            item.position = position;
            ItemsToMove.Enqueue(id);
        }
    }
    #endregion

    #region Batch Operations
    /// <summary>
    /// 清空所有物品
    /// </summary>
    public void ClearAllItems()
    {
        // 创建副本避免修改集合
        var itemsCopy = new List<Item>(Items.Values);

        // 删除所有物品
        foreach (Item item in itemsCopy)
        {
            SafeDestroyItem(item.id);
        }

        // 重置队列
        ItemsToCreate.Clear();
        ItemsToMove.Clear();
    }
    #endregion
}