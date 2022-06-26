﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HorizontalScroll
{
    public enum ScrollType
    {
        Character,
        Enviroment
    }

    private int[] keys = null;
    private float gap = 0f;
    private float itemWidth = 0f;

    private Dictionary<int, Transform> scroll = null;
    private Sequence seq = null;

    public HorizontalScroll(float gap, float itemWidth, params KeyValuePair<int,Transform>[] items)
    {
        this.gap = gap;
        this.itemWidth = itemWidth;
        if (scroll==null)
        {
            scroll = new Dictionary<int, Transform>();
        }
        scroll.Clear();
        keys = new int[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            scroll.Add(items[i].Key, items[i].Value);
            keys[i] = items[i].Key;
        }
    }

    public void UnInit()
    {
        keys = null;
        scroll.Clear();
        DOTween.Kill(seq);
    }

    public void LeftMove()
    {
        int first = keys[0];

        seq = DOTween.Sequence();
        float posX = 0 - itemWidth - gap;
        for (int i = 0; i < keys.Length; i++)
        {
            seq.Insert(0, scroll[keys[i]].DOLocalMoveX(posX, 0.3f));
            posX += itemWidth + gap;
        }
        UIMgr.Inst.CanTouch = false;
        seq.Play().onComplete = () => 
        {
            UIMgr.Inst.CanTouch = true;
            scroll[first].localPosition = new Vector3(posX, 0, 0);
        };
        for (int i = 0; i < keys.Length - 1; i++)
        {
            keys[i] = keys[i + 1];
        }
        keys[keys.Length - 1] = first;
    }

    public void RightMove()
    {
        int last = keys[keys.Length - 1];
        seq = DOTween.Sequence();
        float posX = scroll[last].localPosition.x + itemWidth + gap;
        UIMgr.Inst.CanTouch = false;
        scroll[last].localPosition = new Vector3(0 - itemWidth - gap, 0, 0);
        seq.Insert(0, scroll[last].DOLocalMoveX(0, 0.3f));
        for (int i = keys.Length-2; i >= 0; i--)
        {
            seq.Insert(0, scroll[keys[i]].DOLocalMoveX(posX, 0.3f));
            posX -= itemWidth + gap;
        }
        seq.Play().onComplete = () => { UIMgr.Inst.CanTouch = true; };
        for (int i = keys.Length - 1; i > 0; i--)
        {
            keys[i] = keys[i - 1];
        }
        keys[0] = last;
    }
}
