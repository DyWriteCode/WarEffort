using UnityEngine;

public class DirectionHelper
{
    /// <summary>
    /// 根据鼠标移动方向获取机器朝向
    /// </summary>
    /// <param name="startPosition">起始位置（鼠标按下位置）</param>
    /// <param name="currentPosition">当前位置（鼠标当前位置）</param>
    /// <returns>机器朝向</returns>
    public static Machine.Direction GetDirectionFromMouseMovement(Vector3 startPosition, Vector3 currentPosition)
    {
        // 计算鼠标移动方向
        Vector3 mouseDirection = currentPosition - startPosition;
        mouseDirection.y = 0; // 忽略Y轴变化

        // 计算移动方向的角度（0-360度）
        float angle = Mathf.Atan2(mouseDirection.z, mouseDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        // 将角度转换为机器朝向
        return AngleToDirection(angle);
    }

    /// <summary>
    /// 将角度转换为机器朝向
    /// </summary>
    private static Machine.Direction AngleToDirection(float angle)
    {
        // 将角度标准化到0-360范围
        angle = (angle % 360 + 360) % 360;

        // 四个主要方向的阈值范围（各45度）
        const float threshold = 45f;

        // 右: 0° ±45°
        if (angle <= threshold || angle >= 360 - threshold)
            return Machine.Direction.Right;

        // 上: 90° ±45°
        if (angle > 90 - threshold && angle <= 90 + threshold)
            return Machine.Direction.Up;

        // 左: 180° ±45°
        if (angle > 180 - threshold && angle <= 180 + threshold)
            return Machine.Direction.Left;

        // 下: 270° ±45°
        if (angle > 270 - threshold && angle <= 270 + threshold)
            return Machine.Direction.Down;

        // 对角线方向处理
        return HandleDiagonalDirections(angle);
    }

    /// <summary>
    /// 处理对角线方向（45度角）
    /// </summary>
    private static Machine.Direction HandleDiagonalDirections(float angle)
    {
        // 右上 (0-90)
        if (angle > 0 && angle < 90)
            return Machine.Direction.Right; // 或根据需求返回Up

        // 左上 (90-180)
        if (angle > 90 && angle < 180)
            return Machine.Direction.Up; // 或根据需求返回Left

        // 左下 (180-270)
        if (angle > 180 && angle < 270)
            return Machine.Direction.Left; // 或根据需求返回Down

        // 右下 (270-360)
        if (angle > 270 && angle < 360)
            return Machine.Direction.Down; // 或根据需求返回Right

        // 默认返回右方向
        return Machine.Direction.Right;
    }
}