using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pop_Fail : UIBase
{
    private const string label = "最高分:{0}";

    private Text showText = null;
    private Button admitBtn = null;
    
    protected override void Awake()
    {
        base.Awake();
        showText = transform.Find("Label").RequireComponent<Text>();
        admitBtn = transform.Find("AdmitBtn").RequireComponent<Button>();
        admitBtn.AddListener(() =>
        {
            UIMgr.CloseUI(UIPanelType.Fail);
            UIMgr.CloseUI(UIPanelType.Game);
            UIMgr.OpenUI<Panel_Main>(UIPanelType.Main);
            GameMgr.Inst.GameEnd();
        });
    }

    public override void Open()
    {
        base.Open();
        showText.text = string.Format(label, DatabaseMgr.Inst.Height);
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
