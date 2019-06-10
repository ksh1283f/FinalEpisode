using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill:MonoBehaviour
{
    public string motionName;
    public List<Effect> effectList = new List<Effect>();


    public void ActivateEffect(Unit caster)
    {
        foreach(Effect effect in effectList)
        {
            effect.RefreshCasterBasedAmount(caster);
            effect.ActivateEffect(caster);
        }
    }
}
