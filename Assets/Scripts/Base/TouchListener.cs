using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class TouchListener : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    public delegate void PointerDelegate();

    private Image _img;
    private float jugeHoldTime = 0.1f;

    public event PointerDelegate PointerDown;
    public event PointerDelegate PointerUp;

    private void Awake()
    {
        _img = transform.RequireComponent<Image>();
        _img.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
