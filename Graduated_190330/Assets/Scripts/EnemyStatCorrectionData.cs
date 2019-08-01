using System.Collections;
using System.Collections.Generic;

public class EnemyStatCorrectionData
{
    public int Id { get; private set; }
    public int HpCorrection { get; private set; }
    public int AtkCorrection { get; private set; }

    public EnemyStatCorrectionData(int id, int hpCorrection, int atkCorrection)
    {
        Id = id;
        HpCorrection = hpCorrection;
        AtkCorrection = atkCorrection;
    }
}
