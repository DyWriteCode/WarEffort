#region v1 code
//using FactorySystem.Machines;
//using FactorySystem;
//using UnityEngine;

//public class AttackMachineController : MonoBehaviour
//{
//    [HideInInspector]
//    public float damageRange = 5f;
//    [HideInInspector]
//    public int damageInterval = 10;
//    [HideInInspector]
//    public int damagePerTick = 10;

//    private Machine linkedMachine;

//    public void Initialize(Machine machine)
//    {
//        linkedMachine = machine;
//        GetComponent<SphereCollider>().radius = damageRange;
//        GetComponent<SphereCollider>().isTrigger = true;

//        // 可视化设置
//        var visualizer = gameObject.AddComponent<AttackMachineVisualizer>();
//        visualizer.Initialize(this);
//    }
//}
#endregion

using FactorySystem.Machines;
using UnityEngine;

/// <summary>
/// 攻击机器控制器，负责管理攻击机器的行为和可视化效果
/// </summary>
[RequireComponent(typeof(SphereCollider))] // 确保组件存在
public class AttackMachineController : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("伤害影响范围半径")]
    public float damageRange = 5f;

    [Tooltip("伤害间隔（游戏刻）")]
    public int damageInterval = 10;

    [Tooltip("每次造成的伤害值")]
    public int damagePerTick = 10;

    [Header("References")]
    [Tooltip("关联的机器实例")]
    private Machine linkedMachine;

    /// <summary>
    /// 初始化攻击机器
    /// </summary>
    /// <param name="machine">关联的机器实例</param>
    public void Initialize(Machine machine)
    {
        if (machine == null)
        {
            Debug.LogError("AttackMachineController初始化失败：机器实例为空");
            return;
        }

        linkedMachine = machine;

        // 配置碰撞体
        ConfigureCollider();

        // 初始化可视化效果
        InitializeVisualization();
    }

    /// <summary>
    /// 配置球形碰撞体用于检测范围
    /// </summary>
    private void ConfigureCollider()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }

        collider.radius = damageRange;
        collider.isTrigger = true; // 设置为触发器，避免物理碰撞
        collider.center = Vector3.zero; // 确保中心点在机器中心
    }

    /// <summary>
    /// 初始化攻击范围可视化
    /// </summary>
    private void InitializeVisualization()
    {
        // 确保只有一个可视化组件
        if (GetComponent<AttackMachineVisualizer>() == null)
        {
            var visualizer = gameObject.AddComponent<AttackMachineVisualizer>();
            visualizer.Initialize(this);
        }
    }

    /// <summary>
    /// 当其他碰撞体进入范围时触发
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 可以在此添加进入范围时的特效或音效
        // Debug.Log($"物体进入攻击范围: {other.name}");
    }

    /// <summary>
    /// 当其他碰撞体离开范围时触发
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // 可以在此添加离开范围时的特效或音效
        // Debug.Log($"物体离开攻击范围: {other.name}");
    }

    /// <summary>
    /// 在Inspector中修改值时调用
    /// </summary>
    private void OnValidate()
    {
        // 实时更新碰撞体大小
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.radius = damageRange;
        }
    }
}
