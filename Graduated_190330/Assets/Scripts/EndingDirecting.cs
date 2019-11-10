using System;
using System.Collections;
using System.Collections.Generic;
using Graduate.Unit;
using Graduate.Unit.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum E_EndingDirectingDataType
{
    None,
    FadeIn,
    FadeOut,
    PlayersDeathAni,
    EnemiesDeathAni,
    Dialogue,
    ObjectMoving,
} 

[Serializable]
public class EndingDirectingData
{
    //public int Id;
    public E_EndingDirectingDataType DirectingDataType;
    public GameObject Target;
    public float DirectingValue;
    public string DirectingDialogue;
}

public class EndingDirecting : MonoBehaviour
{
    [SerializeField] List<Graduate.Unit.Player.PlayerUnit> playerList = new List<Graduate.Unit.Player.PlayerUnit>();
    [SerializeField] Graduate.Unit.Enemy.EnemyUnit enemy;
    public List<EndingDirectingData> SuccessDataList = new List<EndingDirectingData>();
    public List<EndingDirectingData> FailedDataList = new List<EndingDirectingData>();
    public List<EndingDirectingData> AnotherSuccessDataList = new List<EndingDirectingData>();

    [SerializeField] Dialogue dialogue;
    [SerializeField] Button btnSkip;
    [SerializeField] Button btnFailed;
    [SerializeField] Transform anotherCamPos;

    Camera mainCam;

    /* Inspector */
    public int SuccessDataIndex;
    public int FailedDataIndex;
    public int AnotherSuccessDataIndex;

    void Start()
    {
        DontDestroyOnLoad(this);
        mainCam = Camera.main;
        if(btnSkip != null)
            btnSkip.onClick.AddListener(OnClickedBtnSkip);
            
        if (btnFailed !=null)
            btnFailed.onClick.AddListener(OnClickedBtnSkipFailed);

        if (dialogue == null)
            return;
        
        // start directing
        StartCoroutine(SceneDirecting());
    }

    List<EndingDirectingData> GetEndingDirectingList(bool isClear, bool isAnotherEnding)
    {
        // 반환형이 따른 카메라 위치 조정
        // 케릭터 위치 조정
        // todo 다중엔딩 조건체크
        if (isClear)
        {
            if(isAnotherEnding)
            {
                int gold = UserManager.Instance.UserInfo.Gold;
                gold += 1000;
                UserManager.instance.SetUserGold(gold);
                mainCam.transform.position = anotherCamPos.transform.position;
                mainCam.transform.rotation = anotherCamPos.transform.rotation;
                return AnotherSuccessDataList;
            }

            return SuccessDataList;
        }

        return FailedDataList;
    }

    public bool isClearTest;
    // 연출에서 인터랙션은 스킵버튼만
    IEnumerator SceneDirecting()
    {
        #region  Check null
        if(dialogue == null)
        {
            Debug.LogError("dialogue is null");
            yield break;
        }
       
        #endregion
        int directingIndex = 0;
        List<EndingDirectingData> directingList = GetEndingDirectingList(BattleManager.Instance.IsClear, BattleManager.Instance.isAnotherEnding);
        // List<EndingDirectingData> directingList = GetEndingDirectingList(isClearTest);

        while (directingIndex < directingList.Count)
        {
            EndingDirectingData data = directingList[directingIndex];
            switch (data.DirectingDataType)
            {
                case E_EndingDirectingDataType.FadeIn:
                    Image fadeInTargetImage = data.Target.GetComponent<Image>();
                    Color tempColor = fadeInTargetImage.color;
                    float startTime = 0f;
                    tempColor.a = Mathf.Lerp(1,0, startTime);
                    while (tempColor.a > 0f)
                    {
                        startTime += Time.deltaTime/data.DirectingValue;
                        tempColor.a = Mathf.Lerp(1,0,startTime);
                        fadeInTargetImage.color = tempColor;

                        yield return null;
                    }
                    fadeInTargetImage.gameObject.SetActive(false);
                    break;

                case E_EndingDirectingDataType.FadeOut:
                    Image fadeOutTargetImage = data.Target.GetComponent<Image>();
                    fadeOutTargetImage.gameObject.SetActive(true);
                    tempColor = fadeOutTargetImage.color;
                    startTime = 0f;
                    tempColor.a = Mathf.Lerp(0,1, startTime);
                    while (tempColor.a < 1f)
                    {
                        startTime += Time.deltaTime/data.DirectingValue;
                        tempColor.a = Mathf.Lerp(0,1,startTime);
                        fadeOutTargetImage.color = tempColor;

                        yield return null;
                    }
                    
                    break;

                case E_EndingDirectingDataType.EnemiesDeathAni:
                    enemy.EnemyUnitState = Graduate.Unit.E_UnitState.Death;
                    data.Target.SetActive(true);
                    yield return new WaitForSeconds(data.DirectingValue);
                    break;
                
                case E_EndingDirectingDataType.PlayersDeathAni:
                    for (int i = 0; i < playerList.Count; i++)
                        playerList[i].PlayerUnitState = Graduate.Unit.E_UnitState.Death;

                    data.Target.SetActive(true);
                    yield return new WaitForSeconds(data.DirectingValue);
                    break;

                case E_EndingDirectingDataType.Dialogue:
                    dialogue.ShowDialogue(data.DirectingDialogue);
                    while(!dialogue.IsTypingEnd)
                        yield return null;

                    yield return new WaitForSeconds(2f);
                    dialogue.ClearText();
                    break;

                case E_EndingDirectingDataType.ObjectMoving:
                    DirectingCrypt dc = data.Target.GetComponent<DirectingCrypt>();
                    if (dc == null)
                        Debug.LogError("DirectingCrypt is null");
                    else
                    {
                        dc.StartDirecting(data.DirectingValue);
                        while (!dc.IsDirectingEnd)
                            yield return null;
                    }
                    break;
            }
            Debug.LogError("DataIndex:"+directingIndex+" type: "+data.DirectingDataType);
            directingIndex++;
            yield return new WaitForSeconds(1f);
        }
       
        StartCoroutine(GoToLobby(BattleManager.instance.IsClear));
    }

