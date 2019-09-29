using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Graduate.GameData.UnitData;
using UnityEngine;

public enum E_GameDataType 
{

    CharacterData, // - 캐릭터 종류에 관한 데이터
    BattlePropertyData, // - 전투 속성
    EnemyPatternData, // 몬스터 별 사용 스킬, 데미지 수치등의 상세 데이터
    DungeonPatternData, //- 몬스터 종류, 체력 등의 전반적인 데이터
    RewardData, // - 던전 보상 정보
    EnemyStatCorrectionData, // 던전 단계별 몬스터 능력치 보정 데이터
    DungeonMonsterData, // 던전별로 어떤 몬스터가 출현하는지에 관한 데이터
    MarketBasisData, // - 시장 기본 정보
    MarketSpecialEventData, // 시장 특별 이벤트 관련 데이터
    StatAugmenterData,
    OpeningSceneDialogueData, // 오프닝 세계관 소개 컷씬 관련 텍스트 데이터
    TutorialData,   // obsolete
    TutorialSimpleData, // 현재 사용중인 튜토리얼
    LocalizeData, // - 언어(후순위)

    DataTypeCount, // 데이터 타입 개수
}

public class GameDataManager : Singletone<GameDataManager> {
    private const string dataDefalutPath = "GameData/";
    private const string DungeonPatternDataName = "DungeonPatternData";
    private const string EnemyPatternDataName = "EnemyPatternData";
    private const string CharacterDataName = "CharacterData";
    private const string battlePropertyDataName = "BattlePropertyData";
    private const string rewardDataName = "RewardData";
    private const string EnemyStatCorrectionDataName = "EnemyStatCorrectionData";
    private const string DungeonMonsterDataName = "DungeonMonsterData";
    private const string LocalizeDataName = "LocalizeData.csv";
    private const string MarketBasisDataName = "MarketBasisData";
    private const string MarketSpecialEventDataName = "MarketSpecialEventData";
    private const string StatAugmenterDataName = "StatAugmenterData";
    private const string OpeningSceneDialogueDataName = "OpeningSceneDialogueData";
    private const string TutorialDataName = "TutorialData";
    private const string TutorialSimpleDataName = "TutorialSimpleData";
    

    public Dictionary<int, CharacterProperty> BattlePropertyDic { get; private set; }
    public Dictionary<int, UnitData> CharacterDataDic { get; private set; }
    public Dictionary<int, DungeonPattern> DungeonPatternDataDic { get; private set; }
    public Dictionary<int, EnemyPattern> EnemyPatternDataDic { get; private set; }
    public Dictionary<int, RewardData> RewardDataDic { get; private set; }
    public Dictionary<int, EnemyStatCorrectionData> EnemyStatCorrectionDataDic { get; private set; }
    public Dictionary<int, DungeonMonsterData> DungeonMonsterDataDic { get; private set; }
    public Dictionary<int, MarketBasisData> MarketBasisDataDic { get; private set; }
    public Dictionary<int, MarketSpecialEventData> MarketSpecialEventDataDic { get; private set; }
    public Dictionary<int, StatAugmenterData> StatAugmenterDataDic { get; private set; }
    public Dictionary<int, OpeningSceneDialogueData> OpeningSceneDialogueDataDic { get; private set; }
    public Dictionary<E_TutorialType, Dictionary<int, TutorialData>> TutorialDataDic {get; private set;}
    public Dictionary<E_SimpleTutorialType, Dictionary<int, TutorialSimpleData>> SimpleTutorialDataDic {get; private set;}

    void Awake () {
        // todo 기타 다른 데이터 딕셔너리도 추가
        BattlePropertyDic = new Dictionary<int, CharacterProperty> ();
        CharacterDataDic = new Dictionary<int, UnitData> ();
        DungeonPatternDataDic = new Dictionary<int, DungeonPattern> ();
        EnemyPatternDataDic = new Dictionary<int, EnemyPattern> ();
        RewardDataDic = new Dictionary<int, RewardData> ();
        EnemyStatCorrectionDataDic = new Dictionary<int, EnemyStatCorrectionData> ();
        DungeonMonsterDataDic = new Dictionary<int, DungeonMonsterData> ();
        MarketBasisDataDic = new Dictionary<int, MarketBasisData> ();
        MarketSpecialEventDataDic = new Dictionary<int, MarketSpecialEventData> ();
        StatAugmenterDataDic = new Dictionary<int, StatAugmenterData> ();
        OpeningSceneDialogueDataDic = new Dictionary<int, OpeningSceneDialogueData>();
        TutorialDataDic = new Dictionary<E_TutorialType, Dictionary<int, TutorialData>>();
        SimpleTutorialDataDic = new Dictionary<E_SimpleTutorialType, Dictionary<int, TutorialSimpleData>>();
                

        for (int i = 0; i < (int) E_GameDataType.DataTypeCount; i++) {
            E_GameDataType type = (E_GameDataType) i;
            LoadGameData (type);
        }
        Debug.Log ("parsing Complete");
    }

