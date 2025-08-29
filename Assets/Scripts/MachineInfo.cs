using FactorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MachineInfo
{
    #region Basic Machine Properties
    /// <summary>
    /// 机器类型枚举
    /// </summary>
    public Machine.Type type;

    /// <summary>
    /// 机器执行类型（生产/销售等）
    /// </summary>
    public Machine.ExecutionType executionType;

    /// <summary>
    /// 机器显示名称
    /// </summary>
    public string name;

    /// <summary>
    /// 机器描述文本
    /// </summary>
    public string description;

    /// <summary>
    /// 机器在X轴占用的网格单位
    /// </summary>
    [Tooltip("X轴占用网格数量")]
    public int x = 1;

    /// <summary>
    /// 机器在Y轴占用的网格单位
    /// </summary>
    [Tooltip("Y轴占用网格数量")]
    public int y = 1;

    /// <summary>
    /// 预制体引用
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// 图标纹理
    /// </summary>
    public Texture icon;

    /// <summary>
    /// 机器价格
    /// </summary>
    [Tooltip("建造所需金钱")]
    public int price = 100;
    #endregion

    #region Production Settings
    /// <summary>
    /// 每次生产周期需要的游戏刻
    /// </summary>
    [Tooltip("生产间隔（游戏刻）")]
    public int ticksBetweenExecution = 10;

    /// <summary>
    /// 生产所需的原材料列表
    /// </summary>
    [Header("生产配方")]
    public ItemUI[] requiredItemsToProduce;

    /// <summary>
    /// 生产出的物品列表
    /// </summary>
    [Header("生产产出")]
    public ItemUI[] itemsThatProduces;

    /// <summary>
    /// 销售机器专用：每次销售获得的金钱
    /// </summary>
    [Header("销售设置")]
    [Tooltip("每次销售获得的金钱")]
    [EnumConditionalHide(nameof(executionType), Machine.ExecutionType.Seller)]
    public int moneyThatGenerates = 0;
    #endregion

    #region Storage Settings
    /// <summary>
    /// 物品存储限制（编辑器可视化的原始数据）
    /// </summary>
    [Header("存储限制")]
    [Tooltip("物品类型 -> 最大存储量")]
    public ItemUI[] _storageLimits;

    /// <summary>
    /// 物品存储限制（运行时使用的字典格式）
    /// </summary>
    [NonSerialized]
    public Dictionary<Item.Type, int> storageLimits = new Dictionary<Item.Type, int>();
    #endregion

    #region Pollution Settings
    /// <summary>
    /// 每次运行产生的基础污染值
    /// </summary>
    [Header("污染设置")]
    [Tooltip("每次运行产生的基础污染值")]
    public float pollutionPerRun = 1f;

    /// <summary>
    /// 污染系数（影响最终污染计算）
    /// </summary>
    [Tooltip("污染系数（乘数）")]
    [Range(0.1f, 5f)]
    public float pollutionFactor = 1.0f;

    /// <summary>
    /// 污染影响范围半径
    /// </summary>
    [Tooltip("污染影响范围半径")]
    public float pollutionRadius = 5f;
    #endregion

    #region Special Machine Settings
    /// <summary>
    /// 攻击机器专用：伤害范围
    /// </summary>
    [Header("攻击机器设置")]
    [Tooltip("伤害影响范围半径")]
    [EnumConditionalHide(nameof(type), Machine.Type.AttackMachine)]
    public float damageRange = 5f;

    /// <summary>
    /// 攻击机器专用：伤害间隔
    /// </summary>
    [Tooltip("伤害间隔（游戏刻）")]
    [EnumConditionalHide(nameof(type), Machine.Type.AttackMachine)]
    public int damageInterval = 10;

    /// <summary>
    /// 攻击机器专用：每次伤害值
    /// </summary>
    [Tooltip("每次造成的伤害值")]
    [EnumConditionalHide(nameof(type), Machine.Type.AttackMachine)]
    public int damagePerTick = 10;

    /// <summary>
    /// 清洁机器专用：清洁间隔
    /// </summary>
    [Header("清洁机器设置")]
    [Tooltip("清洁间隔（游戏刻）")]
    [EnumConditionalHide(nameof(type), Machine.Type.CleanerMachine)]
    public int cleanInterval = 20;

    /// <summary>
    /// 清洁机器专用：每次清洁量
    /// </summary>
    [EnumConditionalHide(nameof(type), Machine.Type.CleanerMachine)]
    [Tooltip("每次减少的污染值")]
    public float pollutionReduction = 50f;
    #endregion

    #region Editor Methods
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器专用：验证数据有效性
    /// </summary>
    private void OnValidate()
    {
        // 自动约束最小值
        x = Mathf.Max(1, x);
        y = Mathf.Max(1, y);
        price = Mathf.Max(0, price);
        ticksBetweenExecution = Mathf.Max(1, ticksBetweenExecution);
    }
#endif
    #endregion
}

/// <summary>
/// 物品数量关联结构（用于编辑器可视化）
/// </summary>
[Serializable]
public class ItemUI
{
    /// <summary>
    /// 物品类型
    /// </summary>
    [Tooltip("物品类型")]
    public Item.Type type;

    /// <summary>
    /// 物品数量
    /// </summary>
    [Tooltip("数量")]
    [Min(1)]
    public int quantity = 1;
}