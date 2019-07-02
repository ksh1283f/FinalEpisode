﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPropertyManager : Singletone<CharacterPropertyManager>
{
    public Dictionary<E_PropertyType, CharacterProperty> propertyDic {get; private set;}
    public CharacterProperty SelectedProperty { get; private set; }
    [SerializeField] E_PropertyType seletedType = E_PropertyType.None;// todo 현재 선택된 프로퍼티 인스펙터에서 보여주기

    void Start()
    {
        propertyDic = new Dictionary<E_PropertyType, CharacterProperty>();
        InitProperty();
        //todo load from user info
    }

    // Test code
    void InitProperty()
    {
        string imagePath = "SkillIcon/RogueIcons/Transparent/SIH_18_1";
        string name = "추가 공격자원";
        string description = "공격자원버튼 클릭 시 추가로 자원을 1개 더 얻습니다.";
        E_PropertyEffectType effectType = E_PropertyEffectType.AdditionalAtkResource;
        E_PropertyType propertyType = E_PropertyType.Atk;
        int effectValue = 1;
        CharacterProperty atkProperty = new CharacterProperty(imagePath, name, description, propertyType, effectType, effectValue);

        imagePath = "SkillIcon/RogueIcons/Filled/SIH_10";
        name = "추가 유틸자원";
        description = "유틸자원버튼 클릭 시 추가로 자원을 1개 더 얻습니다.";
        effectType = E_PropertyEffectType.AdditionalUtilResource;
        propertyType = E_PropertyType.Util;
        effectValue = 1;
        CharacterProperty utilProperty = new CharacterProperty(imagePath, name, description, propertyType, effectType, effectValue);

        imagePath = "SkillIcon/WarriorIcons/Filled/Reflect_2";
        name = "추가 방어자원";
        description = "방어자원버튼 클릭 시 추가로 자원을 1개 더 얻습니다.";
        effectType = E_PropertyEffectType.AdditionalDefResource;
        propertyType = E_PropertyType.Def;
        effectValue = 1;
        CharacterProperty defProperty = new CharacterProperty(imagePath, name, description, propertyType, effectType, effectValue);

        propertyDic.Add(E_PropertyType.Atk, atkProperty);
        propertyDic.Add(E_PropertyType.Util, utilProperty);
        propertyDic.Add(E_PropertyType.Def, defProperty);
    }

    public void OnChangedProperty(E_PropertyType type)
    {
        if (SelectedProperty != null && SelectedProperty.PropertyType.Equals(type))
            return;

        SelectedProperty = propertyDic[type];
        seletedType = type;
    }

}
