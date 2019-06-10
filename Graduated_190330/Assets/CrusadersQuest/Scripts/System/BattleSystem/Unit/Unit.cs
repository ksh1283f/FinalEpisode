using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrusadersQuestReplica
{
    [RequireComponent(typeof(Statisticalbe))]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Healable))]
    [RequireComponent(typeof(Buffable))]
    [RequireComponent(typeof(Castable))]
    public class Unit : MonoBehaviour
    {
        public bool testMode=false;
        public Statisticalbe statComp;
        public Buffable buffComp;
        public Damageable damageComp;
        public Healable healComp;
        public Castable CastComp;

        string u_Id;
        string unitName;



        private void Awake()
        {
            statComp = GetComponent<Statisticalbe>();
            buffComp = GetComponent<Buffable>();
            damageComp = GetComponent<Damageable>();
            healComp = GetComponent<Healable>();
            CastComp = GetComponent<Castable>();
        }
        private void Start()
        {
            if(testMode)
                SetTestHero();
        }
        void SetTestHero()
        {
            foreach (E_StatType _statType in System.Enum.GetValues(typeof(E_StatType)))
            {
                statComp.CreateOrGetStat(_statType);
            }


            statComp.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue = 9798.5f;
            statComp.CreateOrGetStat(E_StatType.MinHealth).ModifiedValue = 0;
            statComp.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue = 9798.5f;

            statComp.CreateOrGetStat(E_StatType.AttackPoint).ModifiedValue = 860.9f;

            statComp.CreateOrGetStat(E_StatType.CriticalRate).ModifiedValue = 21.7f / 100;
            statComp.CreateOrGetStat(E_StatType.CriticalMultiplier).ModifiedValue = 2.05f;

            statComp.CreateOrGetStat(E_StatType.MaxSpecialPoint).ModifiedValue = 100;
            statComp.CreateOrGetStat(E_StatType.MinSpecialPoint).ModifiedValue = 0;
            statComp.CreateOrGetStat(E_StatType.CurrentSpecialPoint).ModifiedValue = 0;

            statComp.CreateOrGetStat(E_StatType.PhysicalDefense).ModifiedValue = 564.6f;
            statComp.CreateOrGetStat(E_StatType.MagicalDefense).ModifiedValue = 464.6f;

            statComp.CreateOrGetStat(E_StatType.DamageReduceRate).ModifiedValue = 0;

            statComp.CreateOrGetStat(E_StatType.BloodSuckingRate).ModifiedValue = 0;


            statComp.CreateOrGetStat(E_StatType.MaxAccuracy).ModifiedValue = 0.75f;
            statComp.CreateOrGetStat(E_StatType.MinAccuracy).ModifiedValue = 0f;
            statComp.CreateOrGetStat(E_StatType.CurrentAccuracy).ModifiedValue = 0.15f;

            statComp.CreateOrGetStat(E_StatType.MaxEvasionRate).ModifiedValue = 0.75f;
            statComp.CreateOrGetStat(E_StatType.MinEvasionRate).ModifiedValue = 0f;
            statComp.CreateOrGetStat(E_StatType.CurrentEvasionRate).ModifiedValue = 0.15f;

            statComp.CreateOrGetStat(E_StatType.MaxRange).ModifiedValue = 3;
            statComp.CreateOrGetStat(E_StatType.MinRange).ModifiedValue = 2.5f;
            statComp.CreateOrGetStat(E_StatType.CurrentRange).ModifiedValue = 0;


            statComp.CreateOrGetStat(E_StatType.MaxAttackSpeed).ModifiedValue = 2.5f;
            statComp.CreateOrGetStat(E_StatType.MinAttackSpeed).ModifiedValue = 0.25f;
            statComp.CreateOrGetStat(E_StatType.CurrentAttackSpeed).ModifiedValue = 1.4f;

            statComp.CreateOrGetStat(E_StatType.PhysicalPenetration).ModifiedValue = 0;
            statComp.CreateOrGetStat(E_StatType.MagicalPenetration).ModifiedValue = 0;
            //이동속도
            statComp.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 75;

            statComp.CreateOrGetStat(E_StatType.KnockbackResistance).ModifiedValue = 0;

            statComp.CreateOrGetStat(E_StatType.TimeAccelerationRate).ModifiedValue = 1;
            statComp.CreateOrGetStat(E_StatType.MotionAccelerationRate).ModifiedValue = 1;
            statComp.CreateOrGetStat(E_StatType.ScaleMultiplier).ModifiedValue = 1;
            GetComponent<Animator>().SetBool("inBattle", true);
        }
    }
}

