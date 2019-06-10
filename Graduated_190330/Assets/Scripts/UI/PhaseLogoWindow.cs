using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseLogoWindow : MonoBehaviour
{
    [SerializeField] Text logoText;

    private const float startPosX = -800f;
    private const float EndPosX = 0f;

    public Action OnEndDirecting { get; set; }

	void Start ()
    {
		
	}

    void ShowPhaseLogoWindow()
    {

    }

    IEnumerator ShowLogo()
    {
        if (logoText == null)
            yield break;

        Vector3 textPos = logoText.rectTransform.localPosition ;
        float startTime = 0f;

        // 나타나기
        bool isEndDirectiong = false;
        while (!isEndDirectiong)
        {
            startTime += Time.deltaTime / 3f;
            textPos.x = Mathf.Lerp(textPos.x, EndPosX, startTime);
            logoText.rectTransform.localPosition = textPos;

            if (logoText.rectTransform.localPosition.x == EndPosX)
                isEndDirectiong = true;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // 돌아오기
        startTime = 0f;
        isEndDirectiong = false;
        while (!isEndDirectiong)
        {
            startTime += Time.deltaTime / 3f;
            textPos.x = Mathf.Lerp(textPos.x, startPosX, startTime);
            logoText.rectTransform.localPosition = textPos;

            if (logoText.rectTransform.localPosition.x == startPosX)
                isEndDirectiong = true;

            yield return null;
        }

        // 연출 끝
        OnEndDirecting.Execute();
    }

    public void ShowDirectingLogo()
    {
        StartCoroutine(ShowLogo());
    }
}