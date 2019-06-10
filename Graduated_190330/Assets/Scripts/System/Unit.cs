using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;
using Priority_Queue;
using System.Text;

public delegate void EventListenerCasterOnly(Unit caster);
public delegate void EventListenerBoth(Unit caster, Unit target);

public enum E_UnitState
{
    Idle,
    Running,
    Attack,
    Dead,

}

public enum E_PlayerType
{
    None = -1,
    Leader,
    Sub1,
    Sub2,
}

public partial class Unit : MonoBehaviour
{
    [SerializeField] float UnitVelocity = 0f; 

    #region 힐 코드 확인용
    [SerializeField] float curHP;
    #endregion
    public Transform Hp;
    [SerializeField] Transform hpTrans;
    [SerializeField]
    private E_UnitState unitState = E_UnitState.Idle;
    public E_UnitState UnitState
    {
        get { return unitState; }
        set
        {
            if (unitState == value)
                return;

            unitState = value;
            switch (unitState)
            {
                case E_UnitState.Idle:
                    OnChangedIdle.Execute();
                    break;

                case E_UnitState.Running:
                    OnChangedRunning.Execute();
                    break;

                case E_UnitState.Attack:
                    OnChangedAttack.Execute();
                    break;

                case E_UnitState.Dead:
                    OnChangedDead.Execute();
                    break;
            }
        }
    }

    [SerializeField]private E_PlayerType playerType = E_PlayerType.None;
    public E_PlayerType PlayerType
    {
        get
        {
            if (groupTag == E_GroupTag.Enemy)
                return E_PlayerType.None;

            return playerType;
        }
        set
        {
            if (value == playerType)
                return;

            playerType = value;
        }
    }

    [SerializeField] private E_Class classType = E_Class.None;
    public E_Class ClassType { get { return classType; } }

    public Action OnChangedIdle { get; set; }
    public Action OnChangedRunning { get; set; }
    public Action OnChangedAttack { get; set; }
    public Action OnChangedDead { get; set; }

    //캐릭터 애니메이터
    Animator animator;

    //캐릭터 정보
    StatManager statManager = new StatManager();
    BuffManager buffManager = new BuffManager();
    public StatManager StatManager
    {
        get
        {
            return statManager;
        }
    }
    public BuffManager BuffManager
    {
        get
        {
            return buffManager;
        }
    }

    //스킬 시전 대기열
    public ActionQueueManager skillQueue = new ActionQueueManager();

}
public partial class Unit : MonoBehaviour
{
    //진영정보
    public E_GroupTag groupTag;

    //전투 관련
    bool inBattle;
    /// <summary>
    /// 전투 진입
    /// </summary>
    public void EnterBattle()
    {
        inBattle = true;
        foreach (Effect effect in EnterBattleEffect)
        {
            effect.ActivateEffect();
        }
    }
    /// <summary>
    /// 전투 이탈
    /// </summary>
    public void ExitBattle()
    {
        inBattle = false;
        foreach (Effect effect in ExitBattleEffect)
        {
            effect.ActivateEffect();
        }
    }
    public List<Effect> EnterBattleEffect = new List<Effect>();
    public List<Effect> ExitBattleEffect = new List<Effect>();

