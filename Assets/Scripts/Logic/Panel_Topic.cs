using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Topic : UIBase
{
    private Button maskBtn;
    private Transform characterRoot, enviromentRoot;
    private HorizontalScroll characterScroll, enviromentScroll;

    protected override void Awake()
    {
        base.Awake();
        characterRoot = transform.Find("CharacterSelect/SV");
        enviromentRoot = transform.Find("EnviromentSelect/SV");
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        maskBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Topic);
        });
    }

    public override void Open()
    {
        base.Open();
        InitCharacterScroll();
        InitEnviromentScroll();
    }

    public override void Close()
    {
        base.Close();
        characterScroll.UnInit();
        enviromentScroll.UnInit();
        DestroyImmediate(gameObject);
    }

    private void InitCharacterScroll()
    {
        characterScroll = characterRoot.RequireComponent<HorizontalScroll>();
        List<int> characters = DatabaseMgr.Inst.GetCharacters();
        List<ScrollItem> scrollItems = new List<ScrollItem>();
        for (int i = 0; i < characters.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(Loader.LoadUI("ScrollItem"),characterRoot);
            ScrollItem scrollItem = obj.RequireComponent<ScrollItem>();
            scrollItem.Init(characters[i], Loader.LoadSprite("Character_Sprite" + characters[i]));
            scrollItems.Add(scrollItem);
        }
        characterScroll.Init(scrollItems.ToArray());
    }

    private void InitEnviromentScroll()
    {
        enviromentScroll = enviromentRoot.RequireComponent<HorizontalScroll>();
        List<int> enviroments = DatabaseMgr.Inst.GetEnviroments();
        List<ScrollItem> scrollItems = new List<ScrollItem>();
        for (int i = 0; i < enviroments.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(Loader.LoadUI("ScrollItem"),enviromentRoot);
            ScrollItem scrollItem = obj.RequireComponent<ScrollItem>();
            scrollItem.Init(enviroments[i], Loader.LoadSprite("Enviroment_Sprite" + enviroments[i]));
            scrollItems.Add(scrollItem);
        }
        enviromentScroll.Init(scrollItems.ToArray());
    }
}
