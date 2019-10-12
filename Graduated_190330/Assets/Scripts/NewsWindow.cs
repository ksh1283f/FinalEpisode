using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsWindow : MonoBehaviour
{
    [SerializeField] Button btnClose;
    [SerializeField] Text newsInfoText;

    [SerializeField] GameObject child;
    [SerializeField] Image background;
    [SerializeField] Image westNoticeIcon;
    [SerializeField] Image eastNoticeIcon;
    [SerializeField] Image MidNoticeIcon;
    [SerializeField] Image SouthNoticeIcon;

    void Start()
    {
        if (btnClose == null)
        {
            Debug.LogError("NewsWindow's btnClose is null");
            return;
        }

        btnClose.onClick.AddListener(OnClickedBtnClose); 
    }

    void OnClickedBtnClose()
    {
        ShowWindow(false);
    }

    // GameDataManager에서 받아오기
    public void SetNewsInfo(string data)
    {
        if (newsInfoText == null)
        {
            Debug.LogError("newsInfoText is null");
            return;
        }

        newsInfoText.text = data;
    }

    public void ShowWindow(bool isShow)
    {
        child.SetActive(isShow);
        background.enabled = isShow;

        if(!isShow)
            return;

        // todo 데이터 갱신 작업
        WorldEventData data = WorldEventManager.Instance.GetPresentEventData();

        // 활성화 시킬 notice Icon 작업
        westNoticeIcon.gameObject.SetActive(false);
        eastNoticeIcon.gameObject.SetActive(false);
        MidNoticeIcon.gameObject.SetActive(false);
        SouthNoticeIcon.gameObject.SetActive(false);
        if(data == null)
        {
            newsInfoText.text = string.Empty;
            return;
        }

        switch (data.IconType)
        {
            case E_WorldEventNoticeIconType.West:
                westNoticeIcon.gameObject.SetActive(true);
                break;

            case E_WorldEventNoticeIconType.East:
                eastNoticeIcon.gameObject.SetActive(true);
                break;

            case E_WorldEventNoticeIconType.Mid:
                MidNoticeIcon.gameObject.SetActive(true);
                break;

            case E_WorldEventNoticeIconType.South:
                SouthNoticeIcon.gameObject.SetActive(true);
                break;
        }

        newsInfoText.text = data.EventDescription;
    }
}
