using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    [CreateAssetMenu(fileName = "NewFloatValueRefUnit", menuName = "Value/FloatValue/RefUnit", order = 1)]
    public class FloatValueRefUnit : FloatValue
    {
        public UnitValue target;
        public E_StatType statType;
        public bool isBaseValue=true;
        public float multiplier = 0;

        public override float GetValue()
        {
            if(isBaseValue)
            {
                return target.GetValue().statComp.GetStat(statType).BaseValue* multiplier;
            }
            else
            {
                return target.GetValue().statComp.GetStat(statType).ModifiedValue* multiplier;
            }
        }
    }
}