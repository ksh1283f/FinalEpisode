using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEffect : Effect
{
    public override void ActivateEffect()
    {
        Debug.Log(gameObject.name+" Effected!");
    }
    public override void ActivateEffect(Unit caster)
    {
        Debug.Log(gameObject.name + " Effected! : " + caster + " >> " + target);
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        Debug.Log(gameObject.name + " Effected! : "+caster+" >> "+target);
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        Debug.Log(gameObject.name + " Effected! : " + caster + " >> " + target);
    }
    public override void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
        Debug.Log(gameObject.name + " Effected! : " + caster + " >> " + target);
    }
}
