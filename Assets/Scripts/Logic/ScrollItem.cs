using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollItem : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IEndDragHandler
{
    private HorizontalScroll scroll;
    public int id;
    private GameObject border;

    public void Init(int id,Sprite sprite)
    {
        this.id = id;
        border = transform.Find("SelectBorder").gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        scroll.Select(id);
    }

    public void Select()
    {
        DatabaseMgr.CharacterID = id;
        border.SetActive(true);
    }

    public void DeSelect()
    {
        border.SetActive(false);
    }
}
