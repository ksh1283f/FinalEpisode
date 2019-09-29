using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : uiSingletone<LobbyUI>
{
    private const float MIN_CAM_POS = 160f;
    private const float MAX_CAM_POS = 440f;
    private const float INTERVAL_VALUE = 280f;

    [SerializeField] Text levelText;
    [SerializeField] Text goldText;
    [SerializeField] Text userNameText;

    [SerializeField] Slider expBar;
    [SerializeField] Text textExp;

    [SerializeField] Text commonPropertyText;
    [SerializeField] Text characterManageText;
    [SerializeField] Scrollbar camScrollBar;
    Camera mainCam;

    protected override void Awake()
    {
        uiType = E_UIType.Lobby;
        base.Awake();
        mainCam = Camera.main;
    }

    private void Start()
    {
        UserManager.Instance.OnCreateUserInfoData += SetUserInfoWithData;
        UserManager.Instance.OnUpdatedUserInfo += SetUserInfoWithData;
        camScrollBar.onValueChanged.AddListener(OnMovedScrollBar);

    }

    public override void Show()
    {
        base.Show();
        SetUserInfoWithData(UserManager.Instance.UserInfo);

        //commonPropertyText.transform.position = Camera.main.WorldToScreenPoint()

        camScrollBar.value = 0.5f;
        if (mainCam != null)
            mainCam.transform.position = new Vector3(ConvertCameraPosToScrollValue(false), mainCam.transform.position.y, mainCam.transform.position.z);
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

    float ConvertCameraPosToScrollValue(bool isFromCam)
    {
        // ex
        // <cam to scroll>
        // 300(present cam pos)

        // -> 300-160 = 140
        // -> 140 / 280 = 0.5

        // <scroll to cam>
        // 0.4(now scroll value)

        // -> 0.4 = x / 280
        // x= 280*0.4 + 160

        if (mainCam == null)
        {
            Debug.LogError("MainCam is null");
            return float.MaxValue;
        }

        if (camScrollBar == null)
        {
            Debug.LogError("Cam scrollBar is null");
            return float.MaxValue;
        }

        float retValue = float.MinValue;
        if (isFromCam)
        {
            // cam to scroll
            Vector3 mainCamPos = mainCam.transform.position;
            retValue = mainCamPos.x - MIN_CAM_POS;
            retValue /= INTERVAL_VALUE;
        }
        else
        {
            // scroll to cam
            float camScrollValue = camScrollBar.value;
            retValue = INTERVAL_VALUE * camScrollValue + MIN_CAM_POS;
        }

        return retValue;
    }

    public void OnMovedScrollBar(float changedVal)
    {
        float val = ConvertCameraPosToScrollValue(false);
        mainCam.transform.position = new Vector3(val, mainCam.transform.position.y, mainCam.transform.position.z);
    }
}