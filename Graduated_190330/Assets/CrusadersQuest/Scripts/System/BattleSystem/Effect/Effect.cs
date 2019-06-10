using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    [System.Serializable]
    public class Effect : MonoBehaviour
    {
        public UnitValue caster;
        public UnitValue target;

        public virtual void Adjust()
        {

        }
        public virtual void Adjust(UnitValue caster)
        {

        }
        public virtual void Adjust(UnitValue caster, UnitValue target)
        {

        }
        public virtual void Adjust(UnitValue caster, List<UnitValue> targetList)
        {

        }
    }
}
