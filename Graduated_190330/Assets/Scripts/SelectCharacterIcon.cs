using System.Collections;
using System.Collections.Generic;
using Graduate.GameData.UnitData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterIcon : MonoBehaviour
{
    private const string defaultIconPath = "EtcIcon/Aura118";

    [SerializeField] Image classIcon;
    [SerializeField] Image selectMark;
    [SerializeField] Button btnSelect;
    public UnitData unitData { get; private set; }
    public Action<UnitData> OnClickedContent { get; set; }

    private bool isSelected = true;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            if (value == isSelected)
                return;

            isSelected = value;
            if (selectMark != null)
                selectMark.enabled = isSelected;
        }
    }

    void Start()
    {
        if (btnSelect != null)
            btnSelect.onClick.AddListener(OnClickedBtnSelect);
    }

    public void SetUnitData(UnitData data)
    {
        if (data == null)
        {
            unitData = null;
            IsSelected = false;
            classIcon.sprite = Resources.Load<Sprite>(defaultIconPath);
            return;
        }

        unitData = data;
        classIcon.sprite = Resources.Load<Sprite>(unitData.IconName);
        
    }

    void OnClickedBtnSelect()
    {
        IsSelected = !IsSelected;

        // todo ui의 selectedUnitInSelectList갱신해주기
        OnClickedContent.Execute(unitData);
    }
}
