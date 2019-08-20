using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketBasisData 
{
    public int Id { get; private set; }
    public int UnitCount { get; private set; }
    public int LimitTurn { get; private set; }
    

    public MarketBasisData(int id, int unitCount, int limitTurn)
    {
        Id = id;
        UnitCount = unitCount;
        LimitTurn = limitTurn;
    }
}
