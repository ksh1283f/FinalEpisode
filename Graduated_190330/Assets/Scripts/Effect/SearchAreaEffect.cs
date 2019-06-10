using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SearchAreaEffect : Effect
{
    public override Unit Caster
    {
        get
        {
            return caster;
        }

        set
        {
            caster = value;
            CasterSet(enterEffectList);
        }
    }

    public override Unit Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
            TargetSet(enterEffectList);
        }
    }


    //원일때 탐색 각도
    //각도
    //최대 포착
    //최소 포착

    //서칭 옵션
    //뒤에서부터, 앞에서부터,랜덤
    //

    public int maximumCount = 0;
    public List<Effect> enterEffectList = new List<Effect>();
    int currentCount = 0;


    public override void RefreshAllAmount(Unit caster, Unit target)
    {
        foreach (Effect effect in enterEffectList)
        {
            effect.RefreshAllAmount(caster, target);
        }
    }
    public override void RefreshFixedAllAmount()
    {
        foreach (Effect effect in enterEffectList)
        {
            effect.RefreshFixedAllAmount();
        }
    }
    public override void RefreshCasterBasedAmount(Unit caster)
    {
        foreach (Effect effect in enterEffectList)
        {
            effect.RefreshCasterBasedAmount(caster);
        }
    }
    public override void RefreshTargetBasedAmount(Unit target)
    {
        foreach (Effect effect in enterEffectList)
        {
            effect.RefreshTargetBasedAmount(target);
        }
    }









    public override void ActivateEffect()
    {
        gameObject.SetActive(true);
    }
    public override void ActivateEffect(Unit caster)
    {
        this.caster = caster;
        gameObject.SetActive(true);
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        this.caster = caster;
        gameObject.SetActive(true);
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        this.caster = caster;
        gameObject.SetActive(true);
    }
    public override void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
        this.caster = caster;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D targetCollider)
    {
        Unit target = targetCollider.GetComponent<Unit>();
        if (target)
        {
            if (currentCount < maximumCount)
            {
                currentCount++;
                foreach (Effect effect in enterEffectList)
                {
                    effect.RefreshTargetBasedAmount(target);
                    effect.ActivateEffect(caster, target);
                }
            }
        }
        

  
        

    }
    private void OnEnable()
    {
        currentCount = 0;
    }
}
