namespace Graduate.Unit
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum E_UnitState
    {
        None,
        Idle,
        Move,
        Attack,
        Death,
    }

    public class Unit : MonoBehaviour
    {
        // 적, 아군 모두의 부모클래스
        // 유닛이라면 가질 공통된 요소들
        public int HP;
        public int Atk;
        public int Def;

        string tag;
        [SerializeField]protected E_UnitState unitState = E_UnitState.None;
        

        protected virtual void Start()
        {
            tag = gameObject.tag;
        }
    }
}