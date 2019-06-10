using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DamageEffect : Effect
{
    public E_EffectType damageType=E_EffectType.Physics;
    
    public List<AmountSet> damageList;
    public float totalDamage;
    public float fixedDamage;
    public float casterBasedDamage;
    public float targetBasedDamage;

    
    public List<AmountSet> additionalPenetrationList;
    public float totalPenetration;
    public float fixedPenetration;
    public float casterBasedPenetration;
    public float targetBasedPenetration;





    public override void RefreshAllAmount(Unit caster, Unit target)
    {
        totalDamage = 0;
        fixedDamage = 0;
        casterBasedDamage = 0;
        targetBasedDamage = 0;
        for (int index = 0; index < damageList.Count; index++)
        {
            fixedDamage += damageList[index].FixedAmount;
            casterBasedDamage += damageList[index].SetAmountCasterBased(caster);
            targetBasedDamage += damageList[index].SetAmountTargetBased(target);
        }
        totalDamage = fixedDamage + casterBasedDamage + targetBasedDamage;

        totalPenetration = 0;
        fixedPenetration = 0;
        casterBasedPenetration = 0;
        targetBasedPenetration = 0;
        for (int index = 0; index < additionalPenetrationList.Count; index++)
        {
            fixedDamage += additionalPenetrationList[index].FixedAmount;
            casterBasedDamage += additionalPenetrationList[index].SetAmountCasterBased(caster);
            targetBasedDamage += additionalPenetrationList[index].SetAmountTargetBased(target);
        }
        totalPenetration = fixedPenetration + casterBasedPenetration + targetBasedPenetration;
    }
    public override void RefreshFixedAllAmount()
    {
        fixedDamage = 0;
        for (int index = 0; index < damageList.Count; index++)
        {
            fixedDamage += damageList[index].FixedAmount;
        }
        totalDamage = fixedDamage + targetBasedDamage + casterBasedDamage;

        fixedPenetration = 0;
        for (int index = 0; index < additionalPenetrationList.Count; index++)
        {
            fixedPenetration += additionalPenetrationList[index].FixedAmount;
        }
        totalPenetration = fixedPenetration + targetBasedPenetration + casterBasedPenetration;
    }
    public override void RefreshTargetBasedAmount(Unit target)
    {
        totalDamage = 0;
        fixedDamage = 0;
        targetBasedDamage = 0;
        for (int index = 0; index < damageList.Count; index++)
        {
            fixedDamage += damageList[index].FixedAmount;
            targetBasedDamage += damageList[index].SetAmountTargetBased(target);
        }
        totalDamage = fixedDamage + casterBasedDamage + targetBasedDamage;

        totalPenetration = 0;
        fixedPenetration = 0;
        casterBasedPenetration = 0;
        targetBasedPenetration = 0;
        for (int index = 0; index < additionalPenetrationList.Count; index++)
        {
            fixedPenetration += additionalPenetrationList[index].FixedAmount;
            targetBasedPenetration += additionalPenetrationList[index].SetAmountTargetBased(target);
        }
        totalPenetration = fixedPenetration + casterBasedPenetration + targetBasedPenetration;
    }
    public override void RefreshCasterBasedAmount(Unit caster)
    {
        totalDamage = 0;
        fixedDamage = 0;
        casterBasedDamage = 0;
        for (int index = 0; index < damageList.Count; index++)
        {
            fixedDamage += damageList[index].FixedAmount;
            casterBasedDamage += damageList[index].SetAmountCasterBased(caster);
        }

        totalDamage = fixedDamage + casterBasedDamage + targetBasedDamage;

        totalPenetration = 0;
        fixedPenetration = 0;
        casterBasedPenetration = 0;
        for (int index = 0; index < additionalPenetrationList.Count; index++)
        {
            fixedPenetration += additionalPenetrationList[index].FixedAmount;
            casterBasedPenetration += additionalPenetrationList[index].SetAmountCasterBased(caster);
        }
        totalPenetration = fixedPenetration + casterBasedPenetration + targetBasedPenetration;
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        bool isCritical = false;
        if (caster.StatManager.CreateOrGetStat(E_StatType.CriticalRate).ModifiedValue > Random.value)
        {
            isCritical = true;
            totalDamage *= caster.StatManager.CreateOrGetStat(E_StatType.CriticalMultiplier).ModifiedValue;
        }

        if (true)
        {
            target.GetDamage(ref totalDamage, damageType, totalPenetration* multiplier, isCritical);
        }
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        bool isCritical = false;
        if (caster.StatManager.CreateOrGetStat(E_StatType.CriticalRate).ModifiedValue > Random.value)
        {
            isCritical = true;
            totalDamage *= caster.StatManager.CreateOrGetStat(E_StatType.CriticalMultiplier).ModifiedValue;
        }

        if (true)
        {
            target.GetDamage(ref totalDamage,damageType,totalPenetration+caster.StatManager.CreateOrGetStat(((E_StatType)damageType)+3).ModifiedValue, isCritical);
        }
    }

    /* protected override bool ConditionCheck()
     {
         for (int index = 0; index < validatorList.Count; index++)
         {
             if (!validatorList[index].Check(caster, target))
             {
                 return false;
             }
         }
         return true;
     }*/
}
