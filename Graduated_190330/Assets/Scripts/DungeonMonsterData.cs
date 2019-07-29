using System.Collections;
using System.Collections.Generic;

public class DungeonMonsterData
{
    public int Id { get; private set; }
    public int MonsterId { get; private set; }
    public int LimitTime { get; private set; }

    public DungeonMonsterData(int id, int monsterId, int limitTime)
    {
        Id = id;
        MonsterId = monsterId;
        LimitTime = limitTime;
    }

    public object ShallowCopy()
    {
        return this.MemberwiseClone();
    }
}
