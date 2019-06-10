using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFloatV : FloatV
{
    public float fixedValue = 0;
    public override float ReturnValue()
    {
        return fixedValue;
    }
}
