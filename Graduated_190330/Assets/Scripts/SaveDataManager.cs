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
                userInfo.UnitDic = ConvertToUnitData(userInfo.UnitList);

            if (userInfo.SelectedUnitDic == null)
                userInfo.SelectedUnitDic = ConvertToUnitData(userInfo.SelectedUnitList);

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
        catch (System.Exception ex)
        {
            Debug.LogError("exception: " + ex);
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
    public void WriteUserInfoData(string userName, int teamLevel = 1, int exp = 0, int gold = 0, E_PropertyEffectType property = E_PropertyEffectType.None, Dictionary<int, UnitData> unitDic = null, Dictionary<int, UnitData> selectedUnitDic = null)
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

        if (userInfo.SelectedUnitDic == null)
            userInfo.SelectedUnitDic = new Dictionary<int, UnitData>();
        else
        {
            if (selectedUnitDic != null)
                userInfo.SelectedUnitDic = selectedUnitDic;
        }

        userInfo.UnitList = ConvertToSerializableUnitdata(userInfo.UnitDic);
        userInfo.SelectedUnitList = ConvertToSerializableUnitdata(userInfo.SelectedUnitDic);

        userInfo.CommonPropertyType = property;
        // todo 나머지 특성들 추가해야함

        userInfo.TutorialClearList = new List<bool>();
        for (int i = 0; i < (int)E_SimpleTutorialType.E_SimpleTutorialTypeCount; i++)
            userInfo.TutorialClearList.Add(false);

        userInfo.IsAllTutorialClear = false;
        
        JsonUtility.ToJson(userInfo);
        string toJson = JsonUtility.ToJson(userInfo, prettyPrint: true);
        File.WriteAllText(path, toJson);
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

        userInfo.UnitList = ConvertToSerializableUnitdata(userInfo.UnitDic);
        userInfo.SelectedUnitList = ConvertToSerializableUnitdata(userInfo.SelectedUnitDic);
        string path = string.Concat(dataFullPath, userInfoData);
        JsonUtility.ToJson(userInfo);
        string toJson = JsonUtility.ToJson(userInfo, prettyPrint: true);
        File.WriteAllText(path, toJson);
    }

    List<SerializableUnitData> ConvertToSerializableUnitdata(Dictionary<int, UnitData> dataDic)
    {
        List<SerializableUnitData> convertedList = new List<SerializableUnitData>();
        foreach (var item in dataDic)
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
            data.Level = item.Value.Level;
            data.Exp = item.Value.Exp;

            convertedList.Add(data);
        }

        return convertedList;
    }

    Dictionary<int, UnitData> ConvertToUnitData(List<SerializableUnitData> dataList)
    {
        Dictionary<int, UnitData> convertedDic = new Dictionary<int, UnitData>();
        if (dataList == null)
        {
            dataList = new List<SerializableUnitData>();
            return convertedDic;
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            int Id = dataList[i].Id;
            int Hp = dataList[i].Hp;
            int Atk = dataList[i].Atk;
            int Def = dataList[i].Def;
            int Cri = dataList[i].Cri;
            int Spd = dataList[i].Spd;
            string IconName = dataList[i].IconName;
            E_CharacterType CharacterType = dataList[i].CharacterType;
            int Price = dataList[i].Price;
            string Description = dataList[i].Description;
            int level = dataList[i].Level;
            int exp = dataList[i].Exp;

            UnitData data = new UnitData(Id, Hp, Atk, Def, Cri, Spd, IconName, CharacterType, Price, Description, level, exp);
            convertedDic.Add(data.Id, data);
        }

        return convertedDic;
    }
}