using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrusadersQuestReplica
{
    public class Stat
    {
        protected string statName;
        protected E_StatType statType;


        public E_StatType StatType
        {
            get
            {
                return statType;
            }
        }
        public string StatName
        {
            get
            {
                return statName;
            }
        }
    }
}
