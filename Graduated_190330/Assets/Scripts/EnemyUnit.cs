namespace Graduate.Unit.Enemy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Graduate.Unit;
    public enum E_EnemySequence
    {
        First,
        Second,
        Third,
    }

    public class EnemyUnit : Graduate.Unit.Unit
    {
        public Action<int> OnDamagedEnemy { get; set; }
        public Action<bool> OnDeathEnemy { get; set; }
        public E_EnemySequence Sequence;
        public bool IsBoss;
        public E_UnitState EnemyUnitState
        {
            get { return unitState; }
            set
            {
                if (value == unitState)
                    return;

                unitState = value;
                switch (unitState)
                {
                    case E_UnitState.Idle:	// 애니메이션 재생
                        break;

                    case E_UnitState.Attack:
                        break;

                    case E_UnitState.Death:
                        OnDeathEnemy.Execute(true);
                        break;
                }

            }
        }
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            OnDeathEnemy+=OnDeath;
        }

        public void GetDamaged(int damage)
        {

            HP -= damage;
            OnDamagedEnemy.Execute(damage);
            if (HP <= 0)
            {
                HP=0;
                EnemyUnitState = E_UnitState.Death;
            }
        }

        void OnDeath(bool isDeath)
        {
            gameObject.SetActive(!isDeath);
        }
    }
}