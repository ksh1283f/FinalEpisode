using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceWindow : MonoBehaviour
{
    [SerializeField] SkillBubbleManager attackManager;
    [SerializeField] SkillBubbleManager utilManager;
    [SerializeField] SkillBubbleManager defenseManager;
    [SerializeField] Button btnExit;

    void Start()
    {
        if(btnExit == null)
            return;

        btnExit.onClick.AddListener(OnClickedBtnExit);
    }

    void OnClickedBtnExit()
    {
        // 종료루틴
        BattleManager.Instance.LoadLobbyScene();
    }

    public void CreateBubble(E_SkillResourceType skillResourceType, int createCount)
    {
        switch (skillResourceType)
        {
            case E_SkillResourceType.Attack:
                if (attackManager == null)
                    return;

                attackManager.CreateBubble(createCount);
                break;

            case E_SkillResourceType.Util:
                if (utilManager == null)
                    return;

                utilManager.CreateBubble(createCount);
                break;

            case E_SkillResourceType.Defense:
                if (defenseManager == null)
                    return;

                defenseManager.CreateBubble(createCount);
                break;
        }
    }

    public void UseBubble(E_SkillResourceType skillResourceType, int skillCount)
    {
        switch (skillResourceType)
        {
            case E_SkillResourceType.Attack:
                if (attackManager == null)
                    return;

                attackManager.UseSkillBubble(skillCount);
                break;

            case E_SkillResourceType.Util:
                if (utilManager == null)
                    return;

                utilManager.UseSkillBubble(skillCount);
                break;

            case E_SkillResourceType.Defense:
                if (defenseManager == null)
                    return;

                defenseManager.UseSkillBubble(skillCount);
                break;
        }
    }
}
