using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Topic : UIBase
{
    private List<GameObject> characterSelectBorders = null;
    private List<GameObject> enviromentSelectBorders = null;

    private Button maskBtn = null;
    private Transform characterRoot, enviromentRoot = null;
    private HorizontalScroll characterScroll = null, enviromentScroll = null;
    private Coroutine characterCor = null, enviromentCor = null;

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
            UIMgr.CloseUI(UIPanelType.Topic);
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
        characterSelectBorders?.Clear();
        enviromentSelectBorders?.Clear();
        characterScroll?.UnInit();
        enviromentScroll?.UnInit();
        DestroyImmediate(gameObject);
    }

    private IEnumerator InitCharacterScroll()
    {
        List<int> ids = DatabaseMgr.Inst.GetCharacters();
        List<Transform> objs = new List<Transform>();
        GameObject prefab = Loader.LoadUI("ScrollItem");
        float posX = 0;
        float itemWidth = prefab.GetComponent<RectTransform>().sizeDelta.x;
        float gap = 5f;
        for (int i = 0; i < ids.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(prefab, characterRoot);
            SetScrollItem(obj.transform, ids[i], posX, HorizontalScroll.ScrollType.Character);
            posX += itemWidth + gap;
            yield return new WaitForSecondsRealtime(0.1f);
            objs.Add(obj.transform);
        }
        KeyValuePair<int, Transform>[] items = new KeyValuePair<int, Transform>[ids.Count];
        for (int i = 0; i < ids.Count; i++)
        {
            items[i] = new KeyValuePair<int, Transform>(ids[i], objs[i]);
        }
        characterScroll = new HorizontalScroll(5f, itemWidth, items);

        Button btnLeft = transform.Find("CharacterSelect/LeftBtn").GetComponent<Button>();
        btnLeft.AddListener(() => characterScroll.LeftMove());
        Button btnRight = transform.Find("CharacterSelect/RightBtn").GetComponent<Button>();
        btnRight.AddListener(() => characterScroll.RightMove());
        characterCor = null;
    }

    private IEnumerator InitEnviromentScroll()
    {
        List<int> ids = DatabaseMgr.Inst.GetEnvironments();
        List<Transform> objs = new List<Transform>();
        GameObject prefab = Loader.LoadUI("ScrollItem");
        float posX = 0;
        float itemWidth = prefab.GetComponent<RectTransform>().sizeDelta.x;
        float gap = 5f;
        for (int i = 0; i < ids.Count; i++)
        {
            GameObject obj = Instantiate<GameObject>(prefab, enviromentRoot);
            SetScrollItem(obj.transform, ids[i], posX, HorizontalScroll.ScrollType.Enviroment);
            posX += itemWidth + gap;
            yield return new WaitForSecondsRealtime(0.1f);
            objs.Add(obj.transform);
        }
        KeyValuePair<int, Transform>[] items = new KeyValuePair<int, Transform>[ids.Count];
        for (int i = 0; i < ids.Count; i++)
        {
            items[i] = new KeyValuePair<int, Transform>(ids[i], objs[i]);
        }
        enviromentScroll = new HorizontalScroll(5f, itemWidth, items);

        Button btnLeft = transform.Find("EnviromentSelect/LeftBtn").GetComponent<Button>();
        btnLeft.AddListener(() => enviromentScroll.LeftMove());
        Button btnRight = transform.Find("EnviromentSelect/RightBtn").GetComponent<Button>();
        btnRight.AddListener(() => enviromentScroll.RightMove());
        enviromentCor = null;
    }

    void SetScrollItem(Transform trs,int id,float posX, HorizontalScroll.ScrollType type)
    {
        Button btn = trs.GetComponent<Button>();
        Image img = trs.GetComponent<Image>();
        GameObject border = trs.Find("SelectBorder").gameObject;

        Sprite sprite = null;
        if (type== HorizontalScroll.ScrollType.Character)
        {
            border.SetActive(id == DatabaseMgr.Inst.CharacterID);
            characterSelectBorders.Add(border);
            sprite = Loader.LoadSprite("CharacterSprite_" + id);
            btn.AddListener(() => 
            {
                DatabaseMgr.Inst.CharacterID = id;
                SelectTarget(trs, type);
            });
        }
        else
        {
            border.SetActive(id == DatabaseMgr.Inst.EnvironmentID);
            enviromentSelectBorders.Add(border);
            sprite = Loader.LoadSprite("EnviromentSprite_" + id);
            btn.AddListener(() => 
            {
                DatabaseMgr.Inst.EnvironmentID = id;
                SelectTarget(trs,type);
            });
        }
        img.sprite = sprite;
        trs.localPosition = new Vector3(posX, 0, 0);
    }

    void SelectTarget(Transform trs,HorizontalScroll.ScrollType type)
    {
        if (type== HorizontalScroll.ScrollType.Character)
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
