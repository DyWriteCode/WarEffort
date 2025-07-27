using UnityEngine;
using Peque;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Transform target; // 跟踪的商品对象
    public Vector3 offset = new Vector3(0, 0.5f, 0); // 血条位置偏移
    private Camera mainCamera;
    private Item item;
    private Transform fillBar;
    private Slider healthSlider;


    public void Initialize(Item item, Transform target)
    {
        this.target = target;
        this.mainCamera = Camera.main;
        this.item = item;
        fillBar = transform.Find("fill");
        healthSlider = GetComponentInChildren<Slider>();
    }

    void LateUpdate()
    {
        // 血条始终面向摄像机
        if (target)
        {
            transform.position = target.position + offset + new Vector3(0, 0, 1f);
            transform.LookAt(transform.position + mainCamera.transform.forward);
            UpdateHealthDisplay();
        }
    }

    public void SetHealth(int currentHp, int maxHp = -1)
    {
        Debug.Log(0);

        if (healthSlider == null) return;

        // 更新血量值
        item.Hp = currentHp;

        // 如果需要更新最大血量
        if (maxHp > 0)
        {
            item.maxHp = maxHp;
            healthSlider.maxValue = maxHp;
        }

        // 设置滑动条值
        healthSlider.value = currentHp;

        // 可选：根据血量比例改变颜色
        UpdateHealthColor();
    }

    private void UpdateHealthColor()
    {
        if (healthSlider == null) return;

        // 获取血量的百分比
        float healthPercentage = (float)item.Hp / item.maxHp;

        // 根据血量比例改变颜色
        healthSlider.fillRect.TryGetComponent<Image>(out Image fillImage);
        if (fillImage != null)
        {
            if (healthPercentage > 0.7f)
            {
                fillImage.color = Color.green;
            }
            else if (healthPercentage > 0.3f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }
    }

    // 更新血条显示（可选，如果需要在外部调用）
    public void UpdateHealthDisplay()
    {
        if (healthSlider != null)
        {
            healthSlider.value = item.Hp;
            healthSlider.maxValue = item.maxHp;
            UpdateHealthColor();
        }
    }
}