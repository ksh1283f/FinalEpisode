using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPropertyUI : uiSingletone<CharacterPropertyUI>, IBaseUI
{
    [SerializeField] Text titleText;
    [SerializeField] PropertyImage AtkImage;
    [SerializeField] PropertyImage UtilImage;
    [SerializeField] PropertyImage DefImage;
	[SerializeField] Button btnOk;

    public Action<E_PropertyType> OnChangeToggleValue { get; set; }
    protected override void Awake()
    {
        uiType = E_UIType.CharacterProperty;
        base.Awake();

    }

    void Start()
    {
        btnOk.onClick.AddListener(() => { OnBtnOk(); });

        Close();
    }

	void OnBtnOk()
	{
		Close();
	}
	
    public override void Show(string[] dataList)
    {
        base.Show(dataList);
        if (dataList == null)
            return;

        string title = dataList[0];
		if(titleText != null)
			titleText.text = title;
        UpdateData();
    }

    void UpdateData()
    {
        if (AtkImage == null)
        {
            Debug.LogError("AtkImage is nul");
            return;
        }

        if (UtilImage == null)
        {
            Debug.LogError("UtilImage is nul");
            return;
        }

        if (DefImage == null)
        {
            Debug.LogError("DefImage is nul");
            return;
        }

        foreach (var item in CharacterPropertyManager.Instance.propertyDic)
        {
            switch (item.Key)
            {
                case E_PropertyType.Atk:
                    AtkImage.SetProperty(item.Value);
                    break;

                case E_PropertyType.Util:
                    UtilImage.SetProperty(item.Value);
                    break;

                case E_PropertyType.Def:
                    DefImage.SetProperty(item.Value);
                    break;
            }
        }
    }
}