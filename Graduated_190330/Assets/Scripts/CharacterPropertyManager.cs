using System.Collections;
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
    }

    void InitProperty()
    {
        foreach (var item in GameDataManager.Instance.BattlePropertyDic)
            propertyDic.Add(item.Value.PropertyType,item.Value);
    }

    public void OnChangedProperty(E_PropertyType type)
    {
        if (SelectedProperty != null && SelectedProperty.PropertyType.Equals(type))
            return;

        SelectedProperty = propertyDic[type];
        seletedType = type;

        // 유저정보에 변경된 사항 저장
        UserManager.Instance.SetPropertyInUserInfo(type);
    }

    public void UpdatePropertyFromUserInfo()
    {
        UserInfo userInfo = UserManager.Instance.UserInfo;
        if(userInfo == null || userInfo.PropertyType == E_PropertyType.None)
            return;
            
        SelectedProperty = propertyDic[userInfo.PropertyType];
        seletedType = userInfo.PropertyType;
    }

}
