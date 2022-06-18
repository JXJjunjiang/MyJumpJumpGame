using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalScroll : MonoBehaviour
{
    private List<ScrollItem> items;
    public void Init(params ScrollItem[] items)
    {
        this.items = new List<ScrollItem>(items);
    }

    public void UnInit()
    {

    }

    public void Select(int id)
    {
        foreach (var scroll in items)
        {
            if (scroll.id==id)
            {
                scroll.Select();
            }
            else
            {
                scroll.DeSelect();
            }
        }
    }

    public void LeftMove()
    {

    }

    public void RightMove()
    {

    }
}
