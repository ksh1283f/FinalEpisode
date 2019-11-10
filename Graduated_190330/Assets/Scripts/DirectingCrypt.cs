using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectingCrypt : MonoBehaviour
{
    [SerializeField] Transform effectTrans;
    [SerializeField] Transform soulTrans;
    [SerializeField] Transform endPosTrans;
    Coroutine coroutine;
    public bool IsDirectingEnd { get; private set; }

    void Start()
    {
        IsDirectingEnd = false;
    }

    public void StartDirecting(float duration)
    {
        if (coroutine == null)
            coroutine = StartCoroutine(directingUp(duration));
    }

    IEnumerator directingUp(float duration)
    {
        Vector3 cryptPos = transform.position;
        float startTime = 0f;
        while (cryptPos.y < endPosTrans.position.y)
        {
            startTime += Time.deltaTime / duration;
            cryptPos.y = Mathf.Lerp(cryptPos.y, endPosTrans.position.y, startTime);
            transform.position = cryptPos;

            yield return null;
        }

        if(effectTrans != null)
            effectTrans.gameObject.SetActive(true);

        if(soulTrans != null)
            soulTrans.gameObject.SetActive(true);

        IsDirectingEnd = true;
    }
}
