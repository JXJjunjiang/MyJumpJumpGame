using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Panel_MoreInfo : UIBase
{
    private static float MinY = 0f, MaxY = 700f;

    private Button maskBtn = null;
    private RectTransform rectMask = null;

    protected override void Awake()
    {
        base.Awake();
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        rectMask = transform.Find("RectMask").GetComponent<RectTransform>();
        maskBtn.AddListener(() =>
        {
            UIMgr.CloseUI(UIPanelType.MoreInfo);
        });
        rectMask.sizeDelta = new Vector2(rectMask.sizeDelta.x, MinY);
    }

    public override void Open()
    {
        base.Open();
        UIMgr.Inst.CanTouch = false;
        DOTween.To((t) =>
        {
            rectMask.sizeDelta = new Vector2(rectMask.sizeDelta.x, MaxY * t);
        }, 0, 1, 0.3f).onComplete = () => UIMgr.Inst.CanTouch = true;
    }

    public override void Close()
    {
        base.Close();
        UIMgr.Inst.CanTouch = false;
        DOTween.To((t) =>
        {
            rectMask.sizeDelta = new Vector2(rectMask.sizeDelta.x, MaxY * t);
        }, 1, 0, 0.3f).onComplete = () => 
        {
            UIMgr.Inst.CanTouch = true;
            DestroyImmediate(gameObject);
        };
    }
}
