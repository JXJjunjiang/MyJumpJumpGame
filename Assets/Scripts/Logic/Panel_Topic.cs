using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Topic : UIBase
{
    private Button maskBtn;
    private Transform characterRoot, enviromentRoot;
    private HorizontalScroll characterScroll, enviromentScroll;
    private Coroutine characterCor, enviromentCor;
    private List<GameObject> characterSelectBorders;
    private List<GameObject> enviromentSelectBorders;

    protected override void Awake()
    {
        base.Awake();
        characterRoot = transform.Find("CharacterSelect/SV");
        enviromentRoot = transform.Find("EnviromentSelect/SV");
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        characterSelectBorders = new List<GameObject>();
        enviromentSelectBorders = new List<GameObject>();
        maskBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Topic);
        });
    }

    public override void Open()
    {
        base.Open();
        characterCor = StartCoroutine(InitCharacterScroll());
        enviromentCor = StartCoroutine(InitEnviromentScroll());
    }

    public override void Close()
    {
        base.Close();
        if (characterCor!=null)
        {
            StopCoroutine(characterCor);
        }
        if (enviromentCor!=null)
        {
            StopCoroutine(enviromentCor);
        }
        characterScroll.UnInit();
        enviromentScroll.UnInit();
        DestroyImmediate(gameObject);
    }

    private IEnumerator InitCharacterScroll()
    {
        characterScroll = characterRoot.RequireComponent<HorizontalScroll>();
        List<int> ids = DatabaseMgr.Inst.GetCharacters();
        List<Transform> objs = new List<Transform>();
        GameObject prefab = Loader.LoadUI("ScrollItem");
        float posX = 0;
        float itemWidth = prefab.GetComponent<RectTransform>().sizeDelta.x;
        float gap = 5f;
        for (int i = 0; i < ids.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(prefab, characterRoot);
            SetScrollItem(obj.transform, ids[i], posX, ScrollType.Character);
            posX += itemWidth + gap;
            yield return new WaitForSecondsRealtime(0.1f);
            objs.Add(obj.transform);
        }
        KeyValuePair<int, Transform>[] items = new KeyValuePair<int, Transform>[ids.Count];
        for (int i = 0; i < ids.Count; i++)
        {
            items[i] = new KeyValuePair<int, Transform>(ids[i], objs[i]);
        }
        characterScroll.Init(5f, itemWidth, items);

        SelectTarget(items[DatabaseMgr.CharacterID].Value, ScrollType.Character);

        Button btnLeft = transform.Find("CharacterSelect/LeftBtn").GetComponent<Button>();
        btnLeft.AddListener(() => characterScroll.LeftMove());
        Button btnRight = transform.Find("CharacterSelect/RightBtn").GetComponent<Button>();
        btnRight.AddListener(() => characterScroll.RightMove());
        characterCor = null;
    }

    private IEnumerator InitEnviromentScroll()
    {
        enviromentScroll = enviromentRoot.RequireComponent<HorizontalScroll>();
        List<int> ids = DatabaseMgr.Inst.GetEnviroments();
        List<Transform> objs = new List<Transform>();
        GameObject prefab = Loader.LoadUI("ScrollItem");
        float posX = 0;
        float itemWidth = prefab.GetComponent<RectTransform>().sizeDelta.x;
        float gap = 5f;
        for (int i = 0; i < ids.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(prefab, enviromentRoot);
            SetScrollItem(obj.transform, ids[i], posX, ScrollType.Enviroment);
            posX += itemWidth + gap;
            yield return new WaitForSecondsRealtime(0.1f);
            objs.Add(obj.transform);
        }
        KeyValuePair<int, Transform>[] items = new KeyValuePair<int, Transform>[ids.Count];
        for (int i = 0; i < ids.Count; i++)
        {
            items[i] = new KeyValuePair<int, Transform>(ids[i], objs[i]);
        }
        enviromentScroll.Init(5f, itemWidth, items);

        SelectTarget(items[DatabaseMgr.EnviromentID].Value, ScrollType.Enviroment);

        Button btnLeft = transform.Find("EnviromentSelect/LeftBtn").GetComponent<Button>();
        btnLeft.AddListener(() => enviromentScroll.LeftMove());
        Button btnRight = transform.Find("EnviromentSelect/RightBtn").GetComponent<Button>();
        btnRight.AddListener(() => enviromentScroll.RightMove());
        enviromentCor = null;
    }

    void SetScrollItem(Transform trs,int id,float posX,ScrollType type)
    {
        Button btn = trs.GetComponent<Button>();
        Image img = trs.GetComponent<Image>();
        GameObject border = trs.Find("SelectBorder").gameObject;

        Sprite sprite = null;
        if (type==ScrollType.Character)
        {
            characterSelectBorders.Add(border);
            sprite = Loader.LoadSprite("CharacterSprite_" + id);
            btn.AddListener(() => 
            {
                DatabaseMgr.CharacterID = id;
                SelectTarget(trs, type);
            });
        }
        else
        {
            enviromentSelectBorders.Add(border);
            sprite = Loader.LoadSprite("EnviromentSprite_" + id);
            btn.AddListener(() => 
            {
                DatabaseMgr.EnviromentID = id;
                SelectTarget(trs,type);
            });
        }
        img.sprite = sprite;
        trs.localPosition = new Vector3(posX, 0, 0);
    }

    void SelectTarget(Transform trs,ScrollType type)
    {
        if (type==ScrollType.Character)
        {
            foreach (var b in characterSelectBorders)
            {
                b.SetActive(false);
            }
        }
        else
        {
            foreach (var b in enviromentSelectBorders)
            {
                b.SetActive(false);
            }
        }
        trs.Find("SelectBorder").gameObject.SetActive(true);
    }
}
