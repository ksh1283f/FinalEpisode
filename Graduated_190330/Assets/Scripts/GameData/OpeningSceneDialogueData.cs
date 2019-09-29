using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSceneDialogueData
{
    public int Id {get; private set;}
    public string Dialogue {get; private set;}

    public OpeningSceneDialogueData(int id, string dialogue)
    {
        Id = id;
        Dialogue = dialogue;
    }
}
