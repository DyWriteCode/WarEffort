using Peque;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MachineInfo
{
    public Machine.Type type;
    public Machine.ExecutionType executionType;
    public string name;
    public string description;
    public int x = 1;
    public int y = 1;
    public int ticksBetweenExecution = 10;
    public GameObject prefab;
    public Texture icon;
    public ItemUI[] requiredItemsToProduce;
    public ItemUI[] itemsThatProduces;
    public ItemUI[] _storageLimits;
    public Dictionary<Item.Type, int> storageLimits = new Dictionary<Item.Type, int>();
    public int moneyThatGenerates = 0;
    public int price = 100;

    [Header("Damage Zone Settings")]
    public float damageRange = 5f;
    public int damageInterval = 10;
    public int damagePerTick = 10;

    [Header("Pollution Settings")]
    [Tooltip("每次运行产生的污染值")]
    public float pollutionPerRun = 1.0f;

    [Tooltip("污染因子（影响污染计算）")]
    public float pollutionFactor = 1.0f;
}
[Serializable]
public class ItemUI
{
    public Item.Type type;
    public int quantity;
}