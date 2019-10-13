using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : uiSingletone<BattleUI>
{
    [SerializeField] ButtonWindow buttonWindow;
    [SerializeField] CastWindow castWindow;
    [SerializeField] ResourceWindow resourceWindow;
    [SerializeField] StatWindow statWindow;
    [SerializeField] PhaseLogoWindow phaseLogoWindow;
    [SerializeField] BattleEndUI battleEndWindow;
    [SerializeField] Text RemainText;

    protected override void Awake()
    {
        uiType = E_UIType.Battle;
        base.Awake();
    }

    void Start()
    {
        if (buttonWindow == null)
            return;

        if (castWindow == null)
            return;

        if (resourceWindow == null)
            return;

        if (statWindow == null)
            return;

        phaseLogoWindow.OnEndDirecting += OnEndDirecting;
        buttonWindow.InitCallBacks();
        buttonWindow.OnClickedAttackResourceBtn += BattleManager.Instance.GenerateSkillResource;
        buttonWindow.OnClickedUtilResourceBtn += BattleManager.Instance.GenerateSkillResource;
        buttonWindow.OnClickedDefenseResourceBtn += BattleManager.Instance.GenerateSkillResource;
        buttonWindow.OnClickedAttackBtn += BattleManager.Instance.DeleteSkillResource;
        buttonWindow.OnClickedUtilBtn += BattleManager.Instance.DeleteSkillResource;
        buttonWindow.OnClickedComplexBtn += BattleManager.Instance.DeleteSkillResource;
        buttonWindow.OnClickedDefenseBtn += BattleManager.Instance.DeleteSkillResource;
        BattleManager.Instance.OnExecuteMonsterCasting += castWindow.OnCastStart;
        BattleManager.Instance.OnCastingEnd += castWindow.OnCastEnd;
        BattleManager.Instance.OnCastProgress += castWindow.OnCastProgress;
        BattleManager.Instance.OnUpdateUserStat += statWindow.UpdateUnitDataList;
        BattleManager.Instance.OnUpdateEnemyHpBar += buttonWindow.InitEnemyHpBar;
        BattleManager.Instance.OnAttackEnemy += buttonWindow.UpdateHpBar;
        BattleManager.Instance.OnDamagedPlayer += statWindow.UpdateHpBar;
        BattleManager.Instance.OnGameEnd += ShowEndUI;
        BattleManager.Instance.OnStartCooldown += buttonWindow.StartCooldown;
        BattleManager.Instance.OnCorrespondPattern += castWindow.ShowSuccessMessage;
        BattleManager.Instance.OnCalculatedRemainTime += SetTimerText;
        battleEndWindow.OnClickedBtnOk += BattleManager.Instance.LoadLobbyScene;
        BattleManager.Instance.OnUpdatedPlayerSkill += buttonWindow.UpdateUtillHealSkill;
        buttonWindow.OnClickBtnFirstProperty += BattleManager.Instance.ExecuteUtilPropertySkill;
        buttonWindow.OnClickBtnSecondProperty += BattleManager.Instance.ExecuteHealPropertySkill;

        buttonWindow.gameObject.SetActive(false);
        castWindow.ShowCastWindow(false);

        resourceWindow.gameObject.SetActive(false);
        statWindow.gameObject.SetActive(false);
        battleEndWindow.gameObject.SetActive(false);
    }

    public void ShowCastWindow(bool isShow)
    {
        if (castWindow == null)
            return;

        castWindow.gameObject.SetActive(isShow);
    }

    public void ShowBattleLogo()
    {
        if (phaseLogoWindow == null)
            return;

        phaseLogoWindow.ShowDirectingLogo();
    }

    public void ShowBattleUI()
    {
        buttonWindow.gameObject.SetActive(true);
        //castWindow.gameObject.SetActive(true);
        resourceWindow.gameObject.SetActive(true);
        statWindow.gameObject.SetActive(true);
    }

    public void OnEndDirecting()
    {
        BattleManager.Instance.BattlePhase = E_BattlePhase.Battle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillResourceType"></param>
    /// <param name="isGenerate"></param>
    /// <param name="skillCount"></param>
    public void SetSkillResorce(E_SkillResourceType skillResourceType, bool isGenerate, int skillCount = 0)
    {
        if (resourceWindow == null)
            return;

        if (isGenerate)
            resourceWindow.CreateBubble(skillResourceType, skillCount);
        else
            resourceWindow.UseBubble(skillResourceType, skillCount);
    }

    private void SetUnitIcon()
    {

    }

    private void SetTimerText(float time)
    {
        if (RemainText == null)
        {
            Debug.LogError("remainText is null");
            return;
        }

        RemainText.text = time.ToString();
    }

    private void ShowEndUI(bool isClear, RewardData reward)
    {
        battleEndWindow.gameObject.SetActive(true);
        battleEndWindow.ShowResultWindow(isClear, reward);
    }
}