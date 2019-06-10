using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum E_PhaseType
{
	None,
	First,
	Second,
	Third,
	Fourth,
}

public class PhaseChecker : MonoBehaviour 
{
	// 인스펙터에서 설정하기
	[SerializeField] private E_PhaseType phaseType = E_PhaseType.None;
	public E_PhaseType PhaseType { get { return phaseType;} }
	public Action<E_PhaseType> OnStartPhase{ get; set; }

	void OnTriggerEnter(Collider collider)
	{
		if(collider.CompareTag("Player"))
		{
			OnStartPhase.Execute(PhaseType);
		}
	}
}
