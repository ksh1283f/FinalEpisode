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
    LodingLobby,
    Lobby,
    LodingBattle,
    Battle,
}

public class UserManager : Singletone<UserManager>
{
    public static int MAX_CHARACTER_COUNT = 8;
    public AsyncOperation ao;
    // 유저 정보 변수
    public UserInfo UserInfo { get; private set; }
    public bool IsReadUserInfo { get; private set; }


    private E_UserSituation userSituation = E_UserSituation.None;
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

                case E_UserSituation.LodingLobby:
                    OnEnterLodingLobby.Execute();
                    break;

                case E_UserSituation.Lobby:
                    while (!ao.isDone) { }
                    OnEnterLobby.Execute();
                    break;

                case E_UserSituation.LodingBattle:
                    OnEnterLodingBattle.Execute();
                    break;

                case E_UserSituation.Battle:
                    OnEnterBattle.Execute();
                    break;
            }
        }
    }

    public Action OnEnterStartPage { get; set; }
    public Action OnEnterLodingLobby { get; set; }
    public Action OnEnterLobby { get; set; }
    public Action OnEnterLodingBattle { get; set; }
    public Action OnEnterBattle { get; set; }

    public Action<UserInfo> OnCreateUserInfoData { get; set; }
    public Action<UserInfo> OnUpdatedUserInfo { get; set; }

    // 직업 특성

    void Start()
    {
        // 내부 함수 이벤트
        OnEnterStartPage += OnStartPage;
        OnEnterLodingLobby += OnLodingLobby;
        OnEnterLobby += OnLobby;
        OnEnterLodingBattle += OnLodingBattle;
        OnEnterBattle += OnBattle;

        // 외부 함수 이벤트
        OnEnterLobby += CharacterPropertyManager.Instance.UpdatePropertyFromUserInfo;
    }

    void GetUserInfo()
    {
        UserInfo = SaveDataManager.Instance.ReadUserInfoData();
        if (UserInfo == null)
        {
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
            };
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

    void OnStartPage()
    {

    }

    void OnLodingLobby()
    {
        // 유저정보 읽어오기(없으면 로비에서 작성)
        // (필요한 데이터 읽어오기)
        GetUserInfo();


        // todo 필요한 데이터 읽기(던전 데이터 등등)
    }

    void OnLobby()
    {
        LobbyUI lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
        // todo 유저정보 ui에 갱신

        lobbyUI.Show();


    }

    void OnLodingBattle()
    {
    }

    void OnBattle()
    {

    }

    public void StartLoadLobbyScene()
    {
        StartCoroutine(LoadLobbyScene());
    }

    IEnumerator LoadLobbyScene()
    {
        yield return new WaitForSeconds(5f);
        BattleUI.Instance.Close();
        Debug.LogError("ready start loadLobbyScene");
        ao = SceneManager.LoadSceneAsync("Lobby");
        while (!ao.isDone)
            yield return null;

        Debug.LogError("start loadLobbyScene");
        UserSituation = E_UserSituation.LodingLobby;
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
