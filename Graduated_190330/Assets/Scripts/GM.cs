using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class GM : MonoBehaviour
{
    public UnitTest unit;
    public string callMethod = "GetDamage";

    private void Start()
    {
        Type type = typeof(UnitTest);
        object instance = Activator.CreateInstance(type);

        MethodInfo test = typeof(UnitTest).GetMethod(callMethod);
        test.Invoke(unit, new object[] {10 });
    }

}
