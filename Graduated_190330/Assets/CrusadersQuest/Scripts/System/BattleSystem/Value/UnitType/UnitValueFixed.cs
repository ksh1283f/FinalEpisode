using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrusadersQuestReplica
{
    [CreateAssetMenu(fileName = "NewUnitValueFixed", menuName = "Value/UnitValue/Fixed", order = 3)]
    public class UnitValueFixed : UnitValue
    {
        public Unit target;
        public override Unit GetValue()
        {
            return target;
        }
    }
}