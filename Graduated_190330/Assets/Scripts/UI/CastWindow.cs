using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CastWindow : MonoBehaviour
{
    private const string corAttack = "SkillIcon/RogueIcons/Filled/SIR_04";
    private const string corUtil = "SkillIcon/RogueIcons/Filled/SIR_16";
    private const string corComplex = "SkillIcon/RogueIcons/Filled/SIR_14";
    private const string corDefense = "SkillIcon/WarriorIcons/Filled/SIW7";

    [SerializeField] Image castBar;
    [SerializeField] Image icon;
    [SerializeField] Text name;
    [SerializeField] GameObject barObj;
    [SerializeField] GameObject parent;
    [SerializeField] Image corSkillIcon;
    [SerializeField] Text successMessage;

    Dictionary<E_UserSkillType, Sprite> skillImageDic = new Dictionary<E_UserSkillType, Sprite>();

    void Start()
    {
        skillImageDic.Add(E_UserSkillType.Attack, Resources.Load<Sprite>(corAttack));
        skillImageDic.Add(E_UserSkillType.Util, Resources.Load<Sprite>(corUtil));
        skillImageDic.Add(E_UserSkillType.AttackAndUtil, Resources.Load<Sprite>(corComplex));
        skillImageDic.Add(E_UserSkillType.Defense, Resources.Load<Sprite>(corDefense));
        successMessage.text = string.Empty;
    }

    public void OnCastStart(EnemyPattern pattern)
    {
        if (pattern == null)
        {
            Debug.LogError("Pattern is Null");
            return;
        }

        ShowCastWindow(true);
        name.text = pattern.Name;
        switch (pattern.SkillType)
        {
            case E_UserSkillType.Attack:
                castBar.color = Color.red;
                break;

            case E_UserSkillType.Util:
                castBar.color = Color.yellow;
                break;

            case E_UserSkillType.AttackAndUtil:
                castBar.color = Color.magenta;
                break;

            case E_UserSkillType.Defense:
                castBar.color = Color.green;
                break;
        }

        if (corSkillIcon != null)
            corSkillIcon.sprite = skillImageDic[pattern.SkillType];
    }

    public void OnCastProgress(float value)
    {
        if (barObj == null)
        {
            Debug.LogError("barObj is null");
            return;
        }

        barObj.transform.localScale = new Vector3(value, 1, 1);
    }

    public void OnCastEnd()
    {
        parent.SetActive(false);
    }

    public void ShowCastWindow(bool isShow)
    {
        parent.SetActive(isShow);
    }
    public void ShowSuccessMessage(bool isSuccess)
    {
        if (successMessage == null)
            return;

        if (BattleManager.Instance.isBattleEnd)
            return;

        StartCoroutine(ShowMessage(isSuccess));
    }

    IEnumerator ShowMessage(bool isSuccess)
    {
        successMessage.text = isSuccess ? "Success" : "Fail";
        yield return new WaitForSeconds(1f);
        successMessage.text = string.Empty;
    }
}
