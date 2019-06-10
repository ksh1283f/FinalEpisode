using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    [CreateAssetMenu(fileName = "NewFloatValueFixed", menuName = "Value/FloatValue/Fixed", order = 2)]
    public class FloatValueFixed : FloatValue
    {
        public float fixedValue;
        public override float GetValue()
        {
            return fixedValue;
        }
    }
}