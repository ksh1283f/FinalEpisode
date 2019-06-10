using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_StageStep
{
    First,
    Secont,
    Third,
}
// 페이즈 만들기
// 콜라이더 3개 만들어서 체크
public partial class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private E_StageStep stageStep = E_StageStep.First;
    public E_StageStep StageStep
    {
        get { return stageStep; }
        set
        {
            if (value == stageStep)
                return;

            stageStep = value;
            switch (stageStep)
            {
                case E_StageStep.First:
                    OnFirstStep.Execute();
                    break;

                case E_StageStep.Secont:
                    OnSecondStep.Execute();
                    break;

                case E_StageStep.Third:
                    OnThirdStep.Execute();
                    break;
            }
        }
    }

    void OnFirst()
    {

    }

    public Action OnFirstStep { get; set; }
    public Action OnSecondStep { get; set; }
    public Action OnThirdStep { get; set; }

    public Action<Unit> OnDeadEnemy { get; set; }
    public Action<Vector3> OnLineUpHpBar { get; set; }

    public List<Unit> playerUnitList = new List<Unit>();
    public List<Unit> alivePlayerUnitList = new List<Unit>();
    public List<Unit> deadPlayerUnitList = new List<Unit>();
    public List<Unit> enemyUnitList = new List<Unit>();
    public List<Unit> aliveEnemyUnitList = new List<Unit>();
}
public partial class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (instance)
            {
                return instance;
            }
            else
            {
                instance = FindObjectOfType<GameManager>();
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "GameManager";
                    instance = container.AddComponent<GameManager>();
                }
                return instance;
            }
        }
    }
    public List<Unit> PlayerUnitList
    {
        get
        {
            return playerUnitList;
        }
    }


    Unit leaderHero;

    //플레이어 유닛 리스트
    //웨이브 정보
    //웨이브 세트 리스트<몬스터 세트>
    //리스트<몬스터> 각각의 몬스터 프리펩 


    //각종 용사 리스트에 대한 쿼리들
    //------------------------------
    //용사 쿼리종류
    public Unit GetAliveHeadHero
    {
        get
        {
            Unit hero = alivePlayerUnitList[0];
            for (int index = 1; index < alivePlayerUnitList.Count; index++)
            {
                if (hero.transform.position.x < alivePlayerUnitList[index].transform.position.x)
                {
                    hero = alivePlayerUnitList[index];
                }
            }

            return hero;
        }
    }
    public Unit GetAliveTailHero
    {
        get
        {
            Unit hero = alivePlayerUnitList[0];
            for (int index = 1; index < alivePlayerUnitList.Count; index++)
            {
                if (hero.transform.position.x > alivePlayerUnitList[index].transform.position.x)
                {
                    hero = alivePlayerUnitList[index];
                }
            }

            return hero;
        }

    }
    public Unit GetDeadHeadHero
    {
        get
        {
            Unit hero = deadPlayerUnitList[0];
            for (int index = 1; index < deadPlayerUnitList.Count; index++)
            {
                if (hero.transform.position.x < deadPlayerUnitList[index].transform.position.x)
                {
                    hero = deadPlayerUnitList[index];
                }
            }

            return hero;
        }
    }
    public Unit GetDeadTailHero
    {
        get
        {
            Unit hero = deadPlayerUnitList[0];
            for (int index = 1; index < deadPlayerUnitList.Count; index++)
            {
                if (hero.transform.position.x > deadPlayerUnitList[index].transform.position.x)
                {
                    hero = deadPlayerUnitList[index];
                }
            }

            return hero;
        }

    }
    public Unit GetCurrentLeaderHero
    {
        get
        {
            return leaderHero;
        }
    }
    public Unit GetRandomAliveHero
    {
        get
        {
            List<Unit> tempHeroList = new List<Unit>();
            for (int index = 0; index < playerUnitList.Count; index++)
            {
                if (playerUnitList[index].IsAlive)
                {
                    tempHeroList.Add(playerUnitList[index]);
                }

            }


            Unit hero = playerUnitList[UnityEngine.Random.Range(0, 3)];
            return hero;
        }
    }
    void ResetLeaderHero()
    {
        leaderHero = GetAliveHeadHero;
    }

    void HeroDead(Unit deadUnit)
    {
        alivePlayerUnitList.Remove(deadUnit);
        deadPlayerUnitList.Add(deadUnit);

        if (alivePlayerUnitList.Count == 3)
        {
            //게임오버
        }
    }
    void HeroRebirth(Unit aliveUnit)
    {
        alivePlayerUnitList.Add(aliveUnit);
        deadPlayerUnitList.Remove(aliveUnit);
    }


    //Get 리더 용사
    //Get 살아있는 모든 용사들
    //Get 죽어있는 모든 용사들
    //Get 죽어있는 용사중 선두 용사
    //Get 죽어있는 용사중 후미 용사
    //Get Random 용사

    //몬스터 쿼리 종류
    //Get 맨앞 몬스터
    //Get 제일 후열 몬스터


    //웨이브 관리
    //----------
    //웨이브 시작(N)
    //웨이브 전체 루프관리 
    //


    //플레이어 승리 모든 웨이브 종료
    //플레이어 패배 아군 생존 0 일때

    //웨이브 정보

    void EnterTheBattle()
    {
        //foreach (Unit _hero in playerUnitList)
        //{
        //    SetTestHero(_hero, E_GroupTag.Player);
        //}
        //foreach (Unit _hero in enemyUnitList)
        //{
        //    SetTestHero(_hero, E_GroupTag.Enemy);
        //}

        for (int i = 0; i < playerUnitList.Count; i++)
            SetTestHero(playerUnitList[i], E_GroupTag.Player, (E_PlayerType)i);

        for (int i = 0; i < enemyUnitList.Count; i++)
            SetTestHero(enemyUnitList[i], E_GroupTag.Enemy);
    }

    void SetTestHero(Unit _hero, E_GroupTag group, E_PlayerType playerType = E_PlayerType.None)
    {
        if (_hero == null)
            return;

        foreach (E_StatType _statType in System.Enum.GetValues(typeof(E_StatType)))
        {
            _hero.StatManager.CreateOrGetStat(_statType);
        }

        _hero.groupTag = group;
        _hero.PlayerType = playerType;

        #region 기존 코드
        _hero.StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue = 9798.5f;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinHealth).ModifiedValue = 0;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue = 9798.5f;
        #endregion

        //_hero.StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue = 5000.5f;
        //_hero.StatManager.CreateOrGetStat(E_StatType.MinHealth).ModifiedValue = 0;
        //_hero.StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue = 5000.5f;

        // todo 캐릭터를 선택할수 있게 할때는 따로 바꿔서 처리해야한다(다른곳으로 빼던지)
        if(_hero.ClassType.IsHealable())
            _hero.StatManager.CreateOrGetStat(E_StatType.AttackPoint).ModifiedValue = -600f;
        else
            _hero.StatManager.CreateOrGetStat(E_StatType.AttackPoint).ModifiedValue = 860.9f;

        // if (_hero.PlayerType == E_PlayerType.Sub2)  // 힐(양수이면 공격, 음수이면 힐)
        //     _hero.StatManager.CreateOrGetStat(E_StatType.AttackPoint).ModifiedValue = -860.9f;
        // else
        //     _hero.StatManager.CreateOrGetStat(E_StatType.AttackPoint).ModifiedValue = 860.9f;

        _hero.StatManager.CreateOrGetStat(E_StatType.CriticalRate).ModifiedValue = 21.7f / 100;
        _hero.StatManager.CreateOrGetStat(E_StatType.CriticalMultiplier).ModifiedValue = 2.05f;

        _hero.StatManager.CreateOrGetStat(E_StatType.MaxSpecialPoint).ModifiedValue = 100;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinSpecialPoint).ModifiedValue = 0;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentSpecialPoint).ModifiedValue = 0;

        _hero.StatManager.CreateOrGetStat(E_StatType.PhysicalDefense).ModifiedValue = 564.6f;
        _hero.StatManager.CreateOrGetStat(E_StatType.MagicalDefense).ModifiedValue = 464.6f;

        _hero.StatManager.CreateOrGetStat(E_StatType.DamageReduceRate).ModifiedValue = 0;

        _hero.StatManager.CreateOrGetStat(E_StatType.BloodSuckingRate).ModifiedValue = 0;


        _hero.StatManager.CreateOrGetStat(E_StatType.MaxAccuracy).ModifiedValue = 0.75f;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinAccuracy).ModifiedValue = 0f;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentAccuracy).ModifiedValue = 0.15f;

        _hero.StatManager.CreateOrGetStat(E_StatType.MaxEvasionRate).ModifiedValue = 0.75f;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinEvasionRate).ModifiedValue = 0f;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentEvasionRate).ModifiedValue = 0.15f;

        _hero.StatManager.CreateOrGetStat(E_StatType.MaxRange).ModifiedValue = 3;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinRange).ModifiedValue = 2.5f;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentRange).ModifiedValue = 0;


        _hero.StatManager.CreateOrGetStat(E_StatType.MaxAttackSpeed).ModifiedValue = 2.5f;
        _hero.StatManager.CreateOrGetStat(E_StatType.MinAttackSpeed).ModifiedValue = 0.25f;
        _hero.StatManager.CreateOrGetStat(E_StatType.CurrentAttackSpeed).ModifiedValue = 1.4f;

        _hero.StatManager.CreateOrGetStat(E_StatType.PhysicalPenetration).ModifiedValue = 0;
        _hero.StatManager.CreateOrGetStat(E_StatType.MagicalPenetration).ModifiedValue = 0;
        //이동속도
        _hero.StatManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 70;

        _hero.StatManager.CreateOrGetStat(E_StatType.KnockbackResistance).ModifiedValue = 0;

        _hero.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).ModifiedValue = 1;
        _hero.StatManager.CreateOrGetStat(E_StatType.MotionAccelerationRate).ModifiedValue = 1;
        _hero.StatManager.CreateOrGetStat(E_StatType.ScaleMultiplier).ModifiedValue = 1;
        _hero.GetComponent<Animator>().SetBool("inBattle", true);
    }

    public List<string> playerHeroIDList = new List<string>(3);

    #region Phase 관련 
    List<PhaseChecker> checkerList = new List<PhaseChecker>();
    public List<PhaseChecker> CheckerList {get{return checkerList;}}
    E_PhaseType lastPhase = E_PhaseType.None;
    public E_PhaseType LastPhase { get{return lastPhase;}}

    E_PhaseType presentPhase = E_PhaseType.None;
    public E_PhaseType PresentPhase { get{return presentPhase;}}
     
    // UI 갱신용 델리게이트
    public Action<E_PhaseType> OnChangedGamePhase {get;set;}

    public Action<bool> OnExecuteResult {get;set;}
    #endregion

    // Use this for initialization
    void Start()
    {
        // enemyList, playerList initialize
        if(playerUnitList.Count == 0)
        {
            GameObject[] playerObjArray = GameObject.FindGameObjectsWithTag("Player");
            for(int i =0 ; i<playerObjArray.Length;i++)
            {
                Unit unit = playerObjArray[i].GetComponent<Unit>();
                if(unit != null)
                    playerUnitList.Add(unit);
            }
        }

        if(enemyUnitList.Count == 0)
        {
            GameObject[] enemyObjArray = GameObject.FindGameObjectsWithTag("Enemy");
            for(int i =0 ; i<enemyObjArray.Length;i++)
            {
                Unit unit = enemyObjArray[i].GetComponent<Unit>();
                if(unit != null)
                    enemyUnitList.Add(unit);
            }
        }
        
        
        #region 기존 코드
        //enemyUnitList[0].StatManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 0;
        //enemyUnitList[1].StatManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 0;
        //enemyList[0].StatManager.CreateOrGetStat(E_StatType.MaxRange).ModifiedValue = 1;
        //enemyList[0].StatManager.CreateOrGetStat(E_StatType.MinRange).ModifiedValue = 0;
        #endregion

        // 플레이어 유닛 상태 초기화
        for (int i = 0; i < PlayerUnitList.Count; i++)
        {
            alivePlayerUnitList.Add(PlayerUnitList[i]);
            PlayerUnitList[i].UnitState = E_UnitState.Running;
        }

        for (int i = 0; i < enemyUnitList.Count; i++)
        {
            // enemyUnitList[i].StatManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 0;
            aliveEnemyUnitList.Add(enemyUnitList[i]);

            enemyUnitList[i].UnitState = E_UnitState.Idle;
        }

        leaderHero = playerUnitList[0];
        OnDeadEnemy += OnDeathEnemy;

        GameObject[] checkerObjArr = GameObject.FindGameObjectsWithTag("PhaseChecker");
        for (int i = 0; i < checkerObjArr.Length; i++)
        {
            PhaseChecker checker = checkerObjArr[i].GetComponent<PhaseChecker>();
            checkerList.Add(checker);

            checker.OnStartPhase += OnChangedPhase;
        }

        // 리스트 안에 있는 값들 중 제일 큰값
        for (int i = 0; i < checkerList.Count; i++)
        {
            if (lastPhase.CompareTo(checkerList[i].PhaseType)<0)
                lastPhase = checkerList[i].PhaseType;
        }

        
        OnChangedGamePhase.Execute(E_PhaseType.None);   // 초기화
        EnterTheBattle();
    }


    void OnDeathEnemy(Unit enemy)
    {
        aliveEnemyUnitList.Remove(enemy);
        if (aliveEnemyUnitList.Count == 0)
        {
            for (int i = 0; i < alivePlayerUnitList.Count; i++)
            {
                alivePlayerUnitList[i].GetComponent<Animator>().SetBool("inBattle", false);
                alivePlayerUnitList[i].GetComponent<Animator>().SetBool("Victory",true);
                alivePlayerUnitList[i].UnitState = E_UnitState.Idle;
                alivePlayerUnitList[i].EnemyRange = E_Range.OutOfRange;


            }

            OnExecuteResult.Execute(true);
        }
    }

    void OnChangedPhase(E_PhaseType phase)
    {
        presentPhase = phase;
        OnChangedGamePhase.Execute(phase);
        // 페이지별로 적들을 움직이도록 처리?

        // 테스트용 2개. 좀 더 범용적으로 쓰려면 따로 처리해야함
        
        for(int i = 0; i<2;i++)
        {
            aliveEnemyUnitList[i].UnitState = E_UnitState.Running;
        }
    }
}

public partial class GameManager : MonoBehaviour
{
    private const float xPosOffset = 0.5f;

    [ContextMenu("LineUP")]
    //public void LineUpHpBar(List<Unit> unitList)
    public void LineUpHpBar(bool isRest)
    {
        List<Unit> unitList = PlayerUnitList;

        if (unitList == null)
            return;

        if (unitList.Count == 0)
            return;

        // 정렬할 유닛의 리스트를 받는다(적 또는 아군)
        List<Unit> targetUnitList = unitList;

        // 유닛들의 체력바 리스트 받기
        List<Transform> hpBarTransList = new List<Transform>();
        for (int i = 0; i < targetUnitList.Count; i++)
            hpBarTransList.Add(targetUnitList[i].Hp);

        // transform 한개를 중심으로 잡기
        Transform anchorTrans = hpBarTransList[0];  // 테스트용으로 처음 들어온 애

        if(isRest)
        {
            for (int i = 0; i < hpBarTransList.Count; i++)
                hpBarTransList[i].localPosition = anchorTrans.localPosition;
        }
        else
        {
            for (int i = 0; i < hpBarTransList.Count; i++)
                hpBarTransList[i].localPosition = new Vector3(anchorTrans.localPosition.x, anchorTrans.localPosition.y + i * 0.3f, 0);
        }
    }
}