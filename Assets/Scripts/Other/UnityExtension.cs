using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class UnityExtension
{
    public static T RequireComponent<T>(this GameObject obj) where T : Behaviour
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
    public static T RequireComponent<T>(this Transform trs) where T : Behaviour
    {
        T component = trs.GetComponent<T>();
        if (component == null)
        {
            component = trs.gameObject.AddComponent<T>();
        }
        return component;
    }
    public static void Reset(this Transform trs)
    {
        trs.localScale = Vector3.one;
        trs.localPosition = Vector3.zero;
        trs.localRotation = Quaternion.identity;
    }

    public static void Reset(this RectTransform trs)
    {
        trs.localScale = Vector3.one;
        trs.localPosition = Vector3.zero;
        trs.localRotation = Quaternion.identity;
        trs.sizeDelta = new Vector2(100f,100f);
    }

    public static void SetFade(this Text text,float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    public static void SetFade(this Image img,float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }

    public static void AddListener(this Button btn,Action callback)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => callback?.Invoke());
    }

    public static Vector3 World2UIPos(Vector3 worldPos,RectTransform uiParent)
    {
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 uiPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, screenPos, UIManager.UICamera, out uiPos);
        return uiPos;
    }

    public static TransformValue CopyTranformPosition(this Transform trs)
    {
        return new TransformValue(trs.localPosition, trs.localRotation, trs.localScale);
    }

    public static RectTransformValue CopyRectTransformValue(this RectTransform trs)
    {
        return new RectTransformValue(trs.localPosition, trs.localRotation, trs.localScale, trs.sizeDelta);
    }
}

public struct TransformValue
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public TransformValue(Vector3 _position,Quaternion _rotation,Vector3 _scale)
    {
        this.position = _position;
        this.rotation = _rotation;
        this.scale = _scale;
    }
}

public struct RectTransformValue
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector2 sizeDetal;
    public RectTransformValue(Vector3 _position,Quaternion _rotation,Vector3 _scale,Vector2 _sizeDetal)
    {
        this.position = _position;
        this.rotation = _rotation;
        this.scale = _scale;
        this.sizeDetal = _sizeDetal;
    }
}
