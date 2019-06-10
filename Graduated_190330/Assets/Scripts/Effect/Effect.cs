using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public partial class Effect : MonoBehaviour
{
    protected Unit caster;
    protected Unit target;

    //protected List<Validator> validatorList = new List<Validator>();
}
public partial class Effect : MonoBehaviour
{
    public Effect()
    {
        caster = null;
        target = null;
    }
    public Effect(Unit caster,Unit target)
    {
        this.caster = caster;
        this.target = target;
    }
    public Effect(Unit caster)
    {
        this.caster = caster;
    }

    public virtual Unit Caster
    {
        get
        {
            return caster;
        }
        set
        {
            caster = value;
        }
    }
    public virtual Unit Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    public virtual void ActivateEffect()
    {
    }
    public virtual void ActivateEffect(Unit caster)
    {
    }
    public virtual void ActivateEffect(Unit caster, Unit target)
    {
    }
    public virtual void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
    }
    public virtual void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
    }

    public virtual void RefreshAllAmount(Unit caster, Unit target)
    {
    }
    public virtual void RefreshTargetBasedAmount(Unit target)
    {
    }
    public virtual void RefreshCasterBasedAmount(Unit caster)
    {
    }
    public virtual void RefreshFixedAllAmount()
    {

    }


    public virtual bool ConditionCheck()
    {/*
        for(int index=0; index < validatorList.Count; index++)
        {
            if(!validatorList[index].Check(caster, target))
            {
                return false;
            }
        }
        return true;*/
        return true;
    }

    protected void CasterSet(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].Caster = caster;
        }
    }
    protected void TargetSet(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].Target = target;
        }
    }

}
