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
        trs.sizeDelta = Vector2.zero;
    }

    public static void SetFade(this CanvasGroup canvas, float alpha)
    {
        canvas.alpha = alpha;
    }

    public static void AddListener(this Button btn,Action callback)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => callback?.Invoke());
    }
}
