using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyContents : MonoBehaviour
{
    public E_LobbyContents contentsType;
    public Action<E_LobbyContents> OnExecuteContets { get; set; }
}
