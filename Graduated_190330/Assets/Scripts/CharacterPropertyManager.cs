using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BattlePropertyType {
    None,
    Common,
    Util,
    Healing,
}

public enum E_BattleBuffType
{
    None,
    Clocking,
    DecreasDamage,
    Invincible,
    DrainHealthPerDamage,
    CheatDeath,
    SpellReflection,
}

public class BattleBuff
{
    public E_BattleBuffType BattleBuffType {get; private set;}
    public int EffectValue {get; private set;}
    public bool IsReady;    // 사용가능
    public bool IsUsed; // 사용한상태

    public BattleBuff(E_BattleBuffType buffType, int effectValue)
    {
        BattleBuffType = buffType;
        EffectValue = effectValue;
        IsReady = true;
        IsUsed = false;
    }

    public void ActiveBuff()
    {
        IsReady = false;
        IsUsed = true;
    }

    public void ReadyToUseBuff()
    {
        IsReady = true;
        IsUsed = false;
    }
}

public class CharacterPropertyManager : Singletone<CharacterPropertyManager> {
    public Dictionary<E_PropertyEffectType, CharacterProperty> CommonPropertyDic { get; private set; }

    public Dictionary<E_PropertyEffectType, CharacterProperty> UtilPropertyDic { get; private set; }
    public Dictionary<E_PropertyEffectType, CharacterProperty> HealingPropertyDic { get; private set; }

    public CharacterProperty SelectedCommonProperty { get; private set; }

    public CharacterProperty SelectedUtilProperty { get; private set; }
    public CharacterProperty SelectedHealingProperty { get; private set; }

    [SerializeField] E_PropertyEffectType seletedCommonType = E_PropertyEffectType.None; // 현재 선택된 프로퍼티 인스펙터에서 보여주기
    [SerializeField] E_PropertyEffectType seletedUtilType = E_PropertyEffectType.None; // 현재 선택된 프로퍼티 인스펙터에서 보여주기
    [SerializeField] E_PropertyEffectType seletedHealingType = E_PropertyEffectType.None; // 현재 선택된 프로퍼티 인스펙터에서 보여주기

    void Start () {
        CommonPropertyDic = new Dictionary<E_PropertyEffectType, CharacterProperty> ();
        UtilPropertyDic = new Dictionary<E_PropertyEffectType, CharacterProperty> ();
        HealingPropertyDic = new Dictionary<E_PropertyEffectType, CharacterProperty> ();

        InitProperty ();
    }

    // 특성데이터 초기화
    void InitProperty () {
        foreach (var item in GameDataManager.Instance.BattlePropertyDic[E_BattlePropertyType.Common])
            CommonPropertyDic.Add (item.Key, item.Value);

        foreach (var item in GameDataManager.Instance.BattlePropertyDic[E_BattlePropertyType.Util])
            UtilPropertyDic.Add (item.Key, item.Value);

        foreach (var item in GameDataManager.Instance.BattlePropertyDic[E_BattlePropertyType.Healing])
            HealingPropertyDic.Add (item.Key, item.Value);
    }

    // 유저정보가 바뀔때
    public void OnChangedBattleProperty (E_PropertyEffectType type) {
        E_BattlePropertyType battlePropertyType = type.GetBattlePropertyType ();
        switch (battlePropertyType) {
            case E_BattlePropertyType.None:
                Debug.LogError ("Invalid battlePropertyType:" + battlePropertyType);
                return;

            case E_BattlePropertyType.Common:
                if (SelectedCommonProperty != null && SelectedCommonProperty.EffectType.Equals (type))
                    return;

                SelectedCommonProperty = CommonPropertyDic[type];
                seletedCommonType = type;
                break;

            case E_BattlePropertyType.Util:
                if (SelectedUtilProperty != null && SelectedUtilProperty.EffectType.Equals (type))
                    return;

                SelectedUtilProperty = UtilPropertyDic[type];
                seletedUtilType = type;
                break;

            case E_BattlePropertyType.Healing:
                if (SelectedHealingProperty != null && SelectedHealingProperty.EffectType.Equals (type))
                    return;

                SelectedHealingProperty = HealingPropertyDic[type];
                seletedHealingType = type;
                break;
        }

        // 유저정보에 변경된 사항 저장
        UserManager.Instance.SetPropertyInUserInfo (type);
    }

    // 유저정보에 저장된 특성 반영
    public void UpdatePropertyFromUserInfo () {
        UserInfo userInfo = UserManager.Instance.UserInfo;
        if (userInfo == null)
            return;

        if (UserManager.Instance.UserInfo.CommonPropertyType != E_PropertyEffectType.None) {
            SelectedCommonProperty = CommonPropertyDic[userInfo.CommonPropertyType];
            seletedCommonType = userInfo.CommonPropertyType;
        }

        if (UserManager.Instance.UserInfo.UtilPropertyType != E_PropertyEffectType.None) {
            SelectedUtilProperty = UtilPropertyDic[userInfo.UtilPropertyType];
            seletedUtilType = userInfo.UtilPropertyType;
        }

        if (UserManager.Instance.UserInfo.HealingPropertyType != E_PropertyEffectType.None) {
            SelectedHealingProperty = HealingPropertyDic[userInfo.HealingPropertyType];
            seletedHealingType = userInfo.HealingPropertyType;
        }
    }

}