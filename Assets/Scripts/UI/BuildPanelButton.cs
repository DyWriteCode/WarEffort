using UnityEngine;
using UnityEngine.EventSystems;

public class BuildPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public Machine.Type type;

    public void OnPointerClick(PointerEventData eventData) {
        BuildPanel.Instance.SelectMachine(type);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        BuildPanel.Instance.ShowMachineInfo(type);
    }
}
