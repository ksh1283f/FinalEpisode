using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectContent : MonoBehaviour
{
    [SerializeField] Text simpleInfo;   // 단수 표시 등 간단한 정보
    [SerializeField] Image ClearMarker; // 클리어 정보 표시 아이콘
    Button btnShowDetail;  // 상세정보 표시를 위한 버튼

    public Action<string, DungeonMonsterData> OnClickedShowDetail { get; set; }
    public string detailInfo { get; private set; }  // 보상, 몬스터 체력, 데미지 등의 상세한 정보
    public RewardData rewardData { get; private set; }
    public DungeonMonsterData dungeonMonsterData { get; private set; }

    void Start()
    {
        if (btnShowDetail == null)
        {
            btnShowDetail = GetComponent<Button>();
            if(btnShowDetail == null)
                btnShowDetail.gameObject.AddComponent<Button>();
        }


        btnShowDetail.onClick.AddListener(() => { OnClickedShowDetail.Execute(detailInfo,dungeonMonsterData); });
        
    }

    public void SetSimpleDataInfo(string simple, bool isClear)
    {
        if (simpleInfo == null)
        {
            Debug.LogError("simple info is null");
            return;
        }

        simpleInfo.text = simple;

        ClearMarker.gameObject.SetActive(isClear);
    }

    public void SetDungeonMonsterData(DungeonMonsterData data)
    {
        dungeonMonsterData = data;
    }

    // 실제 데미지 증가보정은 다른 곳에서
    public void SetDetailInfo(DungeonPattern dungeonPatternData, RewardData rewardData)
    {
        // 단수
        // 체력(증가치)
        // 공격력(증가치)
        // 보상(골드, 경험치)
        // 용병경험치는 그대로, 캐릭터 경험치는 절반
        if (rewardData == null)
        {
            Debug.LogError("rewardData is null");
            return;
        }

        if (dungeonPatternData == null)
        {
            Debug.LogError("dungeonPatternData is null");
            return;
        }

        int step = rewardData.DungeonId + 1;
        int enemyHP = dungeonPatternData.EnemyHealth;
        int enemyAtkPer = step * 5; // 공격력 증가치

        StringBuilder sb = new StringBuilder();
        sb.Append(step);
        sb.Append("단계 던전");
        sb.AppendLine();
        sb.AppendLine();
        sb.Append("<몬스터 정보>");
        sb.AppendLine();
        sb.Append("체력: ");
        sb.Append(dungeonPatternData.EnemyHealth);
        sb.AppendLine();
        sb.Append("공격력: +");
        sb.Append(GameDataManager.Instance.EnemyStatCorrectionDataDic[step-1].AtkCorrection);
        sb.Append("%");
        sb.AppendLine();
        sb.AppendLine();

        sb.Append("<보상 정보>");
        sb.AppendLine();
        sb.Append("exp: ");
        sb.Append(rewardData.Exp);
        sb.AppendLine();
        sb.Append("gold: ");
        sb.Append(rewardData.Gold);
        sb.AppendLine();
        sb.AppendLine();
        sb.Append(rewardData.Description);

        detailInfo = sb.ToString();
    }
}
