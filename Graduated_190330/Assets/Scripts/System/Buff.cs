using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Buff : MonoBehaviour
{
    public string id;

    /// <summary>
    /// 시전자
    /// </summary>
    public Unit caster;
    /// <summary>
    /// 적용 대상
    /// </summary>
    public Unit target;

    /// <summary>
    /// 적용 대상.
    /// </summary>
    public E_ApplyTargetFilter applyTargetFilter;
    /// <summary>
    /// 적용 대상필터 검토후 적용
    /// </summary>
    void CheckTargetFilter()
    {
        switch (applyTargetFilter)
        {
            case E_ApplyTargetFilter.Caster:
                target = caster;
                break;
            /*case E_ApplyTargetFilter.Target:
                target = target;
                break;*/
            default:
                break;
        }
        transform.SetParent(target.transform);
    }
    /// <summary>
    /// 중첩 가능 여부
    /// </summary>
    public bool canOverlap;
    public bool isSeparateCaster;
    /// <summary>
    /// 최대 중첩수
    /// </summary>
    public int maxStackCount;
    public int currentStackCount;

    /// <summary>
    /// 버프 타입
    /// </summary>
    public E_BuffType buffType;
    /// <summary>
    /// 버프 정렬
    /// </summary>
    public E_BuffOrder buffOrder;


    /// <summary>
    /// 지속시간. -1이면 무기한 지속
    /// </summary>
    public float durationTime;
    public float currentRemainTime;
    /// <summary>
    /// 반복 주기
    /// </summary>
    public float reiterationPeriod;
    public float currentReiterationPeriod;
    /// <summary>
    /// 반복 횟수. -1이면 제한 없음
    /// </summary>
    public int repeatCount;
    public int currentRepeatCount;


    /// <summary>
    /// 시간 배율 출처. 버프내에서 발생하는 시간값에 대한 가속 배율의 출처
    /// </summary>
    public E_ApplyTargetFilter timeMultiplierSource;
    /// <summary>
    /// 시간 가속 배율
    /// </summary>
    public float timeMultiplier;
    /// <summary>
    /// 시간 배율 새로고침
    /// </summary>
    /// <param name="targetSource"></param>                                                  
    public void RefreshTimeMultiplier(float targetSource)
    {
        timeMultiplier = targetSource;
    }
    void AddEventAboutTimeMultiplier()
    {
        switch (timeMultiplierSource)
        {
            case E_ApplyTargetFilter.Caster:
                caster.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).AddEvent(RefreshTimeMultiplier);
                break;
            case E_ApplyTargetFilter.Target:
                target.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).AddEvent(RefreshTimeMultiplier);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 시간 배율 출처에 관한 이벤트 제거
    /// </summary>
    void RemoveEventAboutTimeMultiplier()
    {
        switch (timeMultiplierSource)
        {
            case E_ApplyTargetFilter.Caster:
                caster.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).RemoveEvent(RefreshTimeMultiplier);
                break;
            case E_ApplyTargetFilter.Target:
                target.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).RemoveEvent(RefreshTimeMultiplier);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 시간 배율을 즉시 동기화 시킨다.
    /// </summary>
    void ForcedSyncTimeMultiplier()
    {
        switch (timeMultiplierSource)
        {
            case E_ApplyTargetFilter.Caster:
                timeMultiplier = caster.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).ModifiedValue;
                break;
            case E_ApplyTargetFilter.Target:
                timeMultiplier = target.StatManager.CreateOrGetStat(E_StatType.TimeAccelerationRate).ModifiedValue;
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// 추가 스텟
    /// </summary>
    public List<AdditinalStat> additianalStatList = new List<AdditinalStat>();

    [System.Serializable]
    public class AdditinalStat
    {
        public E_StatType targetStat;
        public float totalStat;
        public AmountSet addStat;
        public void SetTotalStat(Unit caster, Unit target)
        {
            totalStat = addStat.FixedAmount + addStat.SetAmountCasterBased(caster) + addStat.SetAmountTargetBased(target);
        }
    }

    /// <summary>
    /// 최초 생성 효과 리스트
    /// </summary>
    public List<Effect> createEffectList;
    /// <summary>
    /// 지속시간 만료 효과 리스트
    /// </summary>
    public List<Effect> durationEndEffectList;
    /// <summary>
    /// 버프 소멸 효과 리스트
    /// </summary>
    public List<Effect> destroyEffectList;
    /// <summary>
    /// 새로고침 효과 리스트
    /// </summary>
    public List<Effect> refreshEffectList;
    /// <summary>
    /// 주기 반복 효과 리스트
    /// </summary>
    public List<Effect> periodEffectList;
    /// <summary>
    /// 피해 응답 효과 리스트
    /// </summary>
    public List<Effect> onHitEffect;
    
    /// <summary>
    /// 생성 효과 발동
    /// </summary>
    void CreatEffectActivate()
    {
        RefreshEffectAll(createEffectList);
        ActivateEffectAll(createEffectList);
    }
    /// <summary>
    /// 지속시간 만료 효과 발동
    /// </summary>
    void DurationEndEffectActivate()
    {
        RefreshEffectTargetBasedOnly(durationEndEffectList);
        ActivateEffectAll(durationEndEffectList);
    }
    /// <summary>
    /// 버프 소멸 효과 발동
    /// </summary>
    void DestroyEffectActivate()
    {
        RefreshEffectTargetBasedOnly(destroyEffectList);
        ActivateEffectAll(destroyEffectList);
    }
    /// <summary>
    /// 새로고침 효과 발동.
    /// </summary>
    public void RefreshEffectActivate()
    {
        RefreshEffectTargetBasedOnly(refreshEffectList);
        ActivateEffectAll(refreshEffectList);
    }
    /// <summary>
    /// 주기 반복 효과 발동
    /// </summary>
    void PeriodEffectActivate()
    {
        RefreshEffectTargetBasedOnly(periodEffectList);
        ActivateEffectAll(periodEffectList);
    }
    /// <summary>
    /// 피해 응답 효과 발동
    /// </summary>
    /// <param name="amount"></param>
    public void OnHitEffectActivate(ref float amount)
    {
        for (int index = 0; index < onHitEffect.Count; index++)
        {
            onHitEffect[index].ActivateEffect(caster, target, ref amount, currentStackCount);
        }
    }
    
    /// <summary>
    /// 버프 새로고침. 지속시간 초기화, 제한횟수 초기화.
    /// </summary>
    public void RefreshBuff()
    {
        //현재 체력이 늘어나는거면 빠질때 1은 남기도록 해야한다.
        //아니면 시스템 설계에 따라서 맞게 바꾸도록 한다.
        foreach (AdditinalStat addedstat in additianalStatList)
        { 
            target.StatManager.CreateOrGetStat(addedstat.targetStat).ModifiedValue -= addedstat.totalStat;
            addedstat.SetTotalStat(caster, target);
            addedstat.totalStat*= currentStackCount;
            target.StatManager.CreateOrGetStat(addedstat.targetStat).ModifiedValue += addedstat.totalStat;
        }

        currentRemainTime = durationTime;
        currentRepeatCount = 0;
        RefreshEffectAll(periodEffectList);
        RefreshEffectActivate();
    }
    /// <summary>
    /// 버프 중첩 적용
    /// </summary>
    public void OverlapBuff(/*애드 스택*/)
    {
        if (currentStackCount < maxStackCount)
        {
            currentStackCount++;
        }

        RefreshBuff();
    }

    /// <summary>
    /// 리스트 내 모든 이펙트를 발동
    /// </summary>
    /// <param name="effectList">발동시킬 이펙트리스트</param>
    void ActivateEffectAll(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].ActivateEffect(caster, target, currentStackCount);
        }
    }
    /// <summary>
    /// 리스트 내의 모든 이펙트의 대상비례값을 갱신한다.
    /// </summary>
    /// <param name="effectList"></param>
    void RefreshEffectTargetBasedOnly(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].RefreshTargetBasedAmount(target);
        }
    }
    /// <summary>
    /// 리스트 내의 모든 이펙트내 값들을 갱신한다.
    /// </summary>
    /// <param name="effectList"></param>
    void RefreshEffectAll(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].RefreshAllAmount(caster, target);
        }
    }




    /// <summary>
    /// 버프 활성화
    /// </summary>
    public void Activate()
    {
        CheckTargetFilter();
        ForcedSyncTimeMultiplier();
        AddEventAboutTimeMultiplier();
        //최초 시작시 적용할 효과 발생
        foreach(AdditinalStat addedstat in additianalStatList)
        {
            addedstat.SetTotalStat(caster,target);
            addedstat.totalStat *= currentStackCount;
            target.StatManager.CreateOrGetStat(addedstat.targetStat).ModifiedValue += addedstat.totalStat;
        }
        StartCoroutine(Duration());
        


    }
    /// <summary>
    /// 지속시간 유지,주기적인 효과 발동, 반복횟수 체크
    /// </summary>
    /// <returns></returns>
    IEnumerator Duration()
    {
        RefreshEffectAll(periodEffectList);
        currentReiterationPeriod = 0;
        currentRepeatCount = 0;
        if (durationTime >= 0)    //일정 시간동안 지속
        {
            currentRemainTime = durationTime;
            while (true)
            {
                //지속시간이 존재할때.
                if (currentRemainTime >= 0)
                {
                    currentRemainTime -= Time.deltaTime * timeMultiplier;

                    //주기 계산
                    if (currentReiterationPeriod < reiterationPeriod)//주기가 차지 않으면 채운다.
                    {
                        currentReiterationPeriod += Time.deltaTime * timeMultiplier;
                    }
                    else//주기가 가득참
                    {
                        if (repeatCount == -1)//무제한 발생
                        {
                            //효과 발생
                            PeriodEffectActivate();
                        }
                        else if (currentRepeatCount < repeatCount - 1)//반복 횟수 제한
                        {
                            //효과발생
                            PeriodEffectActivate();
                            currentRepeatCount += 1;
                        }
                        else//반복 횟수를 초과
                        {
                            break;
                        }
                        currentReiterationPeriod -= reiterationPeriod - (Time.deltaTime * timeMultiplier);
                    }
                }
                else//지속시간이 만료되었을때.
                {
                    break;
                }

                yield return null;
            }
            //마지막 효과 발동
            PeriodEffectActivate();
            //만료 효과 발생
            DurationEndEffectActivate();
        }
        else                    //영구 지속
        {
            while (true)
            {

                //주기 계산
                if (currentReiterationPeriod < reiterationPeriod)//주기가 차지 않으면 채운다.
                {
                    currentReiterationPeriod += Time.deltaTime * timeMultiplier;
                }
                else//주기가 가득참
                {
                    if (repeatCount == -1)//무제한 발생
                    {
                        //효과 발생
                        PeriodEffectActivate();
                    }
                    else if (currentRepeatCount < repeatCount - 1)//반복 횟수 제한
                    {
                        //효과발생
                        PeriodEffectActivate();
                        currentRepeatCount += 1;
                    }
                    else//반복 횟수를 초과
                    {
                        break;
                    }
                    currentReiterationPeriod -= reiterationPeriod - (Time.deltaTime * timeMultiplier);
                }

                yield return null;
            }
            //마지막 효과 발동
            PeriodEffectActivate();
        }
        target.BuffManager.RemoveBuff(this);
        yield return null;
    }
    /// <summary>
    /// 해당 버프를 즉시 소멸시킨다.
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 버프 소멸시 발생 효과
    /// </summary>
    void OnDestroy()
    {
        foreach (AdditinalStat addedstat in additianalStatList)
        {
            target.StatManager.CreateOrGetStat(addedstat.targetStat).ModifiedValue -= addedstat.totalStat;
        }
        DestroyEffectActivate();
        RemoveEventAboutTimeMultiplier();
    }

    /*


    //기초 정보
    public string id;
    public string Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }
    /// <summary>
    /// 지속시간. -1이면 무기한 지속
    /// </summary>
    public float durationTime;
    /// <summary>
    /// 최대 중첩수
    /// </summary>
    public int maxStackCount;
    /// <summary>
    /// 반복 주기
    /// </summary>
    public float reiterationPeriod;
    /// <summary>
    /// 반복 횟수. -1이면 제한 없음
    /// </summary>
    public int repeatCount;
    /// <summary>
    /// 버프 타입
    /// </summary>
    public E_BuffType buffType;
    /// <summary>
    /// 버프 정렬
    /// </summary>
    public E_BuffOrder buffSort;
    public E_ApplyTargetFilter applyTargetFilter;

    void Start()
    {
        CreatEffectActivate();
    }


    /// <summary>
    /// 현재 남은 지속시간
    /// </summary>
    public float currentDurationTime;
    /// <summary>
    /// 현재 반복중인 내부 시간
    /// </summary>
    public float instanceReiterationPeriod;
    /// <summary>
    /// 현재 반복 횟수
    /// </summary>
    public int instanceCount;
    /// <summary>
    /// 해당 버프 활성 상태. true=작동 가능
    /// </summary>
    //bool activate = true;
    public int currentStackCount = 1;


    //관여 개체
    /// <summary>
    /// 시전자
    /// </summary>
    public Unit caster;
    public Unit Caster
    {
        get
        {
            return caster;
        }
        set
        {
            caster = value;
        }
    }
    /// <summary>
    /// 동작 적용 대상
    /// </summary>
    public Unit applyTarget;
    /// <summary>
    /// 시간 배율 출처. 버프내에서 발생하는 시간값에 대한 가속 배율의 출처
    /// </summary>
    public E_ApplyTargetFilter timeMultiplierSource;
    /// <summary>
    /// 시간 가속 배율
    /// </summary>
    public float timeMultiplier;


    public Unit ApplyTarget
    {
        get
        {
            return applyTarget;
        }
        set
        {
            applyTarget = value;
        }
    }
    public float TimeMultiplier
    {
        get
        {
            return timeMultiplier;
        }
        set
        {
            timeMultiplier = value;
        }
    }

    /// <summary>
    /// 적용 대상필터 검토후 적용
    /// </summary>
    void CheckTargetFilter()
    {
        switch (applyTargetFilter)
        {
            case E_ApplyTargetFilter.Caster:
                applyTarget = caster;
                break;
            case E_ApplyTargetFilter.Target:
                break;
            default:
                applyTarget = ApplyTarget;
                break;
        }
    }

    /// <summary>
    /// 버프 활성화
    /// </summary>
    public void Activate()
    {
        CheckTargetFilter();
        ForcingSyncTimeMultiplier();
        //최초 시작시 적용할 효과 발생
        StartCoroutine(Duration());
    }
    /// <summary>
    /// 지속시간 유지,주기적인 효과 발동, 반복횟수 체크
    /// </summary>
    /// <returns></returns>
    IEnumerator Duration()
    {
        RefreshEffectAll(periodEffectList);
        instanceReiterationPeriod = 0;
        instanceCount = 0;
        if (durationTime >= 0)    //일정 시간동안 지속
        {
            currentDurationTime = durationTime;
            while (true)
            {
                //지속시간이 존재할때.
                if (currentDurationTime >= 0)
                {
                    currentDurationTime -= Time.deltaTime * timeMultiplier;

                    //주기 계산
                    if (instanceReiterationPeriod < reiterationPeriod)//주기가 차지 않으면 채운다.
                    {
                        instanceReiterationPeriod += Time.deltaTime * timeMultiplier;
                    }
                    else//주기가 가득참
                    {
                        if (repeatCount == -1)//무제한 발생
                        {
                            //효과 발생
                            PeriodEffectActivate();
                        }
                        else if (instanceCount < repeatCount - 1)//반복 횟수 제한
                        {
                            //효과발생
                            PeriodEffectActivate();
                            instanceCount += 1;
                        }
                        else//반복 횟수를 초과
                        {
                            break;
                        }
                        instanceReiterationPeriod -= reiterationPeriod - (Time.deltaTime * timeMultiplier);
                    }
                }
                else//지속시간이 만료되었을때.
                {
                    break;
                }

                yield return null;
            }
            //마지막 효과 발동
            PeriodEffectActivate();
            //만료 효과 발생
            DurationEndEffectActivate();
        }
        else                    //영구 지속
        {
            while (true)
            {

                //주기 계산
                if (instanceReiterationPeriod < reiterationPeriod)//주기가 차지 않으면 채운다.
                {
                    instanceReiterationPeriod += Time.deltaTime * timeMultiplier;
                }
                else//주기가 가득참
                {
                    if (repeatCount == -1)//무제한 발생
                    {
                        //효과 발생
                        PeriodEffectActivate();
                    }
                    else if (instanceCount < repeatCount - 1)//반복 횟수 제한
                    {
                        //효과발생
                        PeriodEffectActivate();
                        instanceCount += 1;
                    }
                    else//반복 횟수를 초과
                    {
                        break;
                    }
                    instanceReiterationPeriod -= reiterationPeriod - (Time.deltaTime * timeMultiplier);
                }

                yield return null;
            }
            //마지막 효과 발동
            PeriodEffectActivate();
        }
        //applyTarget.BuffManager.RemoveBuff(id);
        yield return null;
    }
    /// <summary>
    /// 해당 버프를 즉시 소멸시킨다.
    /// </summary>
    public void DestroyBuff()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 버프 소멸시 발생 효과
    /// </summary>
    void OnDestroy()
    {
        DestroyEffectActivate();
        RemoveEventAboutTimeMultiplier();
    }




    /// <summary>
    /// 버프 새로고침. 지속시간 초기화, 제한횟수 초기화.
    /// </summary>
    public void RefreshBuff()
    {
        currentDurationTime = durationTime;
        instanceCount = 0;
        RefreshEffectAll(periodEffectList);
        RefreshEffectActivate();
    }
    /// <summary>
    /// 버프 중첩 적용
    /// </summary>
    public void OverlapBuff()
    {
        if (currentStackCount < maxStackCount)
        {
            currentStackCount++;
        }

        RefreshBuff();
    }

    /// <summary>
    /// 리스트 내 모든 이펙트를 발동
    /// </summary>
    /// <param name="effectList">발동시킬 이펙트리스트</param>
    void ActivateEffectAll(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].ActivateEffect(caster, applyTarget, currentStackCount);
        }
    }
    /// <summary>
    /// 리스트 내의 모든 이펙트의 대상비례값을 갱신한다.
    /// </summary>
    /// <param name="effectList"></param>
    void RefreshEffectTargetBasedOnly(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].RefreshTargetBasedAmount(applyTarget);
        }
    }
    /// <summary>
    /// 리스트 내의 모든 이펙트내 값들을 갱신한다.
    /// </summary>
    /// <param name="effectList"></param>
    void RefreshEffectAll(List<Effect> effectList)
    {
        for (int index = 0; index < effectList.Count; index++)
        {
            effectList[index].RefreshAllAmount(caster, applyTarget);
        }
    }


    /// <summary>
    /// 최초 생성 효과 리스트
    /// </summary>
    public List<Effect> createEffectList;
    /// <summary>
    /// 지속시간 만료 효과 리스트
    /// </summary>
    public List<Effect> durationEndEffectList;
    /// <summary>
    /// 버프 소멸 효과 리스트
    /// </summary>
    public List<Effect> destroyEffectList;
    /// <summary>
    /// 새로고침 효과 리스트
    /// </summary>
    public List<Effect> refreshEffectList;
    /// <summary>
    /// 주기 반복 효과 리스트
    /// </summary>
    public List<Effect> periodEffectList;
    /// <summary>
    /// 피해 응답 효과 리스트
    /// </summary>
    public List<Effect> onHitEffect;

    /// <summary>
    /// 생성 효과 발동
    /// </summary>
    void CreatEffectActivate()
    {
        RefreshEffectAll(createEffectList);
        ActivateEffectAll(createEffectList);
    }
    /// <summary>
    /// 지속시간 만료 효과 발동
    /// </summary>
    void DurationEndEffectActivate()
    {
        RefreshEffectTargetBasedOnly(durationEndEffectList);
        ActivateEffectAll(durationEndEffectList);
    }
    /// <summary>
    /// 버프 소멸 효과 발동
    /// </summary>
    void DestroyEffectActivate()
    {
        RefreshEffectTargetBasedOnly(destroyEffectList);
        ActivateEffectAll(destroyEffectList);
    }
    /// <summary>
    /// 새로고침 효과 발동.
    /// </summary>
    public void RefreshEffectActivate()
    {
        RefreshEffectTargetBasedOnly(refreshEffectList);
        ActivateEffectAll(refreshEffectList);
    }
    /// <summary>
    /// 주기 반복 효과 발동
    /// </summary>
    void PeriodEffectActivate()
    {
        RefreshEffectTargetBasedOnly(periodEffectList);
        ActivateEffectAll(periodEffectList);
        //Debug.Log(name + "주기적 효과 발동");
    }
    /// <summary>
    /// 피해 응답 효과 발동
    /// </summary>
    /// <param name="amount"></param>
    public void OnHitEffectActivate(ref float amount)
    {
        for (int index = 0; index < onHitEffect.Count; index++)
        {
            onHitEffect[index].ActivateEffect(caster, applyTarget, ref amount, currentStackCount);
        }
    }*/
}
