using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofle.Tween;
using UnityEngine.UI;
using System;
using System.Text;

public class BattleEndUI : uiSingletone<BattleEndUI>, IBaseUI
{
    [SerializeField] Button btnOK;
    [SerializeField] Text text;
    public Action OnClickedBtnOk { get; set; }

    protected override void Awake()
    {
        uiType = E_UIType.BattleEnd;
        base.Awake();

        text.text = string.Empty;
        btnOK.onClick.AddListener(() => { OnClickedBtnOk.Execute(); });
		Text btnText = btnOK.GetComponentInChildren<Text>();
		btnText.text = "마을로 가기";
    }

    void Start()
    {

        // GameManager.Instance.OnExecuteResult += ShowResultText;
        // end 이벤트로 게임화면을 바꿀수 있다.
    }


    public void ShowResultWindow(bool isClear, RewardData rewardData)
    {
        if (text == null)
            return;

        if (rewardData == null)
            return;

        if (btnOK == null)
            return;
        StringBuilder sb = new StringBuilder();


        if (isClear)
        {
            sb.Append("Clear");
            sb.AppendLine();
            sb.Append("Gold: +");
            sb.Append(rewardData.Gold);
            sb.AppendLine();
            sb.Append("Exp: +");
            sb.Append(rewardData.Exp);
        }
        else
        {
            sb.Append("Fail");
            sb.AppendLine();
            sb.Append("Gold: +0");
            sb.AppendLine();
            sb.Append("Exp: +0");
        }

        text.text = sb.ToString();
    }
}