    /*
     * 사망
     * 사망시 버프,디버프 제거.
     * 상호작용 중단.
     * 사망 애니메이션 재생
     * 사망 시작 이벤트 발생
     * 사망 애니메이션 종료시 이벤트 발생
     */
    public bool IsAlive
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
            IsNormal = (!value) & (!isGroggy);
        }
    }
    public void DeadStart()
    {
        isGroggy = false;
        IsAlive = true;
        //모든 버프를 해제한다.
        //이 용사와 관계된 모든걸 해제한다.
        //사망 애니메이션 시작.
        //충돌체도 꺼야겠지?
        //아래 효과는 필요 없어 보인다. 이후 수정
        foreach (Effect effect in DeadStartEffect)
        {
            effect.ActivateEffect(this);
        }

        animator.SetBool("isDead", true);

        StartCoroutine(Death());
    }
    public void DeadEnd()
    {
        foreach (Effect effect in DeadEndEffect)
        {
            effect.ActivateEffect(this);
        }
    }
    public List<Effect> DeadStartEffect = new List<Effect>();
    public List<Effect> DeadEndEffect = new List<Effect>();
    IEnumerator Death()
    {
        animator.SetBool("inBattle", false);
        animator.SetBool("isDead", true);
        AnimationClip ani = GetAnimationClip("Death");
        if (ani == null)
            yield break;

        
        yield return new WaitForSeconds(ani.length + 1.5f);

        DeadEnd();
        gameObject.SetActive(false);
        GameManager.instance.OnDeadEnemy.Execute(this);
    }

    public AnimationClip GetAnimationClip(string name)
    {
        if (!animator) return null; // no animator

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null; // no clip by that name
    }







    /// <summary>
    /// 부활
    /// </summary>
    public void Rebirth()
    {
        IsAlive = false;
        //사망과 반대겠지?
    }
    public List<Effect> RebirthEffect = new List<Effect>();


    //기절 관련
    public bool IsGroggy
    {
        get
        {
            return isGroggy;
        }
        set
        {
            isGroggy = value;
            IsNormal = (!value) & (!isDead);
        }
    }
    /// <summary>
    /// 기절 시작
    /// </summary>
    public void GroggyStart()
    {
        IsGroggy = true;
        //시전중이던 모션 캔슬
        //그로기 애니메이션 실행
        //기절중 스킬 스택 저장여부는 확인해봐.
        foreach (Effect effect in GroggyStartEffect)
        {
            effect.ActivateEffect();
        }
    }
    /// <summary>
    /// 기절 종료
    /// </summary>
    public void GroggyEnd()
    {
        IsGroggy = false;
        //그로기 애니메이션 종료
        //달리기 모션으로 되돌림.

        foreach (Effect effect in GroggyEndEffect)
        {
            effect.ActivateEffect();
        }
    }
    public List<Effect> GroggyStartEffect = new List<Effect>();
    public List<Effect> GroggyEndEffect = new List<Effect>();


    //정상 여부
    bool isNormal;
    public bool IsNormal
    {
        get
        {
            return isNormal;
        }
        set
        {
            isNormal = value;
        }
    }
    //적과 사정거리
    //사거리 관련
    public E_Range EnemyRange = E_Range.OutOfRange;
    E_Range deltaRange = E_Range.OutOfRange;
    float enemyDistance = 0;

    void RangeSearchStart()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        StartCoroutine(RangeSearch());
    }

    void StopSearchStart()
    {
        StopCoroutine(RangeSearch());
    }
    Vector3 direction;
    IEnumerator RangeSearch()
    {
        StatFloat maxRange = StatManager.CreateOrGetStat(E_StatType.MaxRange);
        StatFloat minRange = StatManager.CreateOrGetStat(E_StatType.MinRange);

        //레이어 마스크를 지정함
        //만약에 게임에서 아군으로 만드는 기능이 있다면 이부분을 루프안으로 넣어야한다.
        LayerMask mask = ~(1 << gameObject.layer);

        Vector2 origin;
        RaycastHit2D hit;

        //적과의 사거리
        direction = transform.worldToLocalMatrix.MultiplyVector(transform.right);
        while (true)
        {
            

            origin = new Vector2(transform.position.x + (0.301f * direction.x), transform.position.y + 0.1f);
            //나중에 위해서 메모를 하는건데
            //여기를 Transfrom으로 아예 저장을 해서 쉽게 처리가 가능할거같다. 한번 생각해봐라.
            //추가적으로 raycast2D를 새로 정의내려도 되고.
            if (hit = Physics2D.Raycast(origin, direction, float.MaxValue, mask))
            {
                if (!hit.transform.CompareTag(tag))
                {
                    enemyDistance = direction.x * (hit.transform.position.x - transform.position.x);

                    //최소 사거리 내
                    if (enemyDistance < minRange.ModifiedValue)
                    {
                        if (deltaRange != E_Range.WithInMinRange)
                        {
                            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
                            //Debug.Log(name + " 사거리밖>> 사거리안");
                        }
                        EnemyRange = E_Range.WithInMinRange;
                        deltaRange = EnemyRange;
                    }//최대 사거리 밖
                    else if (enemyDistance < maxRange.ModifiedValue)
                    {
                        EnemyRange = E_Range.WithInMaxRange;
                        deltaRange = EnemyRange;

                    }
                    else
                    {
                        EnemyRange = E_Range.OutOfRange;
                        deltaRange = EnemyRange;

                    }

                    //Debug.Log(name + " find Enemy : " + hit.transform.name + "(" + enemyDistance + "m) " + enemyRange.ToString());
                }
            }
            yield return null;
        }

        //레이로 탐지
        //레인지로 가장 앞의 적과 사거리 관계를 비교하여 갱신
        //최소 사거리 밖인지
        //아니라면 최소 사거리 안?
        //둘다아니면 당연 정지
    }
    public bool TestRunning = false;
    //달리기 관련 
    Rigidbody2D rigid2D;
    void RunningStart()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        StartCoroutine(Running());
    }
    IEnumerator Running()
    {
        StatFloat moveSpeed = statManager.CreateOrGetStat(E_StatType.MoveSpeed);
        while (true)
        {
            if (isNormal)
            {
                if (inBattle)
                {
                    //스킬 큐가 비어있는지 체크하는 부분
                    if (skillQueue.IsEmpty)
                    {
                        if (UnitState == E_UnitState.Running)
                        {
                            //달리기 애니메이션이 미실행중이라면 애니메이션 재생
                            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                            {
                                //Debug.LogError(transform.name + ", enemyRange: " + enemyRange);
                                if (EnemyRange != E_Range.WithInMinRange)
                                {
                                    //Debug.Log(name + " "+ enemyDistance);
                                    //rigid2D.velocity = direction * moveSpeed.ModifiedValue;
                                    rigid2D.AddForce(direction * moveSpeed.ModifiedValue, ForceMode2D.Force);

                                }
                                else
                                {
                                    rigid2D.AddForce(-direction * moveSpeed.ModifiedValue * 0.5f, ForceMode2D.Force);
                                }
                            }
                        }
                    }
                }
            }
            yield return null;

            //if (!isNormal)
            //    yield return null;

            //if (!inBattle)
            //    yield return null;

            //if (!skillQueue.IsEmpty)    // 스킬큐가 비어있는지 체크
            //    yield return null;

            //if (unitState != E_UnitState.Running)
            //    yield return null;

            //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            //{
            //    if (EnemyRange != E_Range.WithInMinRange)
            //        rigid2D.AddForce(direction * moveSpeed.ModifiedValue, ForceMode2D.Force);
            //    else
            //        rigid2D.AddForce(-direction * moveSpeed.ModifiedValue * 0.5f, ForceMode2D.Force);

            //    yield return null;
            //}

        }
    }

    //스킬 자동 시전



    //기본 공격
    public bool isAttacking = false;
    void BaseAttackCoolDownStart()
    {
        StartCoroutine(BaseAttackChecking());
    }
    IEnumerator BaseAttackChecking()
    {
        if (skillList.Count == 0)
        {
            yield break;
        }
        Skill skill = skillList[0];

        StatFloat attackSpeed = statManager.CreateOrGetStat(E_StatType.CurrentAttackSpeed);
        float currentCoolTime = 10;
        float attackPeriod = 1f;
        isAttacking = false;
        while (true)
        {
            //공격이 끝난 후부터 쿨다운 시작.

            if ((!isAttacking) & (attackPeriod > currentCoolTime))
            {
                currentCoolTime += Time.deltaTime * attackSpeed.ModifiedValue;
            }
            else
            {
                //if ((EnemyRange != E_Range.OutOfRange) & (skillQueue.IsEmpty)) & (!isAttacking))
                if ((EnemyRange != E_Range.OutOfRange) & (skillQueue.IsEmpty))
                {
                    currentCoolTime = 0;
                    isAttacking = true;
                    //평타를 추가하는 곳
                    if (actionQueue.Count == 0)
                    {
                        //if (groupTag != E_GroupTag.Player)
                        actionQueue.Enqueue("Attack1", (int)E_AttackPriority.Third);

                        //UnitState = E_UnitState.Attack;
                    }
                }
            }

            yield return null;
        }
    }
    public void SetIsntAttack()
    {
        isAttacking = false;
    }
    public void SetIsAttack()
    {
        isAttacking = true;
    }
    public List<Skill> baseAttackList = new List<Skill>();


    //상호작용 목록
    public void GetHit(ref float damage, E_EffectType damageType, float penetrationPower, bool isCritical)
    {

        //버프 순회
        //피격 판정
        GetDamage(ref damage, damageType, penetrationPower, isCritical);
        Debug.Log(name + " hit!");
    }

    public delegate void DamageEvent(ref float damage);
    public DamageEvent OnHitEvent;


    /// <summary>
    /// 피해를 입다.
    /// </summary>
    /// <param name="damage">오리지널 데미지량</param>
    /// <param name="damageType">데미지 타입</param>
    /// <param name="penetrationPower">공격자의 관통력</param>
    /// <param name="isCritical">크리티컬 여부</param>
    public void GetDamage(ref float damage, E_EffectType damageType, float penetrationPower, bool isCritical)
    {
        //if (!IsAlive)
        //    return;

        //방어력이나 버프가 적용될 데미지
        float calculatedDamage = damage;
        //
        float damageReducePercent = 0;
        /*
         * 버프로 인한 피해량 계산
         * 버프값, 데미지 속성,관통력,크리티컬 수치,시전자,
         */



        if (OnHitEvent != null)
        {
            OnHitEvent.Invoke(ref damage);
        }


        if (penetrationPower >= StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue)
        {
            damageReducePercent = 1;
        }
        else
        {
            damageReducePercent = (100 / (((StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue - penetrationPower) * 0.348f) + 100));
        }

        /*
         * 오리지널 데미지
         * 계산된 데미지
         * 
         * 피해 응답 처리(ref 오리지널 데미지)
         * 
         * 피해량 계산
         * 1. 피해 응답 처리
         * 2. 피해 감소 적용
         * 3. 속성별 방어도 적용
         * 4. 
         * 
         * =계산된 피해량.
         *
         * 
         * 
         * Function 피해량 출력(계산된 피해량);
         * 계산된 데미지가 오리지널보다 15%이상 적다면 비관통
         * 아니면 관통 출력
         * 
         * 
         * 
         * 
         */

        //원래 데미지와 비교하여 관통치가 15%차이가 난다면 비관통으로 출력

        E_FloatingType floatingType = E_FloatingType.NonpenetratingDamage;

        calculatedDamage *= damageReducePercent;
        if (damageReducePercent > 0.85f)
        {
            floatingType = E_FloatingType.FullPenetrationDamage;
        }

        //크리티컬 계산
        if (isCritical)
        {
            floatingType = E_FloatingType.CriticalDamage;
        }

        if(calculatedDamage < 0)
            floatingType = E_FloatingType.Heal;


        //float currentHP = StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue;
        float currentHP = StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue;
        StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue -= calculatedDamage;
        FloatingNumberManager.FloatingNumber(gameObject, calculatedDamage, floatingType);

        if (currentHP <= StatManager.CreateOrGetStat(E_StatType.MinHealth).ModifiedValue)
        {
            UnitState = E_UnitState.Dead;
        }

        #region 힐 코드
        if (currentHP >= StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue)
            currentHP = StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue;

        curHP = currentHP;
        #endregion

        if (hpTrans != null)
        {
            float hpPer = currentHP / StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue;
            if (hpPer <= 0)
                hpPer = 0;

            hpTrans.localScale = new Vector3(hpPer, hpTrans.localScale.y, hpTrans.localScale.z);
        }

        #region To Check code
        //Debug.Log(transform.name + "HP: " + StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue);
        #endregion
    }


    public void GetHeal(ref float healAmount)
    {
        float currentHP = StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue;
        StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue += healAmount;
    }


    //애니메이션 트리거(애니메이션 이벤트)
    public void SkillActivate(int skillNum)
    {
        skillList[skillNum].ActivateEffect(this);
    }

    public List<Effect> effectList = new List<Effect>();
    public List<Skill> skillList = new List<Skill>();

    //n체인 스킬 사용.








    //n체인 트리거
    bool ChainTrigger1;
    bool ChainTrigger2;
    bool ChainTrigger3;







    /*
     * 상태 확인 [그로기, 사망이면 비정상]
     * 
     * 각각 이벤트를 갖고있다.
     * 
     * 그로기 시작
     * 그로기 중
     * 그로기 완료
     * 
     * 
     * 사망 시작
     * 사망 중
     * 사망 완료
     * 
     * 기본 공격 시작
     * 기본 공격 중
     * 기본 공격 종료
     * 
     * 
     *
     * 
     * 달리기 - 전진
     * 달리기 - 정지
     * 달리기 - 후진
     * 
     * 
     * 기본 공격 사거리 탐지 시작
     * 기본 공격 사거리 탐지 중
     * 
     * 1체인 블록 스킬 시전 시작
     * 1체인 블록 스킬 시전 중
     * 1체인 블록 스킬 시전 종료
     * 
     * 2체인 블록 스킬 시전 시작
     * 2체인 블록 스킬 시전 중
     * 2체인 블록 스킬 시전 종료
     * 
     * 3체인 블록 스킬 시전 시작
     * 3체인 블록 스킬 시전 중
     * 3체인 블록 스킬 시전 종료
     * 
     * 1체인 트리거 장전
     * 2체인 트리거 장전
     * 3체인 트리거 장전
     * 
     * 블록 사용
     * 
     * 
     * 
     * 
     * 
     * 패시브 시작
     * 패시브 중지
     * 
     * 
     * 
     * 
     * 상호 작용 시작
     * 상호 작용 중지
     * 
     * 
     * 넉백 시작
     * 넉백 중
     * 넉백 종료
     * 
     * 
     * 사망 조건 체크
     * 
     * 
     * 전투 진입
     * 전투 중
     * 전투 이탈
     * 
     * 
     * 피격 당함
     * 피해 입음
     * 
     * 회복 받음
     * 
     * 스테이터스 변화
     * 
     * 
     * SP 상승
     * SP 감소
     * 
     * 회피 체크
     * 
     * 명중 체크
     * 
     * 치명타 체크
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 각종 이벤트를 아래 기술
     * 
     * 그로기 시작 이벤트
     * 그로기 종료 이벤트
     * 
     * 사망 시작 이벤트
     * 사망 종료 이벤트
     * 
     * 기본 공격 시작 이벤트
     * 기본 공격 종료 이벤트
     * 
     * 달리기 - 전진 이벤트
     * 달리기 - 정지 이벤트
     * 달리기 - 후진 이벤트
     * 
     * 넉백 시작 이벤트
     * 넉백 종료 이벤트
     * 
     * 1체인 스킬 시전 시작 이벤트
     * 1체인 스킬 시전 종료 이벤트
     * 2체인 스킬 시전 시작 이벤트
     * 2체인 스킬 시전 종료 이벤트
     * 3체인 스킬 시전 시작 이벤트
     * 3체인 스킬 시전 종료 이벤트
     * 
     * 
     * 1체인 트리거 이벤트
     * 2체인 트리거 이벤트
     * 3체인 트리거 이벤트
     * 
     * 블록 사용 이벤트
     * 3체인 트리거 이벤트
     * 
     * 회피 이벤트
     * 명중 이벤트
     * 
     * 
     * 전투 진입 이벤트
     * 전투 이탈 이벤트
     * 
     * 
     * 캐릭터 부활 이벤트
     * 
     * 피격 이벤트
     * 회복 이벤트
     * 
     * 스테이터스 변화 이벤트
     * 
     * SP 상승 이벤트
     * SP 감소 이벤트
     * 
     * 감속 이벤트
     * 
     * 치명타 발동 이벤트
     * 
     * 
     */







    public void SkillQueueAdd(int index)
    {

    }




}
//기본 행동
public partial class Unit : MonoBehaviour
{
    //절대 다른 클래스 인자를 직접 제어하지 말것.

    bool isInbattle;
    bool isDead;
    bool isActing;
    bool isGroggy;
    bool isRestriction;

    //bool isHitWithPlayer;


    List<Effect> createEffect = new List<Effect>();
    void OnCreate()
    {
        foreach (Effect effect in createEffect)
        {
            effect.RefreshCasterBasedAmount(this);
            effect.ActivateEffect(this);
        }
        //AI 재생
        IsGroggy = false;
        isActing = false;
        isRestriction = false;
        IsAlive = false;
        isInbattle = true;

        //RunningStart();
        RangeSearchStart();
        BaseAttackCoolDownStart();
        ActionQueueCheckingStart();
        //기본 공격 시작.
        //행동 대기열재생시작
        //각 상태 정상화
        //
    }

    List<Effect> reBirthEffect = new List<Effect>();
    void OnReBirth()
    {
        foreach (Effect effect in reBirthEffect)
        {
            effect.RefreshCasterBasedAmount(this);
            effect.ActivateEffect(this);
        }

        IsGroggy = false;
        isActing = false;
        isRestriction = false;
        IsAlive = false;
        isInbattle = true;

        //RunningStart();
        RangeSearchStart();
        BaseAttackCoolDownStart();
        actionQueue.Clear();
        ActionQueueCheckingStart();
        //달리기 시작.
        //기본 공격 시작.
        //행동 대기열재생시작
        //상태 정상화
    }

    //사망하면
    //중단 리스트
    //행동 중단.
    //상호작용 X[부활 제외]
    //이동,대기 불가
    //자동 공격 중단
    //기절해제
    //버프소멸[소멸가능한것만]
    //속박해제
    List<Effect> deathEffect = new List<Effect>();
    EventListenerCasterOnly deathEventListener;
    void OnDead()
    {
        foreach (Effect effect in reBirthEffect)
        {
            effect.RefreshCasterBasedAmount(this);
            effect.ActivateEffect(this);
        }
        //스킬 대기열 자동시전 중단.
        //이동 중단.
        //사거리 탐지 중단
        //상호작용 중단
        //버프 해제
    }

    #region 기존 콜백(구현 x)
    ////전투중=true면 움직이기 시작.
    ////전투중=false면 대기중.
    //void OnWait()
    //{

    //}
    ////대상이 최소 사거리 밖이면 전진
    //void OnFoward()
    //{

    //}
    ////대상이 최소 사거리 내면 후진
    //void OnBackWard()
    //{

    //}

    ////전투중=true면 공격 가능
    ////최대 사거리 내면 공격 시작.
    ////행위 대기열이 없으면 쿨다운 시작.
    //void StartAttack()
    //{

    //}
    //void OnAttack()
    //{

    //}
    //void StopAttacking()
    //{

    //}

    //void StartRangeSearch()
    //{

    //}
    //void OnRangeSearch()
    //{

    //}
    //void StopRangeSearch()
    //{

    //}

    ////기절
    //void StartGroggy()
    //{

    //}
    //void OnGroggy()
    //{

    //}
    //void EndGroggy()
    //{

    //}

    ////속박
    //void StartRestriction()
    //{

    //}
    //void OnRestriction()
    //{

    //}
    //void EndRestriction()
    //{

    //}
    #endregion

    public void StartCollision(Projectile projectile)
    {
        //Debug.Log(name + " Start Collision");
        projectile.OnPenetration();
    }
    public void EndCollision(Projectile projectile)
    {
        //Debug.Log(name + " End Collision");
    }


}
//행동 대기열
public partial class Unit : MonoBehaviour
{
    SimplePriorityQueue<string, int> actionQueue = new SimplePriorityQueue<string, int>();
    bool IsActionQueueEmpty
    {
        get
        {
            return (actionQueue.Count == 0);
        }
    }
    //행동 큐 상시 체크
    void ActionQueueCheckingStart()
    {
        StartCoroutine(ActingChecking());
    }
    IEnumerator ActingChecking()
    {
        string currentSkillInfo = string.Empty;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (true)
        {
            //if (groupTag == E_GroupTag.Enemy)
            //    Debug.LogError("RunningStart");

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(currentSkillInfo))
            {
                if (!IsActionQueueEmpty)
                {
                    rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
                    if (string.Equals(currentSkillInfo, Constant.BASE_ATTACK))// && groupTag != E_GroupTag.Player)
                    {
                        SkillActivate(0);
                    }

                    currentSkillInfo = actionQueue.Dequeue();
                    animator.Play(currentSkillInfo);
                }
            }
            yield return null;
        }
    }




}
//상호작용
public partial class Unit : MonoBehaviour
{

}
//스킬과 효과
public partial class Unit : MonoBehaviour
{
    Dictionary<string, Effect> effectDict = new Dictionary<string, Effect>();

