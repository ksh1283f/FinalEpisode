using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : uiSingletone<MessageUI>, IBaseUI
{
    [SerializeField] Text titleText;
    [SerializeField] Text contentText;
    [SerializeField] Button btnOK;
    private const int needDataCount = 2;

    public Action OnClickedBtnOK { get; set; }
    protected override void Awake()
    {
        uiType = E_UIType.ShowMessage;
        base.Awake();
    }

    void Start()
    {
        if (titleText == null)
            return;

        if (contentText== null)
            return;

        if (btnOK==null)
            return;

        btnOK.onClick.AddListener(() => { OnBtnOk(); });
        Close();
    }

    public override void Show(string[] dataList)
    {
        base.Show(dataList);
        if (dataList == null)
            return;

        if (dataList.Length != 2)
        {
            Debug.LogError("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        string title = dataList[0];
        string contentText = dataList[1];
        SetMessageUI(title, contentText);
    }

    void OnBtnOk()
    {
        SoundManager.Instance.PlayButtonSound();
        Close();
    }

    void SetMessageUI(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
    }
}