    void LoadGameData (E_GameDataType dataType) {
        try {
            switch (dataType) {
                case E_GameDataType.BattlePropertyData:
                    ReadBattlePropertyData (battlePropertyDataName);
                    break;

                case E_GameDataType.CharacterData:
                    ReadCharacterData (CharacterDataName);
                    break;

                case E_GameDataType.EnemyPatternData:
                    ReadEnemyPatternData (EnemyPatternDataName);
                    break;

                case E_GameDataType.DungeonPatternData:
                    ReadDungeonPatternData (DungeonPatternDataName);
                    break;

                case E_GameDataType.RewardData:
                    ReadRewardData (rewardDataName);
                    break;

                case E_GameDataType.EnemyStatCorrectionData:
                    ReadEnemyStatCorrectionData (EnemyStatCorrectionDataName);
                    break;

                case E_GameDataType.DungeonMonsterData:
                    ReadDungeonMonsterData (DungeonMonsterDataName);
                    break;

                case E_GameDataType.MarketBasisData:
                    ReadMarketBasisData (MarketBasisDataName);
                    break;

                case E_GameDataType.MarketSpecialEventData:
                    ReadMarketSpecialEventData (MarketSpecialEventDataName);
                    break;

                case E_GameDataType.StatAugmenterData:
                    ReadStatAugmenterData (StatAugmenterDataName);
                    break;

                case E_GameDataType.OpeningSceneDialogueData:
                    ReadOpeningSceneDialogueData(OpeningSceneDialogueDataName);
                    break;

                // case E_GameDataType.TutorialData:
                //     ReadTutorialData(TutorialDataName);
                //     break;

                case E_GameDataType.TutorialSimpleData:
                    ReadTutorialSimpleData(TutorialSimpleDataName);
                    break;
            }
        } catch (FileNotFoundException ex) {
            // 처음 루트를 타도록 바꾼다
            Debug.LogError ("파일이 없습니다. 파일 이름을 확인해주세요.: " + ex);
            return;
        } catch (DirectoryNotFoundException ex) {
            Debug.LogError ("경로가 잘못되었습니다. 경로를 확인해주세요.: " + ex);
            return;
        } catch (IsolatedStorageException ex) {
            Debug.LogError (ex);
            return;
        } catch {
            Debug.LogError ("알수없는 에러");
            return;
        }
    }