    /// <summary>
    /// 행동 대기열에 스킬을 추가한다.
    /// </summary>
    /// <param name="skillType">스킬의 종류</param>
    /// <param name="skillChain">스킬의 체인수</param>
    public void AddSkillQueue(E_SkillType skillType, int skillChain)
    {
        switch (skillType)
        {
            case E_SkillType.Normal:
                actionQueue.Enqueue(string.Concat("BlockSkill", skillChain.ToString()), (int)E_AttackPriority.Second);
                break;
            case E_SkillType.Special:
                actionQueue.Enqueue(string.Concat("SpecialSkill", skillChain.ToString()), (int)E_AttackPriority.Second);
                break;
            default://이후에 추가되는 스킬 타입에 대해 추가할것
                break;

        }
    }
}

//시작 제어
public partial class Unit : MonoBehaviour
{
    private void Awake()
    {
        animator = GetComponent<Animator>();

        OnChangedIdle += OnIdle;
        OnChangedRunning += OnRunning;
        OnChangedAttack += OnAttack;
        OnChangedDead += OnDeath;
    }
    private void Start()
    {
        #region 기존 코드
        //RangeSearchStart();
        //RunningStart();
        //IsNormal = true;
        //inBattle = true;
        //ActionQueueCheckingStart();
        //BaseAttackCoolDownStart();
        #endregion

        curHP = StatManager.CreateOrGetStat(E_StatType.MaxHealth).ModifiedValue;
        UnitVelocity = StatManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue;
    }
}

public partial class Unit : MonoBehaviour
{
    void OnIdle()
    {

    }

    void OnRunning()
    {
        RangeSearchStart();
        RunningStart();
        IsNormal  = true;
        inBattle = true;
        ActionQueueCheckingStart();
        BaseAttackCoolDownStart();
    }

    void OnAttack()
    {

    }

    void OnDeath()
    {
        DeadStart();
        Debug.LogError(gameObject.name + " is dead");
    }
}

//디버그
public partial class Unit : MonoBehaviour
{
    public void ShowAllStat()
    {
        foreach (E_StatType _statType in Enum.GetValues(typeof(E_StatType)))
        {
            Debug.Log(statManager.Get_Stat(_statType).StatName + "/" + statManager.Get_Stat(_statType).StatType + "/" + statManager.Get_Stat(_statType).ModifiedValue);
        }
    }

}

public partial class Unit:MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.LineUpHpBar(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameManager.instance.LineUpHpBar(true);
    }
}

    