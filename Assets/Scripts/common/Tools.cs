using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Tools
{
    private static Dictionary<string,Sprite> icons=new Dictionary<string, Sprite>();

    public static DirectionHelper directionHelper = new DirectionHelper();

    public static DirectionHelper DirectionHelper
    {
        get => directionHelper;
    }

    public static void SetIcon(this UnityEngine.UI.Image img,string res)
    {
         if (!icons.ContainsKey(res))
         {
             icons[res] = Resources.Load<Sprite>($"Icon/{res}");
         }
         img.sprite = icons[res];
    }

    public static void ScreenPointToRay2D(Camera cam, Vector2 mousePos,System.Action<Collider2D> callback)
    {
        Vector3 worldPos=cam.ScreenToWorldPoint(mousePos);
        Collider2D col = Physics2D.OverlapPoint(worldPos);
        callback?.Invoke(col);    
    }

    public static Collider2D ScreenPointToRay2D(Camera cam,Vector2 mousePos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Collider2D col = Physics2D.OverlapPoint(worldPos);
        return col;
    }

    /// <summary>
    /// 获取鼠标在世界空间的位置
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }
}
