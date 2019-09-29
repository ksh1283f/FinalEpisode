using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSimpleData
{
    public int Id {get; private set;}
    public int DetailId {get; private set;}
    public E_SimpleTutorialType SimpleTutorialType { get; private set;}
    public string Dialogue {get; private set;}
    public string ImagePath {get; private set;}

    public TutorialSimpleData(int id, int detailID, E_SimpleTutorialType type,string dialogue, string imagePath)
    {
        Id = id;
        DetailId = detailID;
        SimpleTutorialType = type;
        Dialogue = dialogue;
        ImagePath = imagePath;
    }
}
