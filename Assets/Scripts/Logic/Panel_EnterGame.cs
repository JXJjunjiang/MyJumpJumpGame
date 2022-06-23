﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Panel_EnterGame : UIBase
{
    private CanvasGroup canvasGroup;
    protected override void Awake()
    {
        base.Awake();
        canvasGroup = gameObject.RequireComponent<CanvasGroup>();
    }

    public override void Open()
    {
        canvasGroup.alpha = 0;
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 0f, 1f, 0.5f).onComplete = () => { UIMgr.OpenUI<Panel_Main>(UIPanel.Main);  UIMgr.CloseUI(UIPanel.EnterGame);};
    }

    public override void Close()
    {
        canvasGroup.alpha = 1;
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 1f, 0f, 0.5f).onComplete = () => { UIMgr.Inst.CanTouch = true; DestroyImmediate(gameObject); };
    }
}
