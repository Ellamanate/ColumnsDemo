using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[AddComponentMenu("ButtonExtension")]
public class ButtonExtension : Button
{
    public UnityEvent OnDown => onDown;
    public UnityEvent OnUp => onUp;

    private UnityEvent onDown = new UnityEvent();
    private UnityEvent onUp = new UnityEvent();

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onUp.Invoke();
    }
}