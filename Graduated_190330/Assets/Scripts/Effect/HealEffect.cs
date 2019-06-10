using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealEffect : Effect
{
    public E_EffectType damageType = E_EffectType.Physics;

    public List<AmountSet> healAmountList;
    public float totalHealAmount = 0;
    public float fixedHealAmount = 0;
    public float casterBasedHealAmount = 0;
    public float targetBasedHealAmount = 0;

    public override void RefreshAllAmount(Unit caster, Unit target)
    {
        totalHealAmount = 0;
        fixedHealAmount = 0;
        casterBasedHealAmount = 0;
        targetBasedHealAmount = 0;
        for (int index = 0; index < healAmountList.Count; index++)
        {
            fixedHealAmount += healAmountList[index].FixedAmount;
            casterBasedHealAmount += healAmountList[index].SetAmountCasterBased(caster);
            targetBasedHealAmount += healAmountList[index].SetAmountTargetBased(target);
        }
        totalHealAmount = fixedHealAmount + casterBasedHealAmount + targetBasedHealAmount;
    }
    public override void RefreshFixedAllAmount()
    {
        fixedHealAmount = 0;
        for (int index = 0; index < healAmountList.Count; index++)
        {
            fixedHealAmount += healAmountList[index].FixedAmount;
        }
        totalHealAmount = fixedHealAmount + targetBasedHealAmount + casterBasedHealAmount;
    }
    public override void RefreshTargetBasedAmount(Unit target)
    {
        totalHealAmount = 0;
        fixedHealAmount = 0;
        targetBasedHealAmount = 0;
        for (int index = 0; index < healAmountList.Count; index++)
        {
            fixedHealAmount += healAmountList[index].FixedAmount;
            targetBasedHealAmount += healAmountList[index].SetAmountTargetBased(target);
        }
        totalHealAmount = fixedHealAmount + casterBasedHealAmount + targetBasedHealAmount;

    }
    public override void RefreshCasterBasedAmount(Unit caster)
    {
        totalHealAmount = 0;
        fixedHealAmount = 0;
        casterBasedHealAmount = 0;
        for (int index = 0; index < healAmountList.Count; index++)
        {
            fixedHealAmount += healAmountList[index].FixedAmount;
            casterBasedHealAmount += healAmountList[index].SetAmountCasterBased(caster);
        }
        totalHealAmount = fixedHealAmount + casterBasedHealAmount + targetBasedHealAmount;
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        if (true)
        {
            target.GetHeal(ref totalHealAmount);
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
