using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System.Text;
using System.Linq;

public class TrainingCenterUI : uiSingletone<TrainingCenterUI>, IBaseUI
{
    [SerializeField] Text titleText;
    [SerializeField] Text characterDetailInfoText;
    [SerializeField] Button btnBuy;
    [SerializeField] Button btnCancel;

    List<ClassTrainContent> contentList;
    protected override void Awake()
    {
        uiType = E_UIType.TrainingCenter;
        base.Awake();

        contentList = GetComponentsInChildren<ClassTrainContent>().ToList();
        if (btnBuy != null)
            btnBuy.onClick.AddListener(BuyCharacter);

        if (btnCancel != null)
            btnCancel.onClick.AddListener(() => { Close(); });

    }

    void Start()
    {

        Close();
    }

    public override void Show(string[] dataList)
    {
        base.Show(dataList);
        if (dataList.Length != 1)
        {
            Debug.LogError("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        if (titleText != null)
            titleText.text = dataList[0];

        // 전체적인 부분 업데이트

    }

    void SetContentList()
    {
        for (int i = 0; i < GameDataManager.Instance.CharacterDataDic.Count; i++)
        {
            contentList[i].SetUnitData(GameDataManager.Instance.CharacterDataDic[i]);
            contentList[i].OnClickedContent += SetDetail;
        }
    }

    // 목록 갱신
    // 목록 클릭 시 설명 갱신
    void SetDetail(UnitData unitData)
    {
        if (unitData == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        if (characterDetailInfoText == null)
        {
            Debug.LogError("charcterDetailInfoText is null");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("Lv: 1");
        sb.AppendLine();
        sb.Append("Atk: ");
        sb.Append(unitData.Atk);
        sb.Append("Def: ");
        sb.Append(unitData.Def);
        sb.AppendLine();
        sb.Append("Cri: ");
        sb.Append(unitData.Cri);
        sb.AppendLine();
        sb.Append("Spd: ");
        sb.Append(unitData.Spd);

        characterDetailInfoText.text = sb.ToString();
    }
    // 구매기능(예외처리 포함: 골드 부족 또는 캐릭터 인벤이 가득 찾는지)
    // 구매 후 수정된 데이터 갱신
    void BuyCharacter()
    {

    }
}