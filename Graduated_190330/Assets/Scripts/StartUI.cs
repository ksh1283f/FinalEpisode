using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : uiSingletone<StartUI> , IBaseUI
{
	[SerializeField] Button btnStart;
    [SerializeField] Button btnOption;
    [SerializeField] Button btnExit;

    public Action OnClickedBtnStart { get; set; }
    public Action OnClickedBtnOption { get; set; }
    public Action OnClickedBtnExit { get; set; }

    protected override void Awake()
    {
        uiType = E_UIType.Start;
        base.Awake();
    }

    void Start()
    {
        btnStart.onClick.AddListener(() => { OnClickedBtnStart.Execute(); });
        btnOption.onClick.AddListener(() => { OnClickedBtnOption.Execute(); });
        btnExit.onClick.AddListener(() => { OnClickedBtnExit.Execute(); });

        // scene directing
        btnStart.gameObject.SetActive(false);
        btnOption.gameObject.SetActive(false);
        btnExit.gameObject.SetActive(false);
    }

    public void ShowBtns()
    {
        btnStart.gameObject.SetActive(true);
        btnOption.gameObject.SetActive(true);
        btnExit.gameObject.SetActive(true);
    }

}
