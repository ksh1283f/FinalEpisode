using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Graduate.GameData.UnitData;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSimpleInfo : MonoBehaviour
{
    [SerializeField] Image classIcon;
    [SerializeField] Button btnContent;
    [SerializeField] Image selectedMark;
    [SerializeField] Text infoText;

    public UnitData unitData { get; private set; }
    private bool isSelected = true;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            if (value == isSelected)
                return;

            isSelected = value;
            if (selectedMark != null)
                selectedMark.gameObject.SetActive(isSelected);
        }
    }

    public Action<UnitData, bool> OnClickedContent { get; set; }

    void Start()
    {
        if (btnContent != null)
            btnContent.onClick.AddListener(() => { OnClicked(unitData); });
    }

    public void SetData(UnitData data)
    {
        if (data == null)
        {
            Debug.LogError("Unitdata is null");
            return;
        }

        if (classIcon == null)
        {
            Debug.LogError("class icon is null");
            return;
        }

        if (selectedMark == null)
        {
            Debug.LogError("selelctedMark is null");
            return;
        }

        if (infoText == null)
        {
            Debug.LogError("infoText is null");
            return;
        }

        unitData = data;
        classIcon.sprite = Resources.Load<Sprite>(data.IconName);
        IsSelected = false;
        StringBuilder sb = new StringBuilder();
        sb.Append("직업: ");
        switch (data.CharacterType)
        {
            case E_CharacterType.Warrior:
                sb.Append("전사");
                break;
            case E_CharacterType.Mage:
                sb.Append("마법사");
                break;
            case E_CharacterType.Warlock:
                sb.Append("흑마법사");
                break;
            case E_CharacterType.Rogue:
                sb.Append("도적");
                break;
            default:
                Debug.LogError("Character type is invalid");
                return;
        }

        // todo 만약 출전중인 용병이라면
        if (UserManager.Instance.UserInfo.SelectedUnitDic.ContainsKey(data.Id))
            sb.Append("(출전)");

        sb.AppendLine();
        sb.Append("Lv. ");
        sb.Append(data.Level);

        infoText.text = sb.ToString();
    }

    void OnClicked(UnitData data)
    {
        IsSelected = !IsSelected;
        OnClickedContent.Execute(data, IsSelected);
    }
}
