using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(UnityEngine.UI.Image))]
public class TouchListener : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public delegate void Pointer();

    private Image _img = null;
    public Pointer PointerDown = null;
    public Pointer PointerUp = null;

    private void Awake()
    {
        _img = transform.RequireComponent<Image>();
        _img.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp?.Invoke();
    }
}
