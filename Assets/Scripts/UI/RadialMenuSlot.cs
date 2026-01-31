using UnityEngine;
using UnityEngine.EventSystems;

public class RadialMenuSlot : MonoBehaviour, ISelectHandler
{
    public Mask.MaskColor mask;

    public void OnSelect(BaseEventData eventData)
    {
        transform.GetComponentInParent<RadialMenuHandler>().UpdateSelectedMask(mask);
    }
}
