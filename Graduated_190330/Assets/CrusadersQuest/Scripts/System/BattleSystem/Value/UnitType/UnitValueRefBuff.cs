using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrusadersQuestReplica
{
    [CreateAssetMenu(fileName = "NewUnitValueRefBufft", menuName = "Value/UnitValue/RefBuff", order = 2)]
    public class UnitValueRefBuff : UnitValue
    {
        public Buff buff;
        public E_UnitType returnValue;

        public override Unit GetValue()
        {
            switch(returnValue)
            {
                case E_UnitType.Caster:
                    Debug.Log("Yet");
                    return null;
                case E_UnitType.Target:
                    Debug.Log("Yet");
                    return null;
                default:
                    return null;

            }
        }
    }
}