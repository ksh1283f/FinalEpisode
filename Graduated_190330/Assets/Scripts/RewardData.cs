using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardData
{
    public int DungeonId { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }
    public string Description{get; private set;}

    public RewardData(int dungeonId, int gold, int exp, string description)
    {
        DungeonId = dungeonId;
        Gold = gold;
        Exp = exp;
        Description = description;
    }
}
