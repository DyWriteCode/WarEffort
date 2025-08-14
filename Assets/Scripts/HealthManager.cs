using UnityEngine;
using FactorySystem;

public class HealthManager : BaseManager
{
    public GameObject healthBarPrefab;

    public HealthManager(GameObject healthBarPrefab)
    {
        this.healthBarPrefab = healthBarPrefab;
    }

    public void CreateHealthBarForItem(Item item, Transform target)
    {
        if (!healthBarPrefab || item == null || target == null) return;

        // 在物品正上方创建血条
        Vector3 position = target.position + new Vector3(0, 0, 0);
        GameObject healthBar = GameObject.Instantiate(
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
            item.healthBar.AddComponent<DestroyObj>().DestroyObject(0);
            item.healthBar = null;
        }
    }
}