using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonDisabler : MonoBehaviour ,IPointerExitHandler
{

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("On pointer exit");

        if (eventData.pointerPress != this.gameObject)
            this.gameObject.SetActive(false);
    }
}
