using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Panel_Game : UIBase
{
    public static TouchListener touch = null;

    private CanvasGroup canvasGroup = null;
    private Text heightLabel = null;
    private Image touchImg = null;
    private Button exitBtn = null;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        heightLabel = transform.Find("Height").GetComponent<Text>();
        touchImg = transform.Find("TouchImg").GetComponent<Image>();
        touch = touchImg.transform.RequireComponent<TouchListener>();
        exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        exitBtn.AddListener(() =>
        {
            GameMgr.Inst.GameEnd();
            UIMgr.CloseUI(UIPanelType.Game);
            UIMgr.OpenUI<Panel_Main>(UIPanelType.Main);
        });
    }
    public override void Open()
    {
        base.Open();
        EventHandler.HeightTween_Listener += HeightNumberTween;
        canvasGroup.alpha = 0;
        heightLabel.text = string.Format("{0}m", DatabaseMgr.Inst.Height);
        UIMgr.Inst.CanTouch = false;
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 0f, 1f, 0.3f).onComplete = () => UIMgr.Inst.CanTouch = true;
    }

    public override void Close()
    {
        base.Close();
        EventHandler.HeightTween_Listener -= HeightNumberTween;
        DestroyImmediate(this.gameObject);
    }

    private void HeightNumberTween(int plusScore)
    {
        int num = DatabaseMgr.Inst.Height + plusScore;
        DatabaseMgr.Inst.Height = num;
        heightLabel.text = string.Format("{0}m", num);
        Sequence seq = DOTween.Sequence();
        seq.Append(heightLabel.transform.DOScale(1.3f, 0.2f));
        seq.Append(heightLabel.transform.DOScale(1f, 0.2f));
        seq.Play();
    }

    private void OnDisable()
    {
        EventHandler.HeightTween_Listener -= HeightNumberTween;
    }
}
