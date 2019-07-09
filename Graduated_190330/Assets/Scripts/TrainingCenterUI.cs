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
        SetContentList();
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
        if (characterDetailInfoText != null)
            characterDetailInfoText.text = string.Empty;

        ReleaseContents();
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
        sb.AppendLine();
        sb.Append("<기본 능력치>");
        sb.AppendLine();
        sb.Append("Atk: ");
        sb.Append(unitData.Atk);
        sb.AppendLine();
        sb.Append("Def: ");
        sb.Append(unitData.Def);
        sb.AppendLine();
        sb.Append("Cri: ");
        sb.Append(unitData.Cri);
        sb.AppendLine();
        sb.Append("Spd: ");
        sb.Append(unitData.Spd);
        sb.AppendLine();
        sb.AppendLine();
        sb.Append(unitData.Description);

        characterDetailInfoText.text = sb.ToString();
        ReleaseContents();
    }
    // 구매기능(예외처리 포함: 골드 부족 또는 캐릭터 인벤이 가득 찾는지)
    // 구매 후 수정된 데이터 갱신
    void BuyCharacter()
    {
        // 선택된 캐릭터가 있는지
        for (int i = 0; i < contentList.Count; i++)
        {
            if (contentList[i].IsSelected)
            {
                // if(UserManager.Instance.UserInfo.UnitDic == null)
                // {
                //     UserManager.Instance.UserInfo.UnitDic = new Dictionary<int, UnitData>();
                // }
                    
                // 있다면 리스트가 가득 차진 않았는지
                if (UserManager.Instance.UserInfo.UnitDic.Count >= UserManager.MAX_CHARACTER_COUNT)
                {
                    // todo messageUI같은걸로 표시해주는 작업
                    Debug.LogError("There is no capacity in UnitDic");
                    return;
                }

                // 돈은 제대로 있는지
                if (UserManager.Instance.UserInfo.Gold < contentList[i].UnitData.Price)
                {
                    // todo messageUI같은걸로 표시해주는 작업
                    Debug.LogError("Not enough your gold");
                    return;
                }

                // 모든 조건이 충족되면, 새로운 사용가능한 id부여, 유저인포의 딕셔너리 갱신
                UnitData purchasedUnit = contentList[i].UnitData.ShallowCopy() as UnitData;
                int changedId = GetCreatedUnitID();
                if(changedId == -1)
                    return;
                
                purchasedUnit.UpdateUnitID(changedId);
                UserManager.Instance.SetMyUnitList(purchasedUnit);
                // TODO 골드 차감
                int gold = UserManager.Instance.UserInfo.Gold;
                gold -= purchasedUnit.Price;
                UserInfo userInfo = UserManager.Instance.UserInfo;

                UserManager.Instance.SetUserInfo(userInfo.UserName,userInfo.TeamLevel,userInfo.Exp,gold);
                return;
            }
        }
        // 루프를 빠져나오면 선택된것이 없는 것 -> 함수 종료
        Debug.LogError("There is no selected unit");
    }

    void ReleaseContents()
    {
        for (int i = 0; i < contentList.Count; i++)
            contentList[i].ReleaseSelected();
    }

    int GetCreatedUnitID()
    {
        // 100번부터 시작
        int retVal = -1;
        int index = 0;
        do
        {
            retVal = Random.Range(100, 200);
            Debug.Log("created key value: "+retVal);
            index++;
            if (index >= UserManager.MAX_CHARACTER_COUNT)
            {
                // 할당할 아이디를 모두 다 쓴 경우;;
                Debug.LogError("unit dic is full;");
                return -1;
            }
        } while (UserManager.Instance.UserInfo.UnitDic.ContainsKey(retVal));

        return retVal;
    }
}