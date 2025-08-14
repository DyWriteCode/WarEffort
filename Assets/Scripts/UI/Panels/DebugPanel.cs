using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FactorySystem;

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
            if (GameApp.PollutionManager != null)
            {
                pollutionText.text = $"PollutionLevel : {GameApp.PollutionManager.GlobalPollution}";
            }
        }
    }

    public void Clear ()
    {
        GameApp.MachineManager.ClearAllMachines();
    }

    public void Update()
    {
        updateCount();
        updatePollution();
    }

//    void OnDrawGizmos()
//    {
//        if (Application.isPlaying)
//        {
//            foreach (var item in GameGirdManager.Instance.items.Values)
//            {
//#if UNITY_EDITOR
//                Gizmos.color = Color.red;
//                Gizmos.DrawSphere(item.position, 0.2f);
//                UnityEditor.Handles.Label(item.position, item.type.ToString());
//#endif
//            }
//        }
//    }
}