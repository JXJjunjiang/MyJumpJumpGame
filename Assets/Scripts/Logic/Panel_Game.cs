using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Panel_Game : UIBase
{
    private CanvasGroup canvasGroup;
    private Text score;
    private Button exitBtn;
    public static TouchListener touch = null;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        score = transform.Find("Score").GetComponent<Text>();
        score.text = DatabaseMgr.Score.ToString();
        exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        exitBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Game);
            UIManager.OpenUI<Panel_Main>(UIPanel.Main);
        });
        touch = transform.Find("Touch").RequireComponent<TouchListener>();
    }
    public override void Open()
    {
        base.Open();
        EventHandler.ScoreTween_Listener += ScoreNumberTween;
        canvasGroup.SetFade(0);
        score.text = DatabaseMgr.Score.ToString();
        UIManager.CanTouch = false;
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 0f, 1f, 0.3f).onComplete = () => UIManager.CanTouch = true;
    }

    public override void Close()
    {
        base.Close();
        EventHandler.ScoreTween_Listener -= ScoreNumberTween;
        DestroyImmediate(gameObject);
    }

    private void ScoreNumberTween(int plusScore)
    {
        int num = DatabaseMgr.Score + plusScore;
        DatabaseMgr.Score = num;
        score.text = num.ToString();
        Sequence seq = DOTween.Sequence();
        seq.Append(score.transform.DOScale(1.3f, 0.2f));
        seq.Append(score.transform.DOScale(1f, 0.2f));
        seq.Play();
    }
}
