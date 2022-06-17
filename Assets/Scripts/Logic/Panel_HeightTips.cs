using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Panel_HeightTips : UIBase
{
    private const float MinHeight = 100f;
    private const float MaxHeight = 500f;

    private bool isExpand = false;
    private RectTransform expandBtn;
    private RectTransform rectMask;
    private RectTransform bg;
    private RectTransform label;
    private Button maskBtn;
    protected override void Awake()
    {
        base.Awake();
        expandBtn = transform.Find("ExpandBtn").GetComponent<RectTransform>();
        rectMask = transform.Find("RectMask").GetComponent<RectTransform>();
        bg = transform.Find("RectMask/BG").GetComponent<RectTransform>();
        label = transform.Find("RectMask/Label").GetComponent<RectTransform>();
        maskBtn = transform.Find("Mask").RequireComponent<Button>();
        expandBtn.RequireComponent<Button>().AddListener(ExpandBtnClick);
        maskBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.HeightTips);
            UIManager.OpenUI<Panel_Game>(UIPanel.Game);
        });
    }

    public override void Open()
    {
        base.Open();
        rectMask.sizeDelta = new Vector2(rectMask.sizeDelta.x, MinHeight);
        expandBtn.localPosition = new Vector3(expandBtn.localPosition.x, -MinHeight / 2f - expandBtn.sizeDelta.y / 2f, 0);
        bg.sizeDelta = new Vector2(bg.sizeDelta.x, MaxHeight);
        label.sizeDelta = new Vector2(label.sizeDelta.x, MaxHeight);
        expandBtn.Find("icon").GetComponent<Image>().sprite = Loader.LoadSprite("arrow_down");
        label.RequireComponent<Text>().text = DatabaseMgr.GetHeightLabel();
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }

    #region 展开按钮功能
    private void ExpandBtnClick()
    {
        if (!isExpand)
        {
            Expand();
            isExpand = true;
            expandBtn.Find("icon").GetComponent<Image>().sprite = Loader.LoadSprite("arrow_up");
        }
        else
        {
            Collapse();
            isExpand = false;
            expandBtn.Find("icon").GetComponent<Image>().sprite = Loader.LoadSprite("arrow_down");
        }
    }

    void Expand()
    {
        DOTween.To((t) =>
        {
            Vector2 sizeDetal = rectMask.sizeDelta;
            sizeDetal.Set(sizeDetal.x, MinHeight + (MaxHeight - MinHeight) * t);
            rectMask.sizeDelta = sizeDetal;

            Vector3 pos = expandBtn.localPosition;
            pos.Set(pos.x, -sizeDetal.y / 2f - expandBtn.sizeDelta.y / 2f, 0);
            expandBtn.localPosition = pos;
        }, 0, 1, 0.25f);
    }

    void Collapse()
    {
        DOTween.To((t) =>
        {
            Vector2 sizeDetal = rectMask.sizeDelta;
            sizeDetal.Set(sizeDetal.x, MaxHeight - (MaxHeight - MinHeight) * t);
            rectMask.sizeDelta = sizeDetal;

            Vector3 pos = expandBtn.localPosition;
            pos.Set(pos.x, -sizeDetal.y / 2f - expandBtn.sizeDelta.y / 2f, 0);
            expandBtn.localPosition = pos;
        }, 0, 1, 0.25f);
    }
    #endregion
}
