using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.IsolatedStorage;

public class SaveDataManager : Singletone<SaveDataManager>
{
    private const string dataPath = "/Resources/GameData/";
    private const string userInfoData = "UserInfo.json";
    private const string IsSavedUserInfo = "IS_SAVED_USER_INFO";
    bool isSavedUserInfo;
    string dataFullPath = string.Empty;

    // 캐시된 데이터가 있는지
    // 캐시된 데이터가 없어도 유저 정보 데이터가 있는지

    void Start()
    {
        dataFullPath = string.Concat(Application.dataPath, dataPath);
        isSavedUserInfo = PlayerPrefs.GetInt(IsSavedUserInfo) == 1;

#if DEVELOPMENT_BUILD
        Debug.Log("This release is development build");
#endif
    }

    public UserInfo ReadUserInfoData()
    {
        string path = string.Concat(dataFullPath, userInfoData);
        try
        {
            string userJsonData = File.ReadAllText(path);
            //todo enum값은 util parse함수를 이용하여 별도 처리가 필요할 수 있다.
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(userJsonData);

            return userInfo;
        }
        catch (FileNotFoundException ex)
        {
            // 처음 루트를 타도록 바꾼다
            Debug.LogError("유저정보가 없습니다. 유저정보등록이 필요합니다: " + ex);
            return null;
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogError(ex);
            return null;
        }
        catch (IsolatedStorageException ex)
        {
            Debug.LogError(ex);
            return null;
        }
        catch
        {
            Debug.Log("알수없는 에러");
            return null;
        }
    }

    /// <summary>
    /// 유저정보 쓰기
    /// 매개변수가 없는 경우는 새로 등록하는 경우
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="teamLevel"></param>
    /// <param name="gold"></param>
    /// <param name="unitList"></param>
    /// <param name="unitCommonPropertyList"></param>
    public void WriteUserInfoData(string userName, int teamLevel = 1, int exp = 0, int gold = 0, E_PropertyType property = E_PropertyType.None, List<E_Class> unitList = null)
    {
        DirectoryInfo di = new DirectoryInfo(dataFullPath);
        if (!di.Exists)
            di.Create();

        string path = string.Concat(dataFullPath, userInfoData);
        UserInfo userInfo = new UserInfo();
        if (string.IsNullOrEmpty(userInfo.UserName))
            userInfo.UserName = userName;

        userInfo.TeamLevel = teamLevel;
        userInfo.Exp = exp;
        userInfo.Gold = gold;

        if (userInfo.UnitList == null)
            userInfo.UnitList = new List<E_Class>();
        else
        {
            if (unitList != null)
                userInfo.UnitList = unitList;
        }

        userInfo.PropertyType = property;

        JsonUtility.ToJson(userInfo);
        string toJson = JsonUtility.ToJson(userInfo, prettyPrint: true);
        File.WriteAllText(path, toJson);

        if (!isSavedUserInfo)
            PlayerPrefs.SetInt(IsSavedUserInfo, 1);
    }

    /// <summary>
    /// 유저정보 갱신
    /// 기존 UserInfo.json 파일이 존재하는 경우에만
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="teamLevel"></param>
    /// <param name="gold"></param>
    /// <param name="unitList"></param>
    /// <param name="unitCommonPropertyList"></param>
    public void UpdateUserInfoData(UserInfo userInfo)
    {
        if (userInfo == null)
            return;

        string path = string.Concat(dataFullPath, userInfoData);
        JsonUtility.ToJson(userInfo);
        string toJson = JsonUtility.ToJson(userInfo, prettyPrint: true);
        File.WriteAllText(path, toJson);
    }
}