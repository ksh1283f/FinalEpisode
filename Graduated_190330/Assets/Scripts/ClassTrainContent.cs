using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System.Text;
using System;

public class ClassTrainContent : MonoBehaviour
{
    [SerializeField] Text descText;
    [SerializeField] Image classIcon;
    [SerializeField] Text valueText;
    [SerializeField] Button btnShowDetail;
    public UnitData UnitData { get; private set; }
    public Action<UnitData> OnClickedContent { get; set; }
    public bool IsSelected { get; private set; }
    void Start()
    {
        if (btnShowDetail != null)
            btnShowDetail.onClick.AddListener(OnSelectedContent);

        IsSelected = false;
    }

    public void SetUnitData(UnitData unitData)
    {
        if (unitData == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        UnitData = unitData;
        SetContent(UnitData);
    }

    void SetContent(UnitData unitData)
    {
        if (descText == null)
            return;

        if (valueText == null)
            return;

        if (classIcon == null)
            return;

        // set class icon
        Sprite temp =  Resources.Load<Sprite>(unitData.IconName);
        classIcon.sprite =temp;

        // set text
        StringBuilder sb = new StringBuilder();
        sb.Append("직업: ");
        switch (unitData.CharacterType)
        {
            case E_CharacterType.Warrior:
                sb.Append("전사");
                sb.AppendLine();
                sb.Append("체력, 방어력");
                break;

            case E_CharacterType.Mage:
                sb.Append("마법사");
                sb.AppendLine();
                sb.Append("공격력, 치명타");
                break;

            case E_CharacterType.Warlock:
                sb.Append("흑마법사");
                sb.AppendLine();
                sb.Append("체력, 치명");
                break;

            case E_CharacterType.Rogue:
                sb.Append("도적");
                sb.AppendLine();
                sb.Append("공격력, 속도");
                break;

            default:
                sb.Append("Class type ERROR: ");
                sb.Append(unitData.CharacterType);
                return;
        }

        descText.text = sb.ToString();
        sb.Clear();
        sb.Append("구매가: ");
        sb.Append(unitData.Price);
        valueText.text = sb.ToString();
    }

    void OnSelectedContent()
    {
        if (UnitData == null)
            return;

        OnClickedContent.Execute(UnitData);
        IsSelected = true;
    }

    public void ReleaseSelected()
    {
        if(!IsSelected)
            return;
        IsSelected = false;
    }
}
