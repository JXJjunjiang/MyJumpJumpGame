using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MoreInfo : UIBase
{
    private Button maskBtn;
    protected override void Awake()
    {
        base.Awake();
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        maskBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.MoreInfo);
        });
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
