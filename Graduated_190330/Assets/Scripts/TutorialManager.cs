using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_TutorialType {
    None,
    Intro,
    HireUnit,
    BattleIntro,
    Battle,
    UnitManagement,
    BattleProperty,
    IntroduceMarket,

}

public class TutorialManager : Singletone<TutorialManager>
{
    public List<TutorialData> dataList = new List<TutorialData> ();
    public List<TutorialDataSerialized> serializedDataList = new List<TutorialDataSerialized> ();
    public TutorialDataSerialized presentData {get; private set;}
    public Coroutine tutorialCoroutine;
    [SerializeField] bool IsSkip;

    void Start()
    {
        tutorialCoroutine = null;
        presentData = null;
        for (int i = 0; i < serializedDataList.Count; i++)
        {
            E_TutorialType type = serializedDataList[i].tutorialType;
            int detailId = serializedDataList[i].DetailId;
            serializedDataList[i].Dialogue= GameDataManager.Instance.TutorialDataDic[type][detailId].Dialogue;
        }
    }

    // public void StartTutorial()
    // {
    //     if(IsSkip)
    //     {
    //         Debug.LogError("All Tutorial are skipped");
    //         return;
    //     }
        
    //     if(tutorialCoroutine == null)
    //         tutorialCoroutine = StartCoroutine(ShowTutorial());
    // }

    // private IEnumerator ShowTutorial () 
    // {
    //     E_TutorialType nowTutorialType;
    //     if(UserManager.Instance.UserInfo == null)
    //     {
    //         nowTutorialType = E_TutorialType.None;
    //     }
    //     else
    //     {
    //         if (UserManager.Instance.UserInfo.CompleteTutorialStep == E_TutorialType.IntroduceMarket)
    //             yield break; // 튜토리얼을 완료한 경우에는 종료    
    //     }

    //     if(TutorialUI.Instance.dialogue == null)
    //     {
    //         Debug.LogError("Tutorial ui dialogue is null");
    //         yield break;
    //     }

    //     // 완료한 튜토리얼 다음 단계(init)
    //     nowTutorialType = ++UserManager.Instance.UserInfo.CompleteTutorialStep;
    //     // presentData 초기화
    //     for (int i = 0; i < serializedDataList.Count; i++)
    //     {
    //         if(serializedDataList[i].tutorialType == nowTutorialType && serializedDataList[i].DetailId == 0)
    //         {
    //             presentData = serializedDataList[i];
    //             break;
    //         }
    //     }

    //     //  보여줄 튜토리얼 데이터가 있는 경우에만
    //     while (presentData != null)
    //     {
    //         // 다른 프로세스에 의한 딜레이가 있는지
    //         if(presentData.isNeedDelay)
    //         {
    //             while (!presentData.isCompleteReady)
    //                 yield return null;
    //         }

    //         // 딜레이 타임이 있는지
    //         if(presentData.delayTime > 0)
    //             yield return new WaitForSeconds(presentData.delayTime);

    //         // 보여줄 다이얼로그가 있는지
    //         if(!string.IsNullOrEmpty(presentData.Dialogue))
    //         {
    //             // show dialogue
    //             TutorialUI.Instance.dialogue.SetDialogueDic(presentData.Dialogue);
    //             TutorialUI.Instance.dialogue.ShowDialogue(TutorialUI.Instance.dialogue.dialogueDic[TutorialUI.Instance.dialogue.DialogueDicIndex]);
    //             while (TutorialUI.Instance.dialogue.Coroutine != null)
    //                 yield return null;
    //         }

    //         // 카메라 조정이 필요한지
    //         if(presentData.camPosition != Vector3.zero)
    //         {
    //             // todo move camera
    //         }

    //         // 건드려야할 오브젝트가 있는지
    //         if(presentData.Object != null)
    //         {
    //             // 기존 부모 오브젝트 저장
    //             GameObject parentObj = presentData.Object.transform.parent.gameObject;

    //             // 튜토리얼ui를 부모로 잡아준다
    //             presentData.Object.transform.SetParent(TutorialUI.Instance.transform);
                
                
    //             // todo 조건이 완료될때까지 대기                
    //         }

            

    //         //  다음 데이터 넣기 +  튜토리얼 완료 처리
    //     }

    //     // tutorialCoroutine = null;
    // }

}