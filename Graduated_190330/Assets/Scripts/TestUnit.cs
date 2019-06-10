using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public partial class TestUnit : MonoBehaviour
{
    public string id = string.Empty;
    public string heroName = string.Empty;
    public E_Class heroClass = E_Class.None;
    public int level = 0;
    public E_GroupTag groupTag;
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
}
public partial class TestUnit : MonoBehaviour
{
    public bool CanMove
    {
        get
        {
            return true;
        }
    }
    Vector3 direction;
    public void SetDirection()
    {
        direction = transform.worldToLocalMatrix.MultiplyVector(transform.right);
    }
    Transform destinationPosition;
    public void SetDestination(Transform transform)
    {
        destinationPosition = transform;
    }
    void MovePosition(Vector3 targetPosition)
    {
        if (direction.x >= 0)
        {
            if (targetPosition.x > transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime * 0.5f);
            }
        }
        else
        {
            if (targetPosition.x < transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime * 0.5f);
            }
        }
    }
    IEnumerator AutoMoving()
    {
        while(true)
        {
            if(CanMove)
            {
                if (destinationPosition)
                {
                    MovePosition(destinationPosition.position);
                }
            }
            yield return null;
        }
    }


}
//이벤트
public partial class TestUnit : MonoBehaviour
{
    public delegate void NormalEvent();

    public delegate void UnitEvent(TestUnit unit);
    public delegate void UnitFloatEvent(TestUnit unit, float amount);
    public delegate void UnitUnitEvent(TestUnit unit1, TestUnit unit2);
    public delegate void UnitEffectEvent(TestUnit unit, Effect effect);
    public delegate void UnitBuffEvent(TestUnit unit, Buff buff);
    public delegate void UnitHealEvent(TestUnit unit1, Heal damage);
    public delegate void UnitDamageEvent(TestUnit unit1,Damage damage);
    public delegate void UnitProjectileEvent(TestUnit unit, Projectile projectile);
    public delegate void UnitBlockEvent(TestUnit unit, Block block);

    public event UnitEvent BirthEvent;
    public void OnBirth()
    {
        //초기화
        BirthEvent(this);
    }
    public event UnitEvent DeadEvent;
    public void OnDeath()
    {
        /*
         * 사망에 이르는 피해에 대해서는 데미지 쪽에서 처리.
         * 피해량이 사망에 이르게 한다면 데미지를 따로 조작한다.
         * 사망시 즉시 부활개념은 아래 부활 이벤트로 제어
         */
        //상호작용 중단.[부활 제외]
        //모든 상태이상 제거
        //모든 디버프 제거
        //모든 버프 제거[가능한것만]
        //즉시 사망 애니메이션 실행
        DeadEvent(this);
    }
    public event UnitEvent Rebirth;
    public void OnRebirth()
    {
        //특정 체력값을 기준으로 부활시킨다. 먼저 체력이 회복되어야 사망 이벤트가 발생되지 않으니 체력먼저 복구하도록 조정
        //상호작용 재시작
        Rebirth(this);
    }
    public event UnitFloatEvent MoveEvent;
    public void OnMove(float moveDistance)
    {
        MoveEvent(this, moveDistance);
    }

    public event UnitEvent EntangleEvent;
    public void OnEntangle()
    {
        EntangleEvent(this);
    }
    public event UnitEvent EntangleEndEvent;
    public void OnEntangleEnd()
    {
        EntangleEndEvent(this);
    }
    public event UnitEvent GroggyEvent;
    public void OnGroggy()
    {
        GroggyEvent(this);
    }
    public event UnitEvent GoggyEndEvent;
    public void OnGrggyEnd()
    {
        GoggyEndEvent(this);
    }

    public event UnitBuffEvent GetBuffEvent;
    public void OnBuff(Buff buff)
    {
        GetBuffEvent(this, buff);
    }
    public event UnitBuffEvent GetDebuffEvent;
    public void OnDebuff(Buff buff)
    {
        GetDebuffEvent(this, buff);
    }

    public event UnitEvent CastingEvent;
    public void OnCast()
    {
        CastingEvent(this);
    }

    public event UnitEvent AttackEvent;
    public void OnAttack()
    {
        AttackEvent(this);
    }
    public event UnitUnitEvent HitEvent;
    public void OnHit(TestUnit unit)
    {
        HitEvent(this, unit);
    }
    public event UnitEvent HitFailEvent;
    public void OnMiss()
    {
        HitFailEvent(this);
    }
    public event UnitDamageEvent DamageEvent;
    public void OnDamage(Damage damage)
    {
        DamageEvent(this, damage);
    }

    public event UnitProjectileEvent ProjectileCollisionEvent;
    public void OnCollisionProjectile(Projectile projectile)
    {
        ProjectileCollisionEvent(this, projectile);
    }

