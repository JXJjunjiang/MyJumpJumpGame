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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, screenPos, UIManager.Inst.UICamera, out uiPos);
        return uiPos;
    }

    public static TransformValue GetTranformValue(this Transform trs)
    {
        return new TransformValue(trs.localPosition, trs.localRotation, trs.localScale);
    }

    public static RectTransformValue GetRectTransformValue(this RectTransform trs)
    {
        return new RectTransformValue(trs.localPosition, trs.localRotation, trs.localScale, trs.sizeDelta);
    }

    public static void GetTransformValue(this Transform trs1,Transform trs2)
    {
        TransformValue tv = trs1.GetTranformValue();
        trs2.localPosition = tv.position;
        trs2.localRotation = tv.rotation;
        trs2.localScale = tv.scale;
    }

    public static void GetRectTransformValue(this RectTransform trs1, RectTransform trs2)
    {
        RectTransformValue tv = trs1.GetRectTransformValue();
        trs2.localPosition = tv.position;
        trs2.localRotation = tv.rotation;
        trs2.localScale = tv.scale;
        trs2.sizeDelta = tv.sizeDetal;
    }

    public static Vector3 GetModelSize(this MeshFilter filter)
    {
        Vector3 meshSize = filter.mesh.bounds.size;
        Vector3 localScale = filter.transform.localScale;
        return new Vector3(meshSize.x * localScale.x, meshSize.y * localScale.y, meshSize.z * localScale.z);
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
