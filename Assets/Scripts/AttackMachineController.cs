using Peque.Machines;
using Peque;
using UnityEngine;

public class AttackMachineController : MonoBehaviour
{
    [HideInInspector]
    public float damageRange = 5f;
    [HideInInspector]
    public int damageInterval = 10;
    [HideInInspector]
    public int damagePerTick = 10;

    private Machine linkedMachine;

    public void Initialize(Machine machine)
    {
        linkedMachine = machine;
        GetComponent<SphereCollider>().radius = damageRange;
        GetComponent<SphereCollider>().isTrigger = true;

        // 可视化设置
        var visualizer = gameObject.AddComponent<AttackMachineVisualizer>();
        visualizer.Initialize(this);
    }
}
