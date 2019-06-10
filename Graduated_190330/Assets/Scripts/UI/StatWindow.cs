using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System;
using System.Text;

public class StatWindow : MonoBehaviour 
{
	[SerializeField] Text statText;
	[SerializeField] Image hpBar;
	[SerializeField] Image iconArm1;
	[SerializeField] Image iconArm2;
	[SerializeField] Image iconArm3;
	[SerializeField] GameObject barObj;


	void Start () 
	{
		
	}

	public void UpdateUnitDataList(List<UnitData> unitDataList)
	{
		#region null check
		if(unitDataList == null)
		{
			Debug.LogError("unitData is null");
			return;
		}
		if (statText == null)
		{
			Debug.LogError("unitData is null");
			return;
		}
		
		if(hpBar == null)
		{
			Debug.LogError("unitData is null");
			return;
		}

		if(iconArm1 == null)
		{
			Debug.LogError("unitData is null");
			return;
		}

		if(iconArm2 == null)
		{
			Debug.LogError("unitData is null");
			return;
		}

		if(iconArm3 == null)
		{
			Debug.LogError("unitData is null");
			return;
		}

		if(barObj == null)
		{
			Debug.LogError("barObj is Null");
			return;
		}
		#endregion
		hpBar.color = Color.green;

		int allHp = 0;
		int allAtk = 0;
		int allDef = 0;
		float allCri = 0f;
		float allSpd = 0f;

		for (int i = 0; i < unitDataList.Count; i++)
		{
			UnitData unitData = unitDataList[i];
			allHp+= unitData.Hp;
			allAtk+=unitData.Atk;
			allDef+= unitData.Def;
		}

		StringBuilder sb = new StringBuilder();
		sb.Append("ATK: ");
		sb.Append(allAtk);
		sb.AppendLine();
		sb.Append("DEF: ");
		sb.Append(allDef);
		sb.AppendLine();
		sb.Append("CRI: ");
		sb.Append(allCri);
		sb.Append("%");
		sb.AppendLine();
		sb.Append("SPD: ");
		sb.Append(allSpd);
		sb.Append("%");
		sb.AppendLine();
		statText.text = sb.ToString();		
	}

	public void UpdateHpBar(float value)
	{
		if(barObj == null)
			return;

		barObj.transform.localScale = new Vector3(value, 1,1);
	}
}
