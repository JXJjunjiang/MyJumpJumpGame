using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class TouchListener : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    private Image _img;
    private float jugeHoldTime = 0.1f;

    private bool _isDown;
    public bool IsDown
    {
        get => _isDown;
        set => _isDown = value;
    }

    private bool _isHold;
    public bool IsHold
    {
        get => _isHold;
        set => _isHold = value;
    }

    private float _pressTime;
    public float PressTime
    {
        get => _pressTime;
        set => _pressTime = value;
    }

    private void Awake()
    {
        _img = transform.RequireComponent<Image>();
        _img.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressTime = 0;
        _isHold = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    private void Update()
    {
        if (_isDown)
        {
            _pressTime += Time.unscaledDeltaTime;
            if (_pressTime >= jugeHoldTime)
            {
                _isDown = false;
                _isHold = true;
            }
        }

        if (_isHold)
        {
            _pressTime += Time.unscaledDeltaTime;
        }
    }
}
