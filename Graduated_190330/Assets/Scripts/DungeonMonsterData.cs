﻿using System.Collections;
using System.Collections.Generic;

public class DungeonMonsterData
{
    public int Id { get; private set; }
    public int BossMonsterId { get; private set; }
    public List<int> MinionMonsterIds {get; private set;}
    public int LimitTime { get; private set; }
    public string SceneName {get; private set;}

    public DungeonMonsterData(int id, int monsterId, List<int> minions,int limitTime, string sceneName)
    {
        Id = id;
        BossMonsterId = monsterId;
        MinionMonsterIds = minions;
        LimitTime = limitTime;
        SceneName = sceneName;
    }

    public object ShallowCopy()
    {
        return this.MemberwiseClone();
    }
}
