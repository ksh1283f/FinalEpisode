using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyImages : MonoBehaviour {
    [SerializeField] List<PropertyImage> propertyList = new List<PropertyImage> ();
    [SerializeField] E_BattlePropertyType battlePropertyType = E_BattlePropertyType.None;
    private E_PropertyEffectType propertyEffectType;

    public void UpdateProperties (CharacterProperty argProperty, E_PropertyEffectType type) {
        E_PropertyEffectType thisEffectType = E_PropertyEffectType.None;
        switch (battlePropertyType)
        {
            case E_BattlePropertyType.Common:
                thisEffectType = UserManager.Instance.UserInfo.CommonPropertyType;
                break;   

            case E_BattlePropertyType.Util:
                thisEffectType = UserManager.Instance.UserInfo.UtilPropertyType;
                break;

            case E_BattlePropertyType.Healing:
                thisEffectType = UserManager.Instance.UserInfo.HealingPropertyType;
                break;
        }

        for (int i = 0; i < propertyList.Count; i++) {
            if (type == propertyList[i].propertyEffectType) {
                propertyList[i].SetProperty (argProperty);
                
                if (thisEffectType != E_PropertyEffectType.None && thisEffectType == type)
                    propertyList[i].PropertyToggle.isOn = true;
                break;
            }
        }
    }

}