using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrusadersQuestReplica
{
    public class DamageInfo : Info
    {
        public Unit caster;
        public E_EffectType damageType;
        public float penetration;
        public float damage;
    }
}