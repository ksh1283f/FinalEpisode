using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using Lofle.Tween;

public class BattlePhaseInfoUI : uiSingletone<BattlePhaseInfoUI>, IBaseUI
{
	[SerializeField] Text phaseText;
	[SerializeField] Text directingText;
	[SerializeField] TweenTransform tweenDirectingText;

	string strPhase = string.Empty;
    protected override void Awake()
    {
        uiType = E_UIType.BattlePhaseInfo;
        base.Awake();
    }

    void Start()
    {
        GameManager.Instance.OnChangedGamePhase += ShowPhase;
    }

    void ShowPhase(E_PhaseType phaseType)	// phase 변수들이 모두 제대로 초기화된 후에 호출되어야한다.
	{
		StringBuilder sb = new StringBuilder();
		// if(phaseType == E_PhaseType.None)
		// 	phaseType = E_PhaseType.First;
		
		sb.Append((int)phaseType);
		sb.Append(" / ");
		sb.Append((int)GameManager.Instance.LastPhase);
		strPhase = sb.ToString();
		phaseText.text = strPhase;

		// todo 트윈 연출작업
		// directing
		// if(directingText == null)
		// 	return;

		// if(tweenDirectingText == null)
		// return;

		// StartCoroutine(TweeningText());
	}

	IEnumerator TweeningText()
	{
		tweenDirectingText.PlayForward();

		yield return new WaitForSeconds(1f);

		tweenDirectingText.PlayReverse();
	}

}
