using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputUI : uiSingletone<InputUI>, IBaseUI
{
    [SerializeField] InputField inputField;
    [SerializeField] Button btnOk;

    public Action<string> OnClickedBtnOk { get; set; }
    protected override void Awake()
    {
        uiType = E_UIType.Input;
        base.Awake();
    }

    void Start()
    {
        if (inputField == null)
            return;

        if (btnOk == null)
            return;

        
        btnOk.onClick.AddListener(() => { OnBtnOk(); });

        Close();
    }

    void OnBtnOk()
    {
        string inputValue = inputField.text;
        if (string.IsNullOrEmpty(inputValue))
            return;

        OnClickedBtnOk.Execute(inputValue);
        Close();
    }
}