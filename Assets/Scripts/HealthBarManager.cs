using UnityEngine;
using Peque;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;
    public GameObject healthBarPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateHealthBarForItem(Item item, Transform target)
    {
        if (!healthBarPrefab || item == null || target == null) return;

        // 在物品正上方创建血条
        Vector3 position = target.position + new Vector3(0, 0, 0);
        GameObject healthBar = Instantiate(
            healthBarPrefab,
            position,
            Quaternion.identity,
            target // 设置为物品的子对象
        );

        // 初始化血条控制器
        HealthBar controller = healthBar.GetComponent<HealthBar>();
        if (controller != null)
        {
            controller.Initialize(item, target);
        }

        // 关联到物品
        item.healthBar = healthBar;
    }

    public void DestroyHealthBar(Item item)
    {
        if (item != null && item.healthBar != null)
        {
            Destroy(item.healthBar);
            item.healthBar = null;
        }
    }
}