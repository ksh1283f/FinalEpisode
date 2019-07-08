using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_LobbyContents
{
    None,
    UserInfo,
    CharacterProperty,
    CharacterManage,
    ToBattle,
    Etc,    // 기타(후에 추가될 컨텐츠들 임시명)
    Ground,
    CharacterTraining,
}

public class LobbyContentsManager : Singletone<LobbyContentsManager>
{
    Dictionary<E_LobbyContents, LobbyContents> lobbyContentsDic = new Dictionary<E_LobbyContents, LobbyContents>();
    LobbyUI lobbyUI;

    void Start()
    {
        lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
        LobbyContents[] contentsArray = GetComponentsInChildren<LobbyContents>();
        if (contentsArray == null)
            return;

        for (int i = 0; i < contentsArray.Length; i++)
        {
            if (contentsArray[i].contentsType == E_LobbyContents.None)
                continue;

            lobbyContentsDic.Add(contentsArray[i].contentsType, contentsArray[i]);
            contentsArray[i].OnExecuteContets = ExecuteContents;
        }
    }

    void ExecuteContents(E_LobbyContents contentsType)
    {
        switch (contentsType)
        {
            case E_LobbyContents.UserInfo:

                break;

            case E_LobbyContents.CharacterProperty:
                CharacterPropertyUI propertyUI = UIManager.Instance.LoadUI(E_UIType.CharacterProperty) as CharacterPropertyUI;
                propertyUI.Show(new string[] { "캐릭터 특성" });
                break;

            case E_LobbyContents.CharacterManage:

                break;

            case E_LobbyContents.ToBattle:
                // TODO 별도의 UI를 불러와서 시작할수 있도록 셋팅하기: 현재 다이렉트로 씬 로딩하도록 작업(190310)

                DungeonUI dungeonUI = UIManager.Instance.LoadUI(E_UIType.DungeonSelect) as DungeonUI;
                dungeonUI.Show(new string[] { "던전 정보" });
                break;

            case E_LobbyContents.Etc:
                MessageUI messageUI = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
                // TODO 별도의 텍스트 데이터파일을 받아서 UI를 셋팅하기: 현재 더미로 작업(190310)
                messageUI.Show(new string[] { "유저 메세지", "컨텐츠 업데이트 준비중입니다." });
                break;

            case E_LobbyContents.CharacterTraining:
                TrainingCenterUI trainingUI = UIManager.Instance.LoadUI(E_UIType.TrainingCenter) as TrainingCenterUI;
                trainingUI.Show(new string[] {"훈련소"});
                break;
        }
    }


}
