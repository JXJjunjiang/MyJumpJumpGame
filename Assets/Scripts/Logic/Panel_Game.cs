using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Panel_Game : UIBase
{
    private CanvasGroup canvasGroup;
    private Text score, moveScore;
    private Button exitBtn;
    private const string scoreText = "Score:{0}";
    private const string plusSocreText = "+{0}";

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        score = transform.Find("Score").GetComponent<Text>();
        moveScore = transform.Find("MoveScore").GetComponent<Text>();
        exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        exitBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Game);
        });
        score.text = string.Format(scoreText, 0);
        moveScore.gameObject.SetActive(false);

        EventHandler.ScorePlus_Listener += GetScore;
        EventHandler.ScoreTween_Listener += ScoreNumberTween;
    }
    public override void Open()
    {
        base.Open();
        canvasGroup.SetFade(0);
        UIManager.CanTouch = false;
        DOTween.To((t) =>
        {
            canvasGroup.alpha = t;
        }, 0f, 1f, 0.3f).onComplete = () => UIManager.CanTouch = true;
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }

    private void GetScore(Transform player)
    {
        Vector2 scorePos = UnityExtension.World2UIPos(player.position, GetComponent<RectTransform>());
        Vector2 offset = new Vector2(40, 40);
        scorePos += offset;
        moveScore.gameObject.SetActive(true);
        moveScore.rectTransform.localPosition = scorePos;
        moveScore.SetFade(1);
        DOTween.To((t) =>
        {
            moveScore.rectTransform.localPosition = new Vector2(scorePos.x, scorePos.y + 40f * t);
            moveScore.SetFade(moveScore.color.a - 1 * t);
        }, 0f, 1f, 0.5f).onComplete = () => moveScore.gameObject.SetActive(false);
    }

    private void ScoreNumberTween(int plusScore)
    {
        int nowSocre = DatabaseMgr.Score;
        DOTween.To((t) =>
        {
            score.text = string.Format(scoreText, Mathf.FloorToInt(t));
        }, nowSocre, nowSocre+plusScore, 0.3f);
        DatabaseMgr.Score = nowSocre+plusScore;
    }

    private void OnDisable()
    {
        EventHandler.ScorePlus_Listener -= GetScore;
        EventHandler.ScoreTween_Listener -= ScoreNumberTween;
    }
}
