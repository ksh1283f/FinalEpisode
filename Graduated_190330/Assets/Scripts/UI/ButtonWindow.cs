using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWindow : MonoBehaviour
{
    private const string COOLDOWN_TEXT = "CooldownText";
    private const string COOLDOWN_IMAGE = "CooldownImage";

    /* 인스펙터 변수 */
    [SerializeField] Button btnAttackResource;
    [SerializeField] Button btnUtilResource;
    [SerializeField] Button btnDefenseResource;

    [SerializeField] Button btnAttack;
    [SerializeField] Button btnUtil;
    [SerializeField] Button btnComplex;
    [SerializeField] Button btnDefense;

    [SerializeField] Image enemyHpBar;
    [SerializeField] GameObject enemyHpBarObj;
    [SerializeField] Text enemyName;

    [SerializeField] Button btnFirstProperty;
    [SerializeField] Button btnSecondProperty;
    /**************************** */

    List<Image> cooldownImageList;
    List<Text> cooldownTextList;

    // 버튼 클릭 관련 콜백
    // - 생성관련
    public Action<E_SkillResourceType> OnClickedAttackResourceBtn { get; set; }
    public Action<E_SkillResourceType> OnClickedUtilResourceBtn { get; set; }
    public Action<E_SkillResourceType> OnClickedDefenseResourceBtn { get; set; }
    // - 사용관련
    public Action<E_SkillResourceType> OnClickedAttackBtn { get; set; }
    public Action<E_SkillResourceType> OnClickedUtilBtn { get; set; }
    public Action<E_UserSkillType> OnClickedComplexBtn { get; set; }
    public Action<E_SkillResourceType> OnClickedDefenseBtn { get; set; }

    public Action OnClickBtnFirstProperty { get; set; }
    public Action OnClickBtnSecondProperty { get; set; }

    void Start()
    {
        cooldownImageList = new List<Image>();
        cooldownImageList.Add(GetCooldownImage(btnAttackResource));
        cooldownImageList.Add(GetCooldownImage(btnUtilResource));
        cooldownImageList.Add(GetCooldownImage(btnDefenseResource));
        cooldownImageList.Add(GetCooldownImage(btnAttack));
        cooldownImageList.Add(GetCooldownImage(btnUtil));
        cooldownImageList.Add(GetCooldownImage(btnComplex));
        cooldownImageList.Add(GetCooldownImage(btnDefense));

        cooldownTextList = new List<Text>();
        cooldownTextList.Add(GetCooldownText(btnAttackResource));
        cooldownTextList.Add(GetCooldownText(btnUtilResource));
        cooldownTextList.Add(GetCooldownText(btnDefenseResource));
        cooldownTextList.Add(GetCooldownText(btnAttack));
        cooldownTextList.Add(GetCooldownText(btnUtil));
        cooldownTextList.Add(GetCooldownText(btnComplex));
        cooldownTextList.Add(GetCooldownText(btnDefense));

        SetActiveCooldown(!BattleManager.Instance.IsCooldownComplite);
    }

    public void InitCallBacks()
    {
        if (btnAttackResource == null)
            return;

        if (btnUtilResource == null)
            return;

        if (btnDefenseResource == null)
            return;

        if (btnAttack == null)
            return;

        if (btnUtil == null)
            return;

        if (btnDefense == null)
            return;

        if(btnFirstProperty == null)
            return;

        if(btnSecondProperty == null)
            return;

        btnAttackResource.onClick.AddListener(() => OnClickedAttackResourceBtn.Execute(E_SkillResourceType.Attack));
        btnUtilResource.onClick.AddListener(() => OnClickedUtilResourceBtn.Execute(E_SkillResourceType.Util));
        btnDefenseResource.onClick.AddListener(() => OnClickedDefenseResourceBtn.Execute(E_SkillResourceType.Defense));
        btnAttack.onClick.AddListener(() => OnClickedAttackBtn.Execute(E_SkillResourceType.Attack));
        btnUtil.onClick.AddListener(() => OnClickedUtilBtn.Execute(E_SkillResourceType.Util));
        btnDefense.onClick.AddListener(() => OnClickedDefenseBtn.Execute(E_SkillResourceType.Defense));
        btnComplex.onClick.AddListener(() => OnClickedComplexBtn.Execute(E_UserSkillType.AttackAndUtil));
        btnFirstProperty.onClick.AddListener(()=>OnClickBtnFirstProperty.Execute());
        btnSecondProperty.onClick.AddListener(()=>OnClickBtnSecondProperty.Execute());
    }

    public void InitEnemyHpBar(string name, float value)
    {
        if (enemyName == null)
            return;

        if (enemyHpBarObj == null)
            return;

        if (enemyHpBar == null)
            return;

        enemyName.text = name;
        enemyHpBar.color = Color.green;
        enemyHpBarObj.transform.localScale = new Vector3(value, 1, 1);
    }

    public void UpdateHpBar(float value)
    {
        if (enemyHpBarObj == null)
            return;

        enemyHpBarObj.transform.localScale = new Vector3(value, 1, 1);
    }

    Image GetCooldownImage(Button btn)
    {
        if (btn == null)
            return null;

        // Image image = btn.GetComponentInChildren<Image>();
        Image image = btn.transform.Find(COOLDOWN_IMAGE).GetComponent<Image>();
        return image;
    }

    Text GetCooldownText(Button btn)
    {
        if (btn == null)
            return null;

        Text text = btn.transform.Find(COOLDOWN_TEXT).GetComponent<Text>();
        return text;
    }

    public void StartPropertyCoolDown(float time, Button btn)
    {
        if(btn == null)
        {
            Debug.LogError("btn is null");
            return;
        }

        StopCoroutine(FirstPropertyCoolDown(time, btn));
        StartCoroutine(FirstPropertyCoolDown(time, btn));
    }

    IEnumerator FirstPropertyCoolDown(float time, Button btn)
    {
        // todo 사용후 버튼 쿨다운 프로세스
        // BattleManager.Instance.IsCooldownComplite = false;
        float inTime = time;
        SetActiveCooldown(true);

        while (time > 0)
        {
            time -= Time.deltaTime;
            for (int i = 0; i < cooldownImageList.Count; i++)
            {
                cooldownImageList[i].fillAmount = inTime / time;
                cooldownTextList[i].text = String.Format("{0:0.#}",inTime);
            }

            yield return new WaitForFixedUpdate();
        }

        SetActiveCooldown(false);
        // BattleManager.Instance.IsCooldownComplite = true;
    }

    // 글쿨
    public void StartCooldown(float time)
    {
        StartCoroutine(CooldownResource(time));
    }
    IEnumerator CooldownResource(float cooltime)
    {
        BattleManager.Instance.IsCooldownComplite = false;
        float time = cooltime;
        SetActiveCooldown(true);

        while (time > 0)
        {
            time -= Time.deltaTime;
            for (int i = 0; i < cooldownImageList.Count; i++)
            {
                cooldownImageList[i].fillAmount = time / cooltime;
                cooldownTextList[i].text = String.Format("{0:0.#}",time);
            }

            yield return new WaitForFixedUpdate();
        }

        SetActiveCooldown(false);
        BattleManager.Instance.IsCooldownComplite = true;
    }

    void SetActiveCooldown(bool IsActive)
    {
        if (cooldownImageList == null)
            return;

        if (cooldownTextList == null)
            return;

        for (int i = 0; i < cooldownImageList.Count; i++)
        {
            cooldownImageList[i].gameObject.SetActive(IsActive);
            cooldownTextList[i].gameObject.SetActive(IsActive);
        }
    }

    public void UpdateUtillHealSkill()
    {
        if(btnFirstProperty == null)
        {
            Debug.LogError("btnfirstProperty is null");
            return;
        }

        if(btnSecondProperty == null)
        {
            Debug.LogError("btnSecondProperty is null");
            return;
        }

        if(CharacterPropertyManager.Instance.SelectedHealingProperty != null
        &&CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.RogueUtilMaserty_Clocking)
        {
            btnFirstProperty.image.sprite = Resources.Load<Sprite>(CharacterPropertyManager.Instance.SelectedUtilProperty.ImagePath);
        }
        else 
        {
            btnFirstProperty.image.sprite =  null;
            btnFirstProperty.interactable = false;
        }

        if(CharacterPropertyManager.Instance.SelectedHealingProperty != null)
        {
            btnSecondProperty.image.sprite = Resources.Load<Sprite>(CharacterPropertyManager.Instance.SelectedHealingProperty.ImagePath);
        }
        else
        {
            btnSecondProperty.image.sprite = null;
            btnSecondProperty.interactable = false;
        }
    }
}