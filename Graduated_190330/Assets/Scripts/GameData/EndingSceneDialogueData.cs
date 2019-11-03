using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneDialogueData
{
    public int  Id {get; private set;}
    public string Dialogue{get; private set;}

    public EndingSceneDialogueData(int id, string dialogue)
    {
        Id = id;
        Dialogue = dialogue;
    }
}
