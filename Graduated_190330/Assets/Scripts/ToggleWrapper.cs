using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_ToggleType
{
    None,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
}

public class ToggleWrapper : MonoBehaviour
{
    [SerializeField] E_ToggleType toggleType;
    public Toggle toggle;
    public E_ToggleType ToggleType { get { return toggleType; } }
}
