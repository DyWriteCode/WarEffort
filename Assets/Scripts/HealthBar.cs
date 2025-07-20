using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform target; // 跟踪的商品对象
    public Vector3 offset = new Vector3(0, 1.5f, 0); // 血条位置偏移
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 血条始终面向摄像机
        if (target)
        {
            transform.position = target.position + offset + Vector3.up * 0.5f;
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

    // 更新血条显示
    public void UpdateHealth(int currentHp, int maxHp)
    {
        float scale = (float)currentHp / maxHp;
        transform.Find("fill").localScale = new Vector3(scale, 1, 1);
    }
}