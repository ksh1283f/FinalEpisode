namespace Graduate.Unit.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Graduate.Unit;
    using Graduate.GameData.UnitData;

    public enum E_DistanceWithTarget
    {
        Far,
        Near,
    }

    public class PlayerUnit : Graduate.Unit.Unit
    {
        Animator ani;
        float speed;
        [SerializeField] E_CharacterType characterType;
        public E_CharacterType CharacterType {get{return characterType;}}
        public Unit Target;
        public E_UnitState PlayerUnitState
        {
            get { return unitState; }
            set
            {
                if (value == unitState)
                    return;
                E_UnitState previousState = value;
                unitState = value;
                switch (previousState)
                {
                    case E_UnitState.Attack:
                        ani.SetBool("IsAttack", false);
                        break;
                    case E_UnitState.Move:
                        ani.SetBool("IsMove", false);
                        break;
                }

                switch (unitState)  // 실행될 이벤트
                {
                    case E_UnitState.None:
                        break;

                    case E_UnitState.Idle:
                        break;

                    case E_UnitState.Move:
                        if (gameObject.CompareTag("Player"))
                        {
                            ani.SetBool("IsMove", true);
                            StopAllCoroutines();
                            StartCoroutine(CalculateEnemyDistance());
                            // move animation
                            
                        }
                        break;

                    case E_UnitState.Attack:
                        ani.SetBool("IsMove", false);
                        StartAniAttack();
                        break;

                    case E_UnitState.Death:
                        // 죽음 애니메이션
                        ani.SetBool("IsDeath", true);
                        break;
                }
            }
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            ani = GetComponent<Animator>();
            speed = 2f;
        }

        // Update is called once per frame
        void Update()
        {
            if (tag.Equals("Enemy"))
                return;

            switch (PlayerUnitState)
            {
                case E_UnitState.None:
                    break;

                case E_UnitState.Idle:
                    break;

                case E_UnitState.Move:
                    transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, speed * Time.deltaTime);
                    break;

                case E_UnitState.Attack:
                    break;

                case E_UnitState.Death:
                    break;
            }
        }

        IEnumerator CalculateEnemyDistance()
        {
            E_DistanceWithTarget distance = E_DistanceWithTarget.Far;
            Target = BattleManager.Instance.Target;
            while (Target == null)
                yield return null;

            while (distance != E_DistanceWithTarget.Near)
            {
                float dis = Vector3.Distance(gameObject.transform.position, Target.gameObject.transform.position);
                if (dis <= 3f)
                {
                    distance = E_DistanceWithTarget.Near;
                    BattleManager.Instance.IsEncounterEnemy = true;
                }
                yield return null;

            }
        }

        public void StartAniAttack()
        {
            ani.Play("Attack");
        }

        public void SetCharacterType(E_CharacterType type)
        {
            characterType = type;
        }

        private void OnTriggerEnter(Collider other)
        {

        }
    }
}