using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEffect : Effect
{
    public List<Effect> effectSet = new List<Effect>();


    public override void RefreshAllAmount(Unit caster, Unit target)
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshAllAmount(caster, target);
        }
    }
    public override void RefreshFixedAllAmount()
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshFixedAllAmount();
        }
    }
    public override void RefreshCasterBasedAmount(Unit caster)
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshCasterBasedAmount(caster);
        }
    }
    public override void RefreshTargetBasedAmount(Unit target)
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshTargetBasedAmount(target);
        }
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshTargetBasedAmount(target);
            effect.ActivateEffect(caster, target);
        }
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        foreach (Effect effect in effectSet)
        {
            effect.RefreshTargetBasedAmount(target);
            effect.ActivateEffect(caster, target);
        }
    }

}
