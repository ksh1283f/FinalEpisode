using System.Collections;
using System.Collections.Generic;

public class DungeonMonsterData
{
    public int Id {get; private set;}
    public int MonsterId {get;private set;}

    public DungeonMonsterData(int id, int monsterId)
    {
        Id = id;
        MonsterId = monsterId;
    }
}
