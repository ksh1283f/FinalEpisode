using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : uiSingletone<LobbyUI>
{
    [SerializeField] Text levelText;
    [SerializeField] Text goldText;
    [SerializeField] Text userNameText;

    [SerializeField] Slider expBar;
    [SerializeField] Text textExp;

    [SerializeField] Text commonPropertyText;
    [SerializeField] Text characterManageText;



    protected override void Awake()
    {
        uiType = E_UIType.Lobby;
        base.Awake();
    }

    private void Start()
    {
        UserManager.Instance.OnCreateUserInfoData += SetUserInfoWithData;
        UserManager.Instance.OnUpdatedUserInfo += SetUserInfoWithData;
    }

    public override void Show()
    {
        base.Show();
        SetUserInfoWithData(UserManager.Instance.UserInfo);

        //commonPropertyText.transform.position = Camera.main.WorldToScreenPoint()
    }

    public override void Close()
    {
        base.Close();

    }

    void SetUserInfoWithData(UserInfo userInfo)
    {
        if (userInfo == null)
        {
            Debug.LogError("UserInfo is null");
            return;
        }

        if (levelText == null)
            return;

        if (goldText == null)
            return;

        if (expBar == null)
            return;

        if (textExp == null)
            return;

        levelText.text = userInfo.TeamLevel.ToString();
        goldText.text = userInfo.Gold.ToString();
        userNameText.text = userInfo.UserName;

        // exp
        float value = (float)userInfo.Exp / 100f;
        int percentage = (int)(value * 100);
        expBar.value = value;
        textExp.text = string.Concat(percentage.ToString(), "%");
    }

    public void ShowUserInfoUI()
    {

    }

    public void ShowUserCommonPropertyUI()
    {

    }

    public void ShowCharacterManageUI()
    {

    }

    public void ShowToBattleUI()
    {

    }
}