using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtil
{
    public static void Execute(this Action action)
    {
        if (action == null)
            return;

        action();
    }

    public static void Execute<T>(this Action<T> action, T t1)
    {
        if (action == null)
            return;

        action(t1);
    }

    public static void Execute<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
    {
        if (action == null)
            return;

        action(t1, t2);
    }

    public static void Execute<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
    {
        if (action == null)
            return;

        action(t1, t2, t3);
    }

    public static void Execute<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        if (action == null)
            return;

        action(t1, t2, t3, t4);
    }

    public static bool IsHealable(this E_Class classType)
    {
        switch (classType)
        {
            case E_Class.Paladin:
            case E_Class.Priest:
                return true;
        }

        return false;
    }
}
