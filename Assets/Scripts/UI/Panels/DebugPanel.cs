using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Peque;

public class DebugPanel : MonoBehaviour
{
    private Text text;
    private void Awake() {
        text = GetComponent<Text>();
    }

    public void updateCount () {
        text.text = $"Object Number : {GameObject.FindObjectsOfType<SpriteRenderer>().Length.ToString()}";
    }

    public void Clear ()
    {
        GameGrid.Instance.ClearAllPlayerBuildings();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (var item in GameGrid.Instance.items.Values)
            {
#if UNITY_EDITOR
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(item.position, 0.2f);
                UnityEditor.Handles.Label(item.position, item.type.ToString());
#endif
            }
        }
    }
}