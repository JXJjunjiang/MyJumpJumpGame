using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void Pointer();
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class TouchListener : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private Image _img;
    public Pointer PointerDown;
    public Pointer PointerUp;

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