    public event UnitDamageEvent GetHitEvent;
    public void OnHitted(Effect effect)
    {
        //피격시 나타나는 효과들 재정리 할것.
        //이외 회피효과들도.
    }
    public event UnitDamageEvent GetDamageEvent;
    public void OnDamaged(Damage damage)
    {
        GetDamageEvent(this, damage);
    }

    public event UnitEffectEvent EvadeEvent;
    public void OnEvade(Effect effect)
    {
        EvadeEvent(this, effect);
    }
    public event UnitEffectEvent EvadeFailEvent;
    public void OnEvadeFail(Effect effect)
    {
        EvadeFailEvent(this, effect);
    }

    public event UnitHealEvent HealedEvent;
    public void OnHealedEvent(Heal heal)
    {
        HealedEvent(this, heal);
    }
    public event UnitHealEvent HealEvent;
    public void OnHealEvent(Heal heal)
    {
        HealedEvent(this, heal);
    }

    public event UnitBlockEvent UseAnyBlockEvent;
    public void OnUseAnyBlock(Block block)
    {
        UseAnyBlockEvent(this, block);
    }
    public event UnitBlockEvent UseNormalBlockEvent;
    public void OnUseNormalBlock(Block block)
    {
        UseNormalBlockEvent(this, block);
    }
    public event UnitBlockEvent UseSpecialBlockEvent;
    public void OnUseSpecialBlock(Block block)
    {
        UseSpecialBlockEvent(this, block);
    }

    public event UnitBlockEvent Use1BlockEvent;
    public void OnUse1Block(Block block)
    {
        Use1BlockEvent(this, block);
    }
    public event UnitBlockEvent Use2BlockEvent;
    public void OnUse2Block(Block block)
    {
        Use2BlockEvent(this, block);
    }
    public event UnitBlockEvent Use3BlockEvent;
    public void OnUse3Block(Block block)
    {
        Use3BlockEvent(this, block);
    }

    public event UnitBlockEvent ReactAnyBlockEvent;
    public void OnReactAnyBLock(Block block)
    {
        ReactAnyBlockEvent(this, block);
    }
    public event UnitBlockEvent React1BlockEvent;
    public void OnReact1BLock(Block block)
    {
        React1BlockEvent(this, block);
    }
    public event UnitBlockEvent React2BlockEvent;
    public void OnReact2BLock(Block block)
    {
        React2BlockEvent(this, block);
    }
    public event UnitBlockEvent React3BlockEvent;
    public void OnReact3BLock(Block block)
    {
        React3BlockEvent(this, block);
    }
}
public partial class TestUnit : MonoBehaviour
{
    public void GetDamage()
    {

    }
    public void GetHit()
    {

    }
    public void GetHeal()
    {

    }
    public void GetBuff()
    {

    }
    public void CastSkill(string SkillName)
    {

    }
    public void Pull()
    {

    }
    public void Push()
    {

    }



    public void GetNormalDamage(ref float damage, E_EffectType damageType, float penetrationPower)
    {
        float resultDamage = damage;
        float damageReducePercent = 0;
        E_FloatingType floatingType = E_FloatingType.NonpenetratingDamage;

        if (penetrationPower >= StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue)
        {
            damageReducePercent = 1;//1이면 풀관통
            floatingType = E_FloatingType.FullPenetrationDamage;
        }
        else
        {
            damageReducePercent = (100 / (((StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue - penetrationPower) * 0.348f) + 100));
            if (damageReducePercent > 0.85f)
            {
                floatingType = E_FloatingType.FullPenetrationDamage;
            }
        }
        //if (onhitevent != null)
        //{
        //    onhitevent.invoke(ref resultdamage);
        //}

        resultDamage *= (damageReducePercent * (1 - StatManager.CreateOrGetStat(E_StatType.DamageReduceRate).ModifiedValue));
        StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue -= resultDamage;
        FloatingNumberManager.FloatingNumber(gameObject, resultDamage, floatingType);
    }
    public void GetCriticalDamage(ref float damage, E_EffectType damageType, float penetrationPower)
    {
        float resultDamage = damage;
        float damageReducePercent = 0;
        if (penetrationPower >= StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue)
        {
            damageReducePercent = 1;//1이면 풀관통
        }
        else
        {
            damageReducePercent = (100 / (((StatManager.CreateOrGetStat((E_StatType)damageType).ModifiedValue - penetrationPower) * 0.348f) + 100));
        }
        resultDamage *= damageReducePercent;
        resultDamage *= (1 - StatManager.CreateOrGetStat(E_StatType.DamageReduceRate).ModifiedValue);
        StatManager.CreateOrGetStat(E_StatType.CurrentHealth).ModifiedValue -= resultDamage;
        FloatingNumberManager.FloatingNumber(gameObject, resultDamage, E_FloatingType.CriticalDamage);
    }
    private void Start()
    {
        SetDirection();
        StartCoroutine(AutoMoving());
    }
}