using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyContents : MonoBehaviour
{
    public E_LobbyContents contentsType;
    public Action<E_LobbyContents> OnExecuteContets { get; set; }
    private bool isThisContentsTurnInTutorial=true;
    public bool IsThisContentsTurnInTutorial
    {
        get {return isThisContentsTurnInTutorial;}
        set
        {
            if(value == isThisContentsTurnInTutorial)
                return;

            isThisContentsTurnInTutorial = value;
            if(NoticeIcon == null)
                return;

            NoticeIcon.gameObject.SetActive(value);
        }
    }
    [SerializeField] Text contentsNameText;
    [SerializeField] Image NoticeIcon;


    void Start()
    {
        if (contentsNameText == null)
        {
            Debug.LogError("contentsNameText is null");
            return;
        }

        if(NoticeIcon == null)
        {
            Debug.LogError("NoticeIcon is null");
            return;
        }

        StringBuilder sb = new StringBuilder();
        switch (contentsType)
        {
            case E_LobbyContents.UserInfo:
                sb.Append("사무소");
                sb.AppendLine();
                sb.Append("<유저 정보>");
                break;

            case E_LobbyContents.CharacterProperty:
                sb.Append("연구소");
                sb.AppendLine();
                sb.Append("<전투 특성 선택>");
                break;

            case E_LobbyContents.CharacterManage:
                sb.Append("훈련소");
                sb.AppendLine();
                sb.Append("<용병 캐릭터 관리>");
                break;

            case E_LobbyContents.Etc:
                sb.Append("TODO 기타 추가");
                sb.AppendLine();
                sb.Append("<추후 시장 작업>");
                break;

            case E_LobbyContents.ToBattle:
                sb.Append("던전");
                sb.AppendLine();
                sb.Append("<용병 훈련 및 도전모드>");
                break;

            case E_LobbyContents.CharacterTraining:
                sb.Append("훈련소");
                sb.AppendLine();
                sb.Append("<캐릭터 고용>");
                break;

            case E_LobbyContents.Market:
                sb.Append("용병 시장");
                sb.AppendLine();
                sb.Append("<용병 판매 및 구매>");
                break;
        }

        contentsNameText.text = sb.ToString();
        IsThisContentsTurnInTutorial = false;
    }

    void OnMouseDown()
    {
        if (UIManager.Instance.openedUiDic.Count == 1)
        {
            // 튜토리얼이 실행중이나, 현재 컨텐츠가 이것과 다르다면 종료
            if(!TutorialManager.Instance.IsTutorialComplete && TutorialManager.Instance.presentData.lobbyContentsType != contentsType)
                return;

            // Debug.LogError("Lobby contents OnMouseDown");
            OnExecuteContets.Execute(contentsType);
        }
    }

    void OnMouseUp()
    {
        if (UIManager.Instance.openedUiDic.Count == 1)
        {
            // 튜토리얼이 실행중이나, 현재 컨텐츠가 이것과 다르다면 종료
            if(!TutorialManager.Instance.IsTutorialComplete && TutorialManager.Instance.presentData.lobbyContentsType != contentsType)
                return;

            // Debug.LogError("Lobby contents OnMouseUp");
            OnExecuteContets.Execute(contentsType);
        }
    }
}
