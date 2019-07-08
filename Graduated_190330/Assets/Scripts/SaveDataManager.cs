using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.IsolatedStorage;
using Graduate.GameData.UnitData;
using System.Linq;

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
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(userJsonData);
            if (userInfo.UnitDic == null)
                userInfo.UnitDic = ConvertToUnitData(userInfo);

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
        catch(System.Exception ex)
        {
            Debug.LogError("exception: "+ ex);
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
    public void WriteUserInfoData(string userName, int teamLevel = 1, int exp = 0, int gold = 0, E_PropertyType property = E_PropertyType.None, Dictionary<int, UnitData> unitDic = null)
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

        if (userInfo.UnitDic == null)
            userInfo.UnitDic = new Dictionary<int, Graduate.GameData.UnitData.UnitData>();
        else
        {
            if (unitDic != null)
                userInfo.UnitDic = unitDic;
        }

        userInfo.UnitList = ConvertToSerializableUnitdata(userInfo);

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

        userInfo.UnitList = ConvertToSerializableUnitdata(userInfo);
        string path = string.Concat(dataFullPath, userInfoData);
        JsonUtility.ToJson(userInfo);
        string toJson = JsonUtility.ToJson(userInfo, prettyPrint: true);
        File.WriteAllText(path, toJson);
    }

    List<SerializableUnitData> ConvertToSerializableUnitdata(UserInfo userInfo)
    {
        List<SerializableUnitData> convertedList = new List<SerializableUnitData>();
        foreach (var item in userInfo.UnitDic)
        {
            SerializableUnitData data = new SerializableUnitData();
            data.Id = item.Value.Id;
            data.Hp = item.Value.Hp;
            data.Atk = item.Value.Atk;
            data.Def = item.Value.Def;
            data.Cri = item.Value.Cri;
            data.Spd = item.Value.Spd;
            data.IconName = item.Value.IconName;
            data.CharacterType = item.Value.CharacterType;
            data.Price = item.Value.Price;
            data.Description = item.Value.Description;

            convertedList.Add(data);
        }

        return convertedList;
    }

    Dictionary<int, UnitData> ConvertToUnitData(UserInfo userInfo)
    {
        Dictionary<int, UnitData> convertedDic = new Dictionary<int, UnitData>();
        if (userInfo.UnitList == null)
        { 
            userInfo.UnitList = new List<SerializableUnitData>();
            return convertedDic;           
        }

        for (int i = 0; i < userInfo.UnitList.Count; i++)
        {
            int Id = userInfo.UnitList[i].Id;
            int Hp = userInfo.UnitList[i].Hp;
            int Atk = userInfo.UnitList[i].Atk;
            int Def = userInfo.UnitList[i].Def;
            int Cri = userInfo.UnitList[i].Cri;
            int Spd = userInfo.UnitList[i].Spd;
            string IconName = userInfo.UnitList[i].IconName;
            E_CharacterType CharacterType = userInfo.UnitList[i].CharacterType;
            int Price = userInfo.UnitList[i].Price;
            string Description = userInfo.UnitList[i].Description;

            UnitData data = new UnitData(Id, Hp, Atk, Def, Cri, Spd, IconName, CharacterType, Price, Description);
            convertedDic.Add(data.Id, data);
        }

        return convertedDic;
    }
}