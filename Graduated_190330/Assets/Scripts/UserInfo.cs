using System;
using System.Collections;
using System.Collections.Generic;
using Graduate.GameData.UnitData;

/// <summary>
/// User Data
/// </summary>
[Serializable]
public class UserInfo
{
    public string UserName;
    public int TeamLevel;
    public int Exp;
    public int Gold;
    public Dictionary<int, UnitData> UnitDic;
    public List<SerializableUnitData> UnitList; // dictionary는 serialize가 불가능
    // public List<int> UnitCommonPropertyList;    // 유닛 특성 리스트
    public E_PropertyType PropertyType; // 전투 특성

    public bool isClearTutorial;    // 유저 튜토리얼 클리어 유무 

    public int Atk;
    public int Def;
    public int Cri; // 표기는 퍼센트로 변환 (4당 1퍼)
    public int Spd; // 표기는 퍼센트로 변환 (4당 1퍼)

    public int BestDungeonStep; // 최고 클리어 던전 단수

    // todo 장착 아이템
    public List<Item> EquipItemList;
}
