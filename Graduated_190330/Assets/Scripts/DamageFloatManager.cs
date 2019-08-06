using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloatObject
{
    void Release();
}
public enum E_DamageType
{
    None,
    Normal,
    Critical,
    Heal,
}

public enum E_PatternResultType
{
    None,
    Success,
    Fail,
}

public class DamageFloatManager : Singletone<DamageFloatManager>
{
    // todo 임시 경로 : 나중에 바꿔야함.
    private const string PatternResultObjectPath = "Prefabs/PatternResultObject/PatternResultObject";
    private const string DamageFloatObjectPath = "Prefabs/PatternResultObject/DamageFloatObject";

    void Start()
    {

    }

    public void ShowDamageFont(GameObject target, float damageValue, E_DamageType damageType)
    {
        // 예외처리하고
        // 타겟을 기반으로 하여 생성 좌표 결정
        // damageValue를 이용하여 표시값 설정
        // 애니메이션 재생
    }

    public void ShowPatternResult(bool isSuccess)
    {
        // 예외처리하고
        // 타겟을 기반으로 하여 생성 좌표 결정
        // damageValue를 이용하여 표시값 설정
        // 애니메이션 재생
    }
}
