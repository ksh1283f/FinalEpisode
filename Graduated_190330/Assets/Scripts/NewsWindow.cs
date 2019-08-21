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
    }
}
