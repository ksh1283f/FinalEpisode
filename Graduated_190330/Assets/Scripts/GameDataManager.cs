using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using UnityEngine;
using Graduate.GameData.UnitData;

public enum E_GameDataType
{
    DungeonData, //- 몬스터 종류, 스킬, 텀 등의 던전데이터
    CharacterData,// - 캐릭터 종류에 관한 데이터
    BattlePropertyData,// - 전투 속성
    MarketData,// - 시장 정보
    LocalizeData,// - 언어(후순위)

    DataTypeCount,  // 데이터 타입 개수
}

public class GameDataManager : Singletone<GameDataManager>
{
    private const string dataDefalutPath = "GameData/";

    private const string DungeonDataName = "DungeonData";
    private const string CharacterDataName = "CharacterData";
    private const string battlePropertyDataName = "BattlePropertyData";
    private const string MarketDataName = "MarketData.csv";
    private const string LocalizeDataName = "LocalizeData.csv";


    public Dictionary<int, CharacterProperty> BattlePropertyDic { get; private set; }
    public Dictionary<int, UnitData> CharacterDataDic { get; private set; }

    void Awake()
    {
        // todo 기타 다른 데이터 딕셔너리도 추가
        BattlePropertyDic = new Dictionary<int, CharacterProperty>();
        CharacterDataDic = new Dictionary<int, UnitData>();
        for (int i = 0; i < (int)E_GameDataType.DataTypeCount; i++)
        {
            E_GameDataType type = (E_GameDataType)i;
            LoadGameData(type);
        }
    }

    void LoadGameData(E_GameDataType dataType)
    {
        try
        {
            switch (dataType)
            {
                case E_GameDataType.BattlePropertyData:
                    ReadBattlePropertyData(battlePropertyDataName);
                    break;

                case E_GameDataType.CharacterData:
                    ReadCharacterData(CharacterDataName);
                    break;
            }
        }
        catch (FileNotFoundException ex)
        {
            // 처음 루트를 타도록 바꾼다
            Debug.LogError("파일이 없습니다. 파일 이름을 확인해주세요.: " + ex);
            return;
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogError("경로가 잘못되었습니다. 경로를 확인해주세요.: " + ex);
            return;
        }
        catch (IsolatedStorageException ex)
        {
            Debug.LogError(ex);
            return;
        }
        catch
        {
            Debug.LogError("알수없는 에러");
            return;
        }
    }

    void ReadBattlePropertyData(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("BattlePropertyData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat(dataDefalutPath, path);
        TextAsset assetData = Resources.Load(dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++)
        {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty(strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split(',');
            int id = Convert.ToInt32(values[0]);
            string imagePath = values[1];
            string name = values[2];
            string description = values[3];
            E_PropertyEffectType effectType = (E_PropertyEffectType)Convert.ToInt32(values[4]);
            E_PropertyType propertyType = (E_PropertyType)Convert.ToInt32(values[5]);
            int effectValue = Convert.ToInt32(values[6]);

            CharacterProperty data = new CharacterProperty(id, imagePath, name, description, propertyType, effectType, effectValue);

            BattlePropertyDic.Add(data.Id, data);
        }
    }

    void ReadCharacterData(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("ReadCharacterData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat(dataDefalutPath, path);
        TextAsset assetData = Resources.Load(dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++)
        {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty(strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split(',');
            int id = Convert.ToInt32(values[0]);
            int hp = Convert.ToInt32(values[1]);
            int atk = Convert.ToInt32(values[2]);
            int def = Convert.ToInt32(values[3]);
            int cri = Convert.ToInt32(values[4]);
            int spd = Convert.ToInt32(values[5]);
            string iconName = values[6];
            E_CharacterType characterType = (E_CharacterType)Convert.ToInt32(values[7]);
            int price = Convert.ToInt32(values[8]);
            UnitData data = new UnitData(id, hp, atk,def, cri, spd, iconName, characterType, price);

            CharacterDataDic.Add(id, data);
        }
    }
}
