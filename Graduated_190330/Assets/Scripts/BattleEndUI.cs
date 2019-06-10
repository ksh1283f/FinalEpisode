using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofle.Tween;
using UnityEngine.UI;

public class BattleEndUI : uiSingletone<BattleEndUI>, IBaseUI
{
	[SerializeField] UITweenColorAlpha tweenTextAlpha;
	Text text;

    protected override void Awake()
    {
        uiType = E_UIType.BattleEnd;
        base.Awake();
    }

    void Start()
    {
        text = tweenTextAlpha.GetComponent<Text>();
		text.text = string.Empty;

		// GameManager.Instance.OnExecuteResult += ShowResultText;
		// end 이벤트로 게임화면을 바꿀수 있다.
	}


	public void ShowResultText(bool isClear)
	{
		if(text == null)
			return;

		if(tweenTextAlpha == null)
			return;

		if(isClear)
			text.text = "Clear";
		else
			text.text = "Fail";

		tweenTextAlpha.PlayForward();
	}
}
