using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningDirecting : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Image allMap;
    [SerializeField] Image A1Map;
    [SerializeField] Image A2Map;
    [SerializeField] Image A3Map;
    [SerializeField] Image A4Map;
    [SerializeField] Button btnSkip;
    List<Image> mapList = new List<Image>();

    void Start()
    {
        DontDestroyOnLoad(this);
        SoundManager.Instance.SceneType = E_SceneType.Opening;
        if(btnSkip != null)
            btnSkip.onClick.AddListener(OnClickedBtnSkip);
        if(dialogue == null)
            return;

        dialogue.SetDialogueDic();
        mapList.Add(A1Map);
        mapList.Add(A2Map);
        mapList.Add(A3Map);
        mapList.Add(A4Map);

        // start directing
        StartCoroutine(SceneDirecting());
    }
    
    // 연출에서 인터랙션은 스킵버튼만
    IEnumerator SceneDirecting()
    {
        #region  Check null
        if(dialogue == null)
        {
            Debug.LogError("dialogue is null");
            yield break;
        }

        if(allMap == null)
        {
            Debug.LogError("allMap is null");
            yield break;
        }

        if(A1Map == null)
        {
            Debug.LogError("a1 map is null");
            yield break;
        }

        if(A2Map == null)
        {
            Debug.LogError("a2 map is null");
            yield break;
        }

        if(A3Map == null)
        {
            Debug.LogError("a3 map is null");
            yield break;
        }

        if(A4Map == null)
        {
            Debug.LogError("A4Map is null");
            yield break;
        }
        #endregion

        // Direction initialize
        for (int i = 0; i < mapList.Count; i++)
            mapList[i].gameObject.SetActive(false);

        // (FI)
        dialogue.IsCutSceneDirecting = true;
        Color tempColor = allMap.color;
        float startTime = 0f;
        tempColor.a = Mathf.Lerp(0,1, startTime);
        while (tempColor.a < 1f)
        {
            startTime += Time.deltaTime/1f;
            tempColor.a = Mathf.Lerp(0,1,startTime);
            allMap.color = tempColor;

            yield return null;
        }

        // 0. A대륙에 대한 설명
        // show dialogue
        
        yield return new WaitForSeconds(1f);
        dialogue.IsCutSceneDirecting = false;
        dialogue.ShowDialogue(dialogue.dialogueDic[dialogue.DialogueDicIndex]);
        while (!dialogue.IsTypingEnd)
            yield return null;  // 타이핑 연출이 끝날때까지 대기
        
        dialogue.IsCutSceneDirecting = true;
        yield return new WaitForSeconds(1f);

        // (FO)
        
        tempColor = allMap.color;
        startTime = 0f;
        tempColor.a = Mathf.Lerp(1,0, startTime);
        while (tempColor.a > 0f)
        {
            startTime += Time.deltaTime/1f;
            tempColor.a = Mathf.Lerp(1,0,startTime);
            allMap.color = tempColor;

            yield return null;
        }
        
        // 1초뒤
        yield return new WaitForSeconds(1f);
        dialogue.ClearText();

        for (int i = 0; i < mapList.Count; i++)
        {
            yield return new WaitForSeconds(1f);
            if(mapList[i].gameObject.activeSelf != true)
                mapList[i].gameObject.SetActive(true);
            // 0. (FI)
            tempColor = mapList[i].color;
            startTime = 0f;
            tempColor.a = Mathf.Lerp(0,1, startTime);
            while (tempColor.a < 1f)
            {
                startTime += Time.deltaTime/1f;
                tempColor.a = Mathf.Lerp(0,1,startTime);
                mapList[i].color = tempColor;

                yield return null;
            }
            // 1. 지역별 설명(설명이 나올 때 각 지역별 이미지를 띄우면서(이미지가 있는 쪽에서 가운데로 오도록 이동) 설명, 마지막에 가운데 지역 설명)

            // 1-1. 각 맵 이미지들이 중앙으로 이동하는 연출
            // 1-2. 다이얼로그 타이핑 연출
            dialogue.IsCutSceneDirecting = false;
            dialogue.ShowDialogue(dialogue.dialogueDic[dialogue.DialogueDicIndex]);
            while (!dialogue.IsTypingEnd)
                yield return null;  // 타이핑 연출이 끝날때까지 대기

            // 1-3. (FO) 
            dialogue.IsCutSceneDirecting = true;
            yield return new WaitForSeconds(1f);
            tempColor = mapList[i].color;
            startTime = 0f;
            tempColor.a = Mathf.Lerp(1,0, startTime);
            while (tempColor.a > 0f)
            {
                startTime += Time.deltaTime/1f;
                tempColor.a = Mathf.Lerp(1,0,startTime);
                mapList[i].color = tempColor;

                yield return null;
            }
        }

        // 2. 게임의 목표 설명(자세히x)    
        dialogue.IsCutSceneDirecting = false;
        dialogue.ShowDialogue(dialogue.dialogueDic[dialogue.DialogueDicIndex]);
        while (!dialogue.IsTypingEnd)
            yield return null;  // 타이핑 연출이 끝날때까지 대기

        dialogue.IsCutSceneDirecting = true;
        UserManager.Instance.ao  = SceneManager.LoadSceneAsync("NewLobby");
        while (!UserManager.Instance.ao.isDone)
            yield return null;

        UserManager.Instance.UserSituation = E_UserSituation.LoadingLobby;
        Destroy(gameObject);

    }

    void OnClickedBtnSkip()
    {
        SoundManager.Instance.PlayButtonSound();
        StopCoroutine(SceneDirecting());
        StartCoroutine(GoToLobby());
    }

    IEnumerator GoToLobby()
    {
        UserManager.Instance.ao  = SceneManager.LoadSceneAsync("NewLobby");
        while(!UserManager.Instance.ao.isDone)
            yield return null;
        UserManager.Instance.UserSituation = E_UserSituation.LoadingLobby;
        Destroy(gameObject);
    }
}
