﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    // 직업 특성

    void Start()
    {
        OnEnterStartPage += OnStartPage;
        OnEnterLodingLobby += OnLodingLobby;
        OnEnterLobby += OnLobby;
        OnEnterLodingBattle += OnLodingBattle;
        OnEnterBattle += OnBattle;

        
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

    public void SetUserInfo(string userName, int teamLevel, int exp, int gold)
    {
        if (UserInfo == null)
            return;

        if (!UserInfo.UserName.Equals(userName))
            UserInfo.UserName = userName;

        UserInfo.TeamLevel = teamLevel;
        UserInfo.Exp = exp;
        UserInfo.Gold = gold;
        SaveDataManager.Instance.WriteUserInfoData(UserInfo, false);
    }

    public void SetUserInfo(string userName, int teamLevel, int exp, int gold, List<E_Class> unitList, List<int> unitCommonPropertyList)
    {
        if (UserInfo == null)
            return;

        SetUserInfo(userName, teamLevel, exp, gold);
        if (unitList != null)
            UserInfo.UnitList = unitList;

        if (unitCommonPropertyList != null)
            UserInfo.UnitCommonPropertyList = unitCommonPropertyList;

        SaveDataManager.Instance.WriteUserInfoData(UserInfo,true);
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
}
