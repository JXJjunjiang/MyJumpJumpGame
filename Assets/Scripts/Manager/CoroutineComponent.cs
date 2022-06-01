using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineComponent : MonoBehaviour
{
    public void CreateCoroutine(IEnumerator coroutine,string tag)
    {
        StartCoroutine(RunCoroutine(coroutine, tag));
    }

    IEnumerator RunCoroutine(IEnumerator coroutine,string tag)
    {
        yield return StartCoroutine(coroutine);
        CoroutineManager.Inst.DestroyCoroutine(tag);
    }
}
