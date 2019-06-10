using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchMissileEffect : Effect
{
    /*
     * 미사일 발사각도
     * 발사 속도
     * 
     * 발사 위치
     * 발사 위치에 대한 추가 오프셋
     * 발사 위치 X값 월드 좌표 고정여부
     * 발사 위치 Y값 월드 좌표 고정여부
     * 
     * 발사체
     * 발사 횟수
     * 발사 주기
     * 
     * 조건 확인
     */

    public override Unit Caster
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
    public override Unit Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }







    public Vector3 launchAngle;
    public Vector3 launchVelocity;
    public GameObject launchPosition;
    public Vector3 offset;


    public Projectile projectile;
    public int launchCount = 0;
    public float launchPeriod = 0;

    public bool fixedX;
    public bool fixedY;

    public override void RefreshAllAmount(Unit caster, Unit target)
    {

    }
    public override void RefreshFixedAllAmount()
    {

    }
    public override void RefreshCasterBasedAmount(Unit caster)
    {

    }
    public override void RefreshTargetBasedAmount(Unit target)
    {

    }

    public override void ActivateEffect()
    {
        StartCoroutine(Launch());
    }
    public override void ActivateEffect(Unit caster)
    {
        this.caster = caster;
        StartCoroutine(Launch());
    }
    public override void ActivateEffect(Unit caster, Unit target)
    {
        this.caster = caster;
        StartCoroutine(Launch());
    }
    public override void ActivateEffect(Unit caster, Unit target, float multiplier)
    {
        this.caster = caster;
        StartCoroutine(Launch());
    }
    public override void ActivateEffect(Unit caster, Unit target, ref float amount, float multiplier)
    {
        this.caster = caster;
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        int currentCount = 0;
        float currentPeriod = float.MaxValue;
        while (true)
        {
            if (currentCount < launchCount)
            {
                if (currentPeriod < launchPeriod)
                {
                    currentPeriod += Time.deltaTime;
                }
                else
                {
                    LaunchMissile();
                    currentPeriod = 0;
                    currentCount++;
                }
            }
            yield return null;
        }
    }

    void LaunchMissile()
    {
        if (projectile == null)
        {
            Debug.LogError("projectile = null");
            return;
        }

        Vector3 launchTempPosition = new Vector3();
        Projectile instanceProjectile = Instantiate(projectile);
        if (instanceProjectile == null)
        {
            Debug.LogError("instanceProjectile = null");
            return;
        }

        E_EffectType effectType = instanceProjectile.SkillEffectType;
        launchTempPosition = launchPosition.transform.position + offset;

        switch (effectType)
        {
            case E_EffectType.Heals:
                instanceProjectile.gameObject.layer = LayerMask.NameToLayer("Default");
                break;

            default:
                switch (caster.groupTag)
                {
                    case E_GroupTag.Player:
                        instanceProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerMissile");
                        break;
                    case E_GroupTag.Enemy:
                        instanceProjectile.gameObject.layer = LayerMask.NameToLayer("EnemyMissile");
                        break;
                }
                break;
        }

        if (fixedX)
        {
            launchTempPosition.x = offset.x;
        }
        if (fixedY)
        {
            launchTempPosition.y = offset.y;
        }
        instanceProjectile.transform.position = launchTempPosition;
        if (instanceProjectile.GetComponent<Rigidbody2D>())
        {
            instanceProjectile.GetComponent<Rigidbody2D>().velocity = launchVelocity;
        }
        instanceProjectile.Initialize(caster);
        instanceProjectile.OnCreat();
        instanceProjectile.FlyingStart();
    }

    public override bool ConditionCheck()
    {/*
        for(int index=0; index < validatorList.Count; index++)
        {
            if(!validatorList[index].Check(caster, target))
            {
                return false;
            }
        }
        return true;*/
        return true;
    }
}
