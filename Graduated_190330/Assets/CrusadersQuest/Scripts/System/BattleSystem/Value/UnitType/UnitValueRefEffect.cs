using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrusadersQuestReplica
{
    [CreateAssetMenu(fileName = "NewUnitValueRefEffect", menuName = "Value/UnitValue/RefEffect", order = 1)]
    public class UnitValueRefEffect : UnitValue
    {
        public Effect effect;
        public E_UnitType retrunValue;
        public override Unit GetValue()
        {
            switch(retrunValue)
            {
                case E_UnitType.Caster:
                    return effect.caster.GetValue();
                case E_UnitType.Target:
                    return effect.target.GetValue();
                default:
                    return null;
            }
        }
    }
}