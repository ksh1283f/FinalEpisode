using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloatObject
{
    void Release();
    void SetData(string data);
    void Directing();
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
    private const string DamageFloatObjectPath = "Prefabs/DamageFloat/DamageFloatObject";
    private const string SuccessSpritePath = "EtcIcon/Aura78";
    private const string FailSpritePath = "EtcIcon/Aura118";
    [SerializeField] Canvas floatingCanvas;

    DamageFloatObject damageFloat;
    PatternResultObject patternResult;

    void Start()
    {
        damageFloat = Resources.Load<DamageFloatObject>(DamageFloatObjectPath);
        patternResult = Resources.Load<PatternResultObject>(PatternResultObjectPath);
        if (damageFloat == null)
        {
            Debug.LogError("DamageFloat is null");
            return;
        }

        if (patternResult == null)
        {
            Debug.LogError("PatternResult is null");
            return;
        }
    }

    public void ShowDamageFont(GameObject target, float damageValue, E_DamageType damageType)
    {
        // 예외처리하고
        if (target == null)
        {
            Debug.LogError("target gameObject is null");
            return;
        }

        if (damageValue <= 0)
        {
            Debug.LogError("damage value is invalid");
            return;
        }

        DamageFloatObject dfObj = Instantiate<DamageFloatObject>(damageFloat);
        dfObj.transform.SetParent(floatingCanvas.transform);
        dfObj.SetDamageType(damageType);
        dfObj.SetData(damageValue.ToString());

        // 타겟을 기반으로 하여 생성 좌표 결정
        dfObj.transform.position = target.transform.position + new Vector3(Random.value - 0.5f, 0.5f + Random.value, -1f);
        Debug.LogError("dfObj.transform.position: " + dfObj.transform.position);
        // damageValue를 이용하여 표시값 설정
        // 애니메이션 재생?
        dfObj.Directing();
    }

    public void ShowPatternResult(GameObject target, bool isSuccess)
    {
        // 예외처리하고
        if (targetPlayer == null)
        {
            Debug.LogError("target is null");
            return;
        }

        PatternResultObject prObj = Instantiate<PatternResultObject>(patternResult);
        prObj.SetData(isSuccess ? SuccessSpritePath : FailSpritePath);

        // 타겟을 기반으로 하여 생성 좌표 결정
        prObj.transform.position = target.transform.position + 3 * Vector3.up;
        prObj.Directing();
    }

    #region TestCode
    [SerializeField] GameObject targetPlayer;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (targetPlayer == null)
            {
                Debug.LogError("targetPlayer is null");
                return;
            }

            ShowDamageFont(targetPlayer, 200, E_DamageType.Normal);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (targetPlayer == null)
            {
                Debug.LogError("targetPlayer is null");
                return;
            }

            ShowPatternResult(targetPlayer, true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (targetPlayer == null)
            {
                Debug.LogError("targetPlayer is null");
                return;
            }
            ShowPatternResult(targetPlayer, false);
        }
    }
    #endregion

}
