using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterateTransportEffect : Effect
{
    public float period = 0;
    public int periodCount = 0;




    public List<Effect> initialEffectList = new List<Effect>();
    public List<Effect> periodicEffectList = new List<Effect>();

    private void Start()
    {
        foreach (Effect effect in initialEffectList)
        {
            effect.ActivateEffect(caster, target);
        }
        StartCoroutine(PeriodEffect());
    }


    public override void ActivateEffect()
    {
        StartCoroutine(PeriodEffect());
    }

    IEnumerator PeriodEffect()
    {
        int currentCount = 0;
        float currentPeriod = 0;
        //6초마다 총 5번 시작1번~5번
        while(true)
        {
            if(currentCount<periodCount)
            {
                if (currentPeriod<period)
                {
                    currentPeriod += Time.deltaTime;
                }
                else
                {
                    currentPeriod = 0;
                    currentCount++;
                    foreach (Effect effect in periodicEffectList)
                    {
                        effect.ActivateEffect(caster, target);
                        
                    }
                }
            }

            yield return null;
        }
    }

    public override void ActivateEffect(Unit caster, Unit target)
    {
        StartCoroutine(PeriodEffect());
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        StartCoroutine(PeriodEffect());
    }
    public override void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
        StartCoroutine(PeriodEffect());
    }
}
