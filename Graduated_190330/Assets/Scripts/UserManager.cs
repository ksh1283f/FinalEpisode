using System;
using System.Collections;
using System.Collections.Generic;
using Graduate.GameData.UnitData;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_UserSituation
{
    None,
    StartPage,
    LoadingLobby,
    Lobby,
    LoadingBattle,
    Battle,
}

public class UserManager : Singletone<UserManager>
{
    public static int MAX_CHARACTER_COUNT = 8;
    public AsyncOperation ao;
    // 유저 정보 변수
    public UserInfo UserInfo { get; private set; }
    public bool IsReadUserInfo { get; private set; }


    [SerializeField]private E_UserSituation userSituation = E_UserSituation.None;
    public E_UserSituation UserSituation
    {
        get { return userSituation; }
        set
        {
            if (value == userSituation)
                return;

            userSituation = value;
            switch (userSituation)
            {
                case E_UserSituation.StartPage:
                    OnEnterStartPage.Execute();
                    break;

                case E_UserSituation.LoadingLobby:
                    OnEnterLoadingLobby.Execute();
                    break;

                case E_UserSituation.Lobby:
                    while (!ao.isDone) { }
                    OnEnterLobby.Execute();
                    break;

                case E_UserSituation.LoadingBattle:
                    OnEnterLoadingBattle.Execute();
                    break;

                case E_UserSituation.Battle:
                    OnEnterBattle.Execute();
                    break;
            }
        }
    }

    public DungeonMonsterData SelectedDungeonMonsterData;

    public Action OnEnterStartPage { get; set; }
    public Action OnEnterLoadingLobby { get; set; }
    public Action OnEnterLobby { get; set; }
    public Action OnEnterLoadingBattle { get; set; }
    public Action OnEnterBattle { get; set; }

    public Action<UserInfo> OnCreateUserInfoData { get; set; }
    public Action<UserInfo> OnUpdatedUserInfo { get; set; }

    // 직업 특성

    void Start()
    {
        // 내부 함수 이벤트
        OnEnterStartPage += OnStartPage;
        OnEnterLoadingLobby += OnLoadingLobby;
        OnEnterLobby += OnLobby;
        OnEnterLoadingBattle += OnLoadingBattle;
        OnEnterBattle += OnBattle;

        // 외부 함수 이벤트
        OnEnterLobby += CharacterPropertyManager.Instance.UpdatePropertyFromUserInfo;
    }

    void GetUserInfo()
    {
        UserInfo = SaveDataManager.Instance.ReadUserInfoData();
        if (UserInfo == null)
        {
            // 없을경우에는 무조건 튜토리얼 실행
            // 널이면 파일이 없거나 경로가 잘못된 경우
            // show input ui
            Debug.Log("There is no user info");
            
            InputUI input = UIManager.Instance.LoadUI(E_UIType.Input) as InputUI;
            input.Show();
            input.OnClickedBtnOk += (inputValue) =>
            {
                SaveDataManager.Instance.WriteUserInfoData(inputValue);
                UserInfo = SaveDataManager.Instance.ReadUserInfoData();
                OnCreateUserInfoData.Execute(UserInfo);
                MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
                message.Show(new string [] {"게임 안내","전투를 하기 위해서는 훈련소에서 용병을 고용한 후, 출전 리스트에 등록해야 합니다."});
                
                // 용병 고용을 위한 100골드 제공
                SetUserGold(100);
            };
        }
        else
        {
            // 튜토리얼이 중간에 끝난 경우?
            
        }

        UserSituation = E_UserSituation.Lobby;
    }

    // 유저 기본 정보 갱신 함수
    public void SetUserInfo(string userName, int teamLevel, int exp, int gold)
    {
        if (UserInfo == null)
            return;

        if (!UserInfo.UserName.Equals(userName))
            UserInfo.UserName = userName;

        UserInfo.TeamLevel = teamLevel;
        UserInfo.Exp = exp;
        UserInfo.Gold = gold;
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
        OnUpdatedUserInfo.Execute(UserInfo);
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        if (userInfo == null)
        {
            Debug.LogError("userInfo is null");
            return;
        }

        UserInfo = userInfo;
        OnUpdatedUserInfo.Execute(UserInfo);
    }

