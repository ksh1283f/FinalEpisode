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
    public List<TutorialDataSerialized> serializedDataList = new List<TutorialDataSerialized> ();
    public TutorialDataSerialized presentData {get; private set;}
    public Coroutine tutorialCoroutine;
    public bool IsTutorialComplete;

    [SerializeField] bool IsSkip;
    int presentTutorialIndex = 0;
    LobbyUI lobbyUI;



    void Start()
    {
        tutorialCoroutine = null;
        presentData = null;
        
        lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
    }

    public void StartTutorial()
    {
        if(UserManager.Instance.UserInfo.IsAllTutorialClear)
        {
            Debug.Log("All tutorial is clear");
            return;
        }

        if(tutorialCoroutine == null)
            tutorialCoroutine = StartCoroutine(LobbyTutorialCoroutine());
        
    }

    IEnumerator LobbyTutorialCoroutine()
    {
        // 튜토리얼 중에는 비활성화
        lobbyUI.ActivateScrollBarInteraction(false);
        
        while (presentTutorialIndex < serializedDataList.Count)
        {
            // 0. 인덱스 초기화
            int index = presentTutorialIndex;
            presentData = serializedDataList[index];

            // 1. 메세지(안내)
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;         
            message.Show(new string [] {"게임 안내",serializedDataList[index].Dialogue});
            while (message.gameObject.activeSelf)
                yield return null;

            // 2. 카메라 이동, 카메라 스크롤바 조정
            lobbyUI.SetCamPosition(serializedDataList[index].camXpos);

            // 3. 알림 아이콘 활성화
            LobbyContentsManager.Instance.lobbyContentsDic[serializedDataList[index].lobbyContentsType].IsThisContentsTurnInTutorial = true;

            // 3-1. 켜질때까지 기다리기
            while(serializedDataList[index].lobbyContentsUI.gameObject != null
            && !serializedDataList[index].lobbyContentsUI.gameObject.activeSelf)
                yield return null;

            while (presentData != null && presentData.lobbyContentsUI.gameObject.activeSelf)
                yield return null;

            // 4. 대상 ui가 종료되면 다음으로
            LobbyContentsManager.Instance.lobbyContentsDic[serializedDataList[index].lobbyContentsType].IsThisContentsTurnInTutorial = false;
            Debug.LogError("presentTutorialIndex: "+presentTutorialIndex);
            presentTutorialIndex++;
        }

        // 끝날때 로비의 스크롤바를 다시 재활성화
        lobbyUI.ActivateScrollBarInteraction(true);

        // 튜토리얼 클리어 상태로 변경
        UserManager.Instance.SetAllTutorialClear();
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