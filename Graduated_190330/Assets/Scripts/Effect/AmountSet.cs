using UnityEngine;
using System.Collections;
[System.Serializable]
public class AmountSet
{
    public float fixedAmount;
    public E_StatType casterBasedStatType;
    public float casterBasedAmount;
    public float FixedAmount
    {
        get
        {
            return fixedAmount;
        }
        set
        {
            fixedAmount = value;
        }
    }
    
    public float CasterBasedAmount
    {
        get
        {
            return casterBasedAmount;
        }
        set
        {
            casterBasedAmount = value;
        }
    }

    public float targetBasedAmount;
    public float TargetBasedAmount
    {
        get
        {
            return targetBasedAmount;
        }
        set
        {
            targetBasedAmount = value;
        }
    }
    public E_StatType targetBasedStatType;

    /// <summary>
    /// 시전자 기반 값 갱신
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public float SetAmountCasterBased(Unit caster)
    {
        return casterBasedAmount * caster.StatManager.CreateOrGetStat(casterBasedStatType).ModifiedValue;
    }
    /// <summary>
    /// 대상 기반값 갱신
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public float SetAmountTargetBased(Unit target)
    {
        return targetBasedAmount * target.StatManager.CreateOrGetStat(targetBasedStatType).ModifiedValue;
    }

}
