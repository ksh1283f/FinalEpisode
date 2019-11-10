using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_SkillEffectType
{
    None,
    UserGenerateResourceEffect,
    UserUseAttackSkillEffect,
    UserUseUtilSkillEffect,
    UserUseDefendSkillEffect,
    UserUseComplexSkillEffect,
    UserPropertySkillEffect,
    EnemySkillEffect,
}

public class EffectManager : Singletone<EffectManager>
{
//     기 모으기
// : venomSpell, YellowFairyDust
// — 쓰기: explosion5
// — 힐 : bufferfly(heal)//
// — 뎀감 : magical//
// — 유틸 : (ㄷㅂ)whitesmoke//
    [SerializeField] Transform effectPivotTrans;
    private const string generateResourceEffect = "Effects/VenomSpell";
    private const string useAttackBasicSkillEffect = "Effects/Explosion5";
    private const string useUtilBasicSkillEffect = "Effects/YellowFairyDust";
    private const string useDefendBasicSkillEffect = "Effects/MagicalFountain";
    private const string useComplexBasicSkillEffect = "Effects/Inferno";

    public void StartEffect(E_SkillEffectType type, Vector3 pos)
    {
        switch (type)
        {
            case E_SkillEffectType.UserGenerateResourceEffect:
                SoundManager.Instance.PlaySynthEffectSound();
                StartEffect(generateResourceEffect,pos);
                break;

            case E_SkillEffectType.UserUseAttackSkillEffect:
                StartEffect(useAttackBasicSkillEffect,pos);
                SoundManager.Instance.PlayExplosionSound();
                break;

            case E_SkillEffectType.UserUseUtilSkillEffect:
                StartEffect(useUtilBasicSkillEffect, pos);
                SoundManager.Instance.PlayExplosionSound();
                break;

            case E_SkillEffectType.UserUseDefendSkillEffect:
                StartEffect(useDefendBasicSkillEffect, pos);
                SoundManager.Instance.PlayExplosionSound();
                break;

            case E_SkillEffectType.UserUseComplexSkillEffect:
                StartEffect(useComplexBasicSkillEffect, pos);
                SoundManager.Instance.PlayExplosionSound();
                break;
        }
    }

    public void StartEffect(string path, Vector3 pos)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>(path)) as GameObject;

        effect.transform.position = pos;
        effect.SetActive(true);
    }

    public void StartEffect(string path, Vector3 pos, bool isSound)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>(path)) as GameObject;

        effect.transform.position = pos;
        effect.SetActive(true);
    }
}
