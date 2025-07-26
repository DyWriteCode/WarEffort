using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Peque;

public class DebugPanel : MonoBehaviour
{
    public Text objText;
    public Text pollutionText;

    public void updateCount () {
        if (objText != null)
        {
            objText.text = $"Object Number : {GameObject.FindObjectsOfType<SpriteRenderer>().Length.ToString()}";
        }
    }

    public void updatePollution()
    {
        if (pollutionText != null)
        {
            pollutionText.text = $"PollutionLevel : {GameGrid.Instance.GetPollutionPercentage()}%";
        }
    }

    public void Clear ()
    {
        GameGrid.Instance.ClearAllPlayerBuildings();
    }

    public void Update()
    {
        updateCount();
        updatePollution();
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