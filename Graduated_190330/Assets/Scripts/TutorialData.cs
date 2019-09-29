using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_TutorialDetailType
{
    ShowDialogue,
    OnClick,   
}

public class TutorialData
{
    public int Id {get; private set;}
    public int DetailId{get;private set;}
    public E_TutorialType TutorialType {get; private set;}
    public string Dialogue {get;private set;}

    public TutorialData(int id, int detailId, E_TutorialType type, string dialogue)
    {
        Id = id;
        DetailId = detailId;
        TutorialType = type;
        Dialogue = dialogue;
    }
}

[Serializable]
public class TutorialDataSerialized
{
    public int DetailId;
    public GameObject Object;
    public E_TutorialType tutorialType;
    //public E_TutorialDetailType detailType;
    public Vector3 camPosition;
    public bool isNeedDelay;
    public bool isCompleteReady;
    public float delayTime;
    public string Dialogue;
    public bool IsThisTutorialEnd;
     
    public Action ExecuteEvent{ get; set; }
}
