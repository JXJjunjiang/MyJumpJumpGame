using System.Collections;
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
        canvasGroup.SetFade(0);
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 0f, 1f, 0.5f).onComplete = () => { UIManager.OpenUI<Panel_Main>(UIPanel.Main);  UIManager.CloseUI(UIPanel.EnterGame);};
    }

    public override void Close()
    {
        canvasGroup.SetFade(1);
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 1f, 0f, 0.5f).onComplete = () => { UIManager.CanTouch = true; DestroyImmediate(gameObject); };
    }
}
