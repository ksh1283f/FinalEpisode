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

    public static T Parse<T>(this System.Enum aEnum, string aText)
    {
        return (T)System.Enum.Parse(typeof(T), aText);
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

    public static E_BattlePropertyType GetBattlePropertyType(this E_PropertyEffectType type)
    {
        switch (type)
        {
            case E_PropertyEffectType.AdditionalAtkResource:
            case E_PropertyEffectType.AdditionalDefResource:
            case E_PropertyEffectType.AdditionalUtilResource:
                return E_BattlePropertyType.Common;    

            case E_PropertyEffectType.WarriorUtilMaserty_AdditionalDefense:
            case E_PropertyEffectType.MageUtilMaserty_HOT:
            case E_PropertyEffectType.WarlockUtilMaserty_IncreaseCri:
            case E_PropertyEffectType.RogueUtilMaserty_Clocking:
                return E_BattlePropertyType.Util;

            case E_PropertyEffectType.WarriorHealingMaserty_DecreaseDamageFromEnemy:
            case E_PropertyEffectType.WarriorHealingMastery_SpellReflection:
            case E_PropertyEffectType.MageHealingMaserty_Invincible:
            case E_PropertyEffectType.WarlockHealingMaserty_DrainHealthPerDamage:
            case E_PropertyEffectType.RogueHealingMaserty_CheatDeath:
                return E_BattlePropertyType.Healing;
        }

        return E_BattlePropertyType.None;
    }
}
