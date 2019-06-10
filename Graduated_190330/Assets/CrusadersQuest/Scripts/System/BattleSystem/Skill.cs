using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    public class Skill : MonoBehaviour
    {
        public string skillName = string.Empty;
        public List<Effect> effectList = new List<Effect>();

        public void ActivateEffect(UnitValue caster)
        {
            foreach(Effect effect in effectList)
            {
                effect.Adjust(caster);
            }
        }
    
    }

}
