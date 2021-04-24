using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SliderExtension : Slider, IEndDragHandler
{
    public SliderEvent onDragEnd = new SliderEvent();

    public void OnEndDrag(PointerEventData eventData) => onDragEnd.Invoke(value);
}