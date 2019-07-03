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
    [SerializeField] Text contentsNameText;

    void Start()
    {
        if(contentsNameText == null)
        {
            Debug.LogError("contentsNameText is null");
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
        }

        contentsNameText.text = sb.ToString();
    }
}
