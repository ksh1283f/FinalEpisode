using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AddBuffEffect : Effect
{
    public List<Buff> buffList = new List<Buff>();

    public override void ActivateEffect(Unit caster)
    {
        base.ActivateEffect(caster);
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        foreach (Buff buff in buffList )
        {
            target.BuffManager.Add(buff, caster, target);
        }
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        foreach (Buff buff in buffList)
        {
            target.BuffManager.Add(buff, caster, target);
        }
    }
    public override void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
        foreach (Buff buff in buffList)
        {
            target.BuffManager.Add(buff, caster, target);
        }
    }
    public override bool ConditionCheck()
    {
        return base.ConditionCheck();
    }
}