    void ReadBattlePropertyData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("BattlePropertyData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            string imagePath = values[1];
            string name = values[2];
            string description = values[3];
            E_PropertyEffectType effectType = (E_PropertyEffectType) Convert.ToInt32 (values[4]);
            E_PropertyType propertyType = (E_PropertyType) Convert.ToInt32 (values[5]);
            int effectValue = Convert.ToInt32 (values[6]);

            CharacterProperty data = new CharacterProperty (id, imagePath, name, description, propertyType, effectType, effectValue);

            BattlePropertyDic.Add (data.Id, data);
        }
    }

    void ReadCharacterData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadCharacterData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int hp = Convert.ToInt32 (values[1]);
            int atk = Convert.ToInt32 (values[2]);
            int def = Convert.ToInt32 (values[3]);
            int cri = Convert.ToInt32 (values[4]);
            int spd = Convert.ToInt32 (values[5]);
            string iconName = values[6];
            E_CharacterType characterType = (E_CharacterType) Convert.ToInt32 (values[7]);
            int price = Convert.ToInt32 (values[8]);
            string description = values[9];
            int level = Convert.ToInt32 (values[10]);
            int exp = Convert.ToInt32 (values[11]);
            UnitData data = new UnitData (id, hp, atk, def, cri, spd, iconName, characterType, price, description, level, exp);

            CharacterDataDic.Add (id, data);
        }
    }

    void ReadEnemyPatternData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadEnemyPatternData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            string skillName = values[1];
            E_UserSkillType skillType = (E_UserSkillType) Convert.ToInt32 (values[2]);
            int castTime = Convert.ToInt32 (values[3]);
            string skillDescription = values[4];
            int damage = Convert.ToInt32 (values[5]);
            EnemyPattern pattern = new EnemyPattern (id, skillName, skillType, castTime, skillDescription, damage);

            EnemyPatternDataDic.Add (pattern.SkillId, pattern);
        }
    }

    void ReadDungeonPatternData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadDungeonPatternData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            string enemyName = values[1];
            int enemyHealth = Convert.ToInt32 (values[2]);
            string patternDescription = values[3];
            int patternTerm = Convert.ToInt32 (values[4]);
            int skillCount = Convert.ToInt32 (values[5]);
            List<EnemyPattern> enemyPatternList = new List<EnemyPattern> ();
            for (int j = 0; j < skillCount; j++) {
                int skillId = Convert.ToInt32 (values[6 + j]);
                EnemyPattern enemyPattern = EnemyPatternDataDic[skillId].ShallowCopy () as EnemyPattern;
                enemyPatternList.Add (enemyPattern);
            }

            DungeonPattern pattern = new DungeonPattern (id, enemyName, enemyHealth, enemyPatternList, patternDescription, patternTerm);
            DungeonPatternDataDic.Add (pattern.Id, pattern);
        }
    }

    void ReadRewardData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadRewardData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int dungeonId = Convert.ToInt32 (values[0]);
            int gold = Convert.ToInt32 (values[1]);
            int exp = Convert.ToInt32 (values[2]);
            RewardData data = new RewardData (dungeonId, gold, exp);
            RewardDataDic.Add (data.DungeonId, data);
        }
    }

    void ReadEnemyStatCorrectionData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadDungeonPatternData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int hpCorrection = Convert.ToInt32 (values[1]);
            int atkCorrection = Convert.ToInt32 (values[2]);

            EnemyStatCorrectionData data = new EnemyStatCorrectionData (id, hpCorrection, atkCorrection);
            EnemyStatCorrectionDataDic.Add (data.Id, data);
        }
    }

    void ReadDungeonMonsterData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadDungeonPatternData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int monsterId = Convert.ToInt32 (values[1]);
            int limitTime = Convert.ToInt32 (values[2]);

            DungeonMonsterData data = new DungeonMonsterData (id, monsterId, limitTime);
            DungeonMonsterDataDic.Add (data.Id, data);
        }
    }

    void ReadMarketBasisData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int unitCount = Convert.ToInt32 (values[1]);
            int limitTurn = Convert.ToInt32 (values[2]);

            MarketBasisData basisData = new MarketBasisData (id, unitCount, limitTurn);
            MarketBasisDataDic.Add (basisData.Id, basisData);
        }
    }

    void ReadMarketSpecialEventData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            E_MarketEventType marketEventType = (E_MarketEventType) Convert.ToInt32 (values[1]);
            int eventValue = Convert.ToInt32 (values[2]);
            string description = values[3];

            MarketSpecialEventData specialEventData = new MarketSpecialEventData (id, marketEventType, eventValue, description);
            MarketSpecialEventDataDic.Add (specialEventData.Id, specialEventData);
        }
    }

    void ReadStatAugmenterData (string path) {
        if (string.IsNullOrEmpty (path)) {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int level = Convert.ToInt32 (values[1]);
            int augmenter = Convert.ToInt32 (values[2]);

            StatAugmenterData statAugmenterData = new StatAugmenterData (id, level, augmenter);
            StatAugmenterDataDic.Add (id, statAugmenterData);
        }
    }

    void ReadOpeningSceneDialogueData (string path) 
    {
        if (string.IsNullOrEmpty (path)) 
        {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            string dialogue = values[1];

            OpeningSceneDialogueData dialogueData = new OpeningSceneDialogueData(id, dialogue);
            OpeningSceneDialogueDataDic.Add(id, dialogueData);
        }
    }

    void ReadTutorialData (string path) 
    {
        if (string.IsNullOrEmpty (path)) 
        {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) 
        {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int detailId = Convert.ToInt32(values[1]);
            E_TutorialType type = (E_TutorialType) Convert.ToInt32 (values[2]);
            string dialogue = values[3];

            TutorialData data = new TutorialData(id, detailId,type, dialogue);

            if(!TutorialDataDic.ContainsKey(type))
                TutorialDataDic.Add(type,new Dictionary<int, TutorialData>());

            TutorialDataDic[type].Add(data.DetailId, data);
        }
    }

    void ReadTutorialSimpleData (string path) 
    {
        if (string.IsNullOrEmpty (path)) 
        {
            Debug.LogError ("ReadMarketBasisData path is null or emtpy");
            return;
        }

        string dataFullPath = string.Concat (dataDefalutPath, path);
        TextAsset assetData = Resources.Load (dataFullPath) as TextAsset;
        string[] textData = assetData.text.Split ('\n'); // 줄단위로 구분
        string strLineValue = string.Empty;
        string[] values = null;
        for (int i = 0; i < textData.Length; i++) 
        {
            strLineValue = textData[i];
            if (string.IsNullOrEmpty (strLineValue))
                return;

            if (i == 0)
                continue;

            values = strLineValue.Split (',');
            int id = Convert.ToInt32 (values[0]);
            int detailId = Convert.ToInt32(values[1]);
            E_SimpleTutorialType type = (E_SimpleTutorialType) Convert.ToInt32 (values[2]);
            string dialogue = values[3];
            dialogue = dialogue.Replace("\\n","\n");
            string imagePath = values[4].Replace("\r","");

            TutorialSimpleData data = new TutorialSimpleData(id, detailId, type, dialogue,imagePath);

            if(!SimpleTutorialDataDic.ContainsKey(type))
                SimpleTutorialDataDic.Add(type,new Dictionary<int, TutorialSimpleData>());

            SimpleTutorialDataDic[type].Add(detailId, data);
        }
    }
}