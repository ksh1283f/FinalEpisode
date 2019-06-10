using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    public class Castable : MonoBehaviour
    {
        Unit caster;
        
        public List<Skill> skillList = new List<Skill>();
        void Awake()
        {
            caster = GetComponent<Unit>();
        }
        public void CastSkill(int index)
        {
            UnitValueFixed caster = ScriptableObject.CreateInstance<UnitValueFixed>();
            caster.target = this.caster;
            skillList[index].ActivateEffect(caster);
        }
    }

}
