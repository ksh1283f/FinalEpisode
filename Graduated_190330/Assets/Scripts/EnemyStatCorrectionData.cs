using System.Collections;
using System.Collections.Generic;

public class EnemyStatCorrectionData
{
    public int Id { get; private set; }
    public int Correction { get; private set; }

    public EnemyStatCorrectionData(int id, int correction)
    {
        Id = id;
        Correction = correction;
    }
}
