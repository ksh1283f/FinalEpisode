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
    public Dictionary<int,UnitData> SelectedUnitDic;
    public List<SerializableUnitData> UnitList; // dictionary는 serialize가 불가능
    public List<SerializableUnitData> SelectedUnitList;
    public E_PropertyEffectType CommonPropertyType; // 전투 특성
    public E_PropertyEffectType UtilPropertyType;
    public E_PropertyEffectType HealingPropertyType;

    public int Atk;
    public int Def;
    public int Cri; // 표기는 퍼센트로 변환 (4당 1퍼)
    public int Spd; // 표기는 퍼센트로 변환 (4당 1퍼)

    public int BestDungeonStep; // 최고 클리어 던전 단수
    public List<bool> TutorialClearList;
    public bool IsAllTutorialClear;
    public bool IsSawTheEnding; // 엔딩씬을 봤는지
}