    public void SetPropertyInUserInfo(E_PropertyType propertyType)
    {
        if (UserInfo == null)
            return;

        UserInfo.PropertyType = propertyType;
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
    }

    public void SetUserGold(int gold)
    {
        if (UserInfo == null)
            return;

        UserInfo.Gold = gold;
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
        OnUpdatedUserInfo.Execute(UserInfo);
    }

    void OnStartPage()
    {

    }

    void OnLoadingLobby()
    {
        // todo 계정 이름을 설정하기 전에 튜토리얼을 할 필요가 있을 때 여기서 작업


        // 유저정보 읽어오기(없으면 로비에서 작성)
        // (필요한 데이터 읽어오기)
        GetUserInfo();
    }

    void OnLobby()
    {
        LobbyUI lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
        // todo 유저정보 ui에 갱신

        lobbyUI.Show();

        // todo 튜토리얼이 남아있는지
    }

    void OnLoadingBattle()
    {

    }

    void OnBattle()
    {
        // todo 튜토리얼이 남아있는지
    }

    public void StartLoadLobbyScene()
    {
        StartCoroutine(LoadLobbyScene());
    }

    IEnumerator LoadLobbyScene()
    {
        yield return new WaitForSeconds(2f);
        BattleUI.Instance.Close();
        Debug.LogError("ready start loadLobbyScene");
        // ao = SceneManager.LoadSceneAsync("Lobby");
        ao = SceneManager.LoadSceneAsync("NewLobby");
        while (!ao.isDone)
            yield return null;

        Debug.LogError("start loadLobbyScene");
        UserSituation = E_UserSituation.LoadingLobby;
    }

    public void SetMyUnitList(UnitData changedData)
    {
        if (changedData == null)
        {
            Debug.LogError("changed data is null");
            return;
        }

        if (UserInfo == null)
        {
            Debug.LogError("user info is null");
            return;
        }

        // if(!UserInfo.UnitDic.ContainsKey(changedData.Id))
        // {
        //     Debug.LogError("There is no id in unitDic");
        //     return;
        // }

        // UserInfo.UnitDic[changedData.Id] = changedData;
        if (UserInfo.UnitDic.ContainsKey(changedData.Id))
            UserInfo.UnitDic[changedData.Id] = changedData;
        else
            UserInfo.UnitDic.Add(changedData.Id, changedData);
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
    }

    public void RemoveUnitInList(int unitId)
    {
        if (UserInfo == null)
        {
            Debug.LogError("userInfo is null");
            return;
        }

        if (!UserInfo.UnitDic.ContainsKey(unitId))
        {
            Debug.LogError("There is no id in UnitList");
            return;
        }

        UserInfo.UnitDic.Remove(unitId);
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
    }

    public void SetTutorialClearState(E_SimpleTutorialType type)
    {
        UserInfo.TutorialClearList[(int)type] = true;
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
    }

    public void SetMySelectedUnitList(UnitData data, E_SelectedUnitListSetType setType)
    {
        if (data == null)
        {
            Debug.LogError("unit data is null");
            return;
        }

        if (UserInfo == null)
        {
            Debug.LogError("user info is null");
            return;
        }
        switch (setType)
        {

            case E_SelectedUnitListSetType.Insert:
                if (UserInfo.SelectedUnitDic.Count >= 3)
                {
                    Debug.LogError("SelectedUnitDic's capacity is full");
                    return;
                }
                UserInfo.SelectedUnitDic.Add(data.Id, data);
                break;

            case E_SelectedUnitListSetType.Remove:
                if (!UserInfo.SelectedUnitDic.ContainsKey(data.Id))
                {
                    Debug.LogError("There is not in dictionary to remove data");
                    return;
                }
                UserInfo.SelectedUnitDic.Remove(data.Id);
                break;

            case E_SelectedUnitListSetType.Trade:
                //remove and insert
                break;
        }

        // 유저파일에 기록
        SaveDataManager.Instance.UpdateUserInfoData(UserInfo);
    }
}