    void OnClickedBtnSkip()
    {
        SoundManager.Instance.PlayButtonSound();
        StopCoroutine(SceneDirecting());
        StartCoroutine(GoToLobby(true));    // 심사용으로 true를 넣은것
    }

    void OnClickedBtnSkipFailed()
    {
        SoundManager.Instance.PlayButtonSound();
        StopCoroutine(SceneDirecting());
        StartCoroutine(GoToLobby(false));    // 심사용으로 true를 넣은것
    }

    IEnumerator GoToLobby(bool isClear)
    {
        if(isClear)
            UserManager.Instance.SetClearEnding();

        UserManager.Instance.ao  = SceneManager.LoadSceneAsync("NewLobby");
        while(!UserManager.Instance.ao.isDone)
            yield return null;
        UserManager.Instance.UserSituation = E_UserSituation.LoadingLobby;
        Destroy(gameObject);
        if(BattleManager.instance != null)
            BattleManager.Instance.DestroyBattleManager();
    }

    /* inspector */
     /// <summary>
    /// 맨 끝에 데이터를 추가
    /// </summary>
    public void InsertSuccessData()
    {
        SuccessDataList.Add(new EndingDirectingData());
    }
    
    /// <summary>
    /// 넣고싶은 데이터를 원하는 인덱스의 위치에 추가
    /// </summary>
    public void InsertSuccessDataAtDataIndex()
    {
        SuccessDataList.Insert(SuccessDataIndex, new EndingDirectingData());
    }

    /// <summary>
    /// 없애고 싶은 데이터를 제거
    /// </summary>
    public void RemoveSuccessData()
    {
        if(SuccessDataList.Count == 0)
        {
            Debug.LogError("SuccessDataList is empty");
            return;
        }

        SuccessDataList.RemoveAt(SuccessDataIndex);
    }

    /// <summary>
    /// 맨 끝에 데이터를 추가
    /// </summary>
    public void InsertFailedData()
    {
        FailedDataList.Add(new EndingDirectingData());
    }
    
    /// <summary>
    /// 넣고싶은 데이터를 원하는 인덱스의 위치에 추가
    /// </summary>
    public void InsertFailedDataAtDataIndex()
    {
        FailedDataList.Insert(FailedDataIndex, new EndingDirectingData());
    }

    /// <summary>
    /// 없애고 싶은 데이터를 제거
    /// </summary>
    public void RemoveFailedData()
    {
        if(FailedDataList.Count == 0)
        {
            Debug.LogError("SuccessDataList is empty");
            return;
        }

        FailedDataList.RemoveAt(FailedDataIndex);
    }


    /// <summary>
    /// 맨 끝에 데이터를 추가
    /// </summary>
    public void InsertAnotherData()
    {
        AnotherSuccessDataList.Add(new EndingDirectingData());
    }

    /// <summary>
    /// 넣고싶은 데이터를 원하는 인덱스의 위치에 추가
    /// </summary>
    public void InsertAnotherDataAtDataIndex()
    {
        AnotherSuccessDataList.Insert(AnotherSuccessDataIndex, new EndingDirectingData());
    }

    /// <summary>
    /// 없애고 싶은 데이터를 제거
    /// </summary>
    public void RemoveAnotherdData()
    {
        if (AnotherSuccessDataList.Count == 0)
        {
            Debug.LogError("AnotherDataList is empty");
            return;
        }

        AnotherSuccessDataList.RemoveAt(AnotherSuccessDataIndex);
    }
}