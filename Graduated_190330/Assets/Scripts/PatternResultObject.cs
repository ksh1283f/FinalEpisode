using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternResultObject : MonoBehaviour, IFloatObject
{
    private float fadeDuration = 5f;
    private E_UserSkillType userSkillType;
    [SerializeField] SpriteRenderer sr;

    public void Release()
    {
        Destroy(gameObject);
    }

    public void Directing()
    {
        StartCoroutine(ShowPatternResult());
    }

    IEnumerator ShowPatternResult()
    {
        // x좌표는 일정범위에서 랜덤으로 계산
        // 현재 위치 -> 조금 위쪽 위치
        float targetAlpha = 1f;
        float startTime = 0f;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(sr.color.a, targetAlpha, startTime));
        while ((targetAlpha - sr.color.a) > 0f)
        {
            startTime += Time.deltaTime / fadeDuration;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(sr.color.a, targetAlpha, startTime));

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Release();
    }

    public void SetData(string data)
    {
        if (sr == null)
        {
            Debug.LogError("image is null");
            return;
        }

        if (string.IsNullOrEmpty(data))
        {
            Debug.LogError("data path is null or empty");
            return;
        }

        sr.sprite = Resources.Load<Sprite>(data);
        Color initColor = sr.color;
        initColor.a = 0f;
        sr.color = initColor;
    }
}
