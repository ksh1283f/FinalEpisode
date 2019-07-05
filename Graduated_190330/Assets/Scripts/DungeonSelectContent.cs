using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectContent : MonoBehaviour
{
    [SerializeField] Text simpleInfo;   // 단수 표시 등 간단한 정보
    [SerializeField] Image ClearMarker; // 클리어 정보 표시 아이콘
    Button btnShowDetail;  // 상세정보 표시를 위한 버튼

    public Action<string> OnClickedShowDetail { get; set; }
    public string detailInfo { get; private set; }  // 보상, 몬스터 체력, 데미지 등의 상세한 정보
    void Start()
    {
        if (btnShowDetail == null)
        {
            btnShowDetail = GetComponent<Button>();
            return;
        }

        btnShowDetail.onClick.AddListener(() => { OnClickedShowDetail.Execute(detailInfo); });
    }

    public void SetDataInfo(string simple, string detail, bool isClear)
    {
        if (simpleInfo == null)
        {
            Debug.LogError("simple info is null");
            return;
        }

        simpleInfo.text = simple;
        detailInfo = detail;

        ClearMarker.gameObject.SetActive(isClear);
    }
}
