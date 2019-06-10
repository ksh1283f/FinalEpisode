using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="refStat",menuName ="V/Float",order = 0)]
public class RefStatFloatV : FloatV
{
    public TUnit refUnit;
    public override float ReturnValue()
    {
        return refUnit.GetStat();
    }
}
