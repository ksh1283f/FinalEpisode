using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System;
using System.Text;

public class StatWindow : MonoBehaviour
{
    private const string defaultIconPath = "EtcIcon/Aura118";
    [SerializeField] Text statText;
    [SerializeField] Image hpBar;
    [SerializeField] List<Image> iconUnitList;
    [SerializeField] GameObject barObj;

    public void UpdateUnitDataList(List<UnitData> unitDataList)
    {
        #region null check
        if (unitDataList == null)
        {
            Debug.LogError("unitData is null");
            return;
        }
        if (statText == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        if (hpBar == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        if (barObj == null)
        {
            Debug.LogError("barObj is Null");
            return;
        }
        #endregion

        // todo 미리 초상화를 디폴트 이미지로 초기화 후 갱신
        for (int i = 0; i < iconUnitList.Count; i++)
            iconUnitList[i].sprite = Resources.Load<Sprite>(defaultIconPath);

        for (int i = 0; i < unitDataList.Count; i++)
            iconUnitList[i].sprite = Resources.Load<Sprite>(unitDataList[i].IconName);

        hpBar.color = Color.green;

        int allHp = 0;
        int allAtk = 0;
        int allDef = 0;
        float allCri = 0f;
        float allSpd = 0f;

        for (int i = 0; i < unitDataList.Count; i++)
        {
            UnitData unitData = unitDataList[i];
            allHp += unitData.Hp;
            allAtk += unitData.Atk;
            allCri += unitData.Cri;
            allDef += unitData.Def;

            // todo 방어력 상승 특성 유무 체크
            if(CharacterPropertyManager.Instance.SelectedUtilProperty!= null 
            && CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.WarriorUtilMaserty_AdditionalDefense
            && unitData.CharacterType == E_CharacterType.Warrior)
                allDef += CharacterPropertyManager.Instance.SelectedUtilProperty.EffectValue;
            else if (CharacterPropertyManager.Instance.SelectedUtilProperty != null
            && CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.WarlockUtilMaserty_Healing
            && unitData.CharacterType == E_CharacterType.Warlock)
                allCri += CharacterPropertyManager.Instance.SelectedUtilProperty.EffectValue;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("ATK: ");
        sb.Append(allAtk);
        sb.AppendLine();
        sb.Append("DEF: ");
        sb.Append(allDef);
        sb.AppendLine();
        sb.Append("CRI: ");
        sb.Append(allCri);
        sb.Append("%");
        sb.AppendLine();
        sb.Append("SPD: ");
        sb.Append(allSpd);
        sb.Append("%");
        sb.AppendLine();
        statText.text = sb.ToString();
    }

    public void UpdateHpBar(float value)
    {
        if (barObj == null)
            return;

        barObj.transform.localScale = new Vector3(value, 1, 1);
    }
}
