using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System.Text;

public enum E_MarketContents
{
    None,
    Purchase,
    Sell,
    Etc,
}

public class MarketUI : uiSingletone<MarketUI>
{
    enum E_TabType
    {
        None,
        Sell,
        Purchase,
    }

    [SerializeField] Button btnPurchase;
    [SerializeField] Button btnSell;
    [SerializeField] Button btnBack;
    [SerializeField] Button btnShowNews;
    [SerializeField] Text textInfo;
    public List<CharacterSimpleInfo> simpleInfoList; // 임시
    public E_MarketContents selectedMarketContents;

    [SerializeField] Text titleText;
    [SerializeField] NewsWindow newsWindow;
    [SerializeField] Toggle sellToggle;
    [SerializeField] Toggle purchaseToggle;

    UnitData selectedUnitInSimpleList;
    E_TabType selectedTabType = E_TabType.None;


    protected override void Awake()
    {
        uiType = E_UIType.UnitMarket;
        base.Awake();

        simpleInfoList = GetComponentsInChildren<CharacterSimpleInfo>().ToList();
        selectedMarketContents = E_MarketContents.None;

        //todo 버튼 기능 연결
        string result = ConectBtnWithMethod();
        if (!string.IsNullOrEmpty(result))
            Debug.LogError(result);
    }

    public override void Show(string[] dataList)
    {
        base.Show();
        if (dataList.Length != 1)
        {
            Debug.LogError("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        if (titleText != null)
            titleText.text = dataList[0];

        if(sellToggle != null)
            sellToggle.isOn = true; // 콜백으로 등록된 함수를 호출하여 리스트 갱신
        //SetSimpleInfoList();
    }

    void SetSimpleInfoList()
    {
        List<UnitData> listToShow = GetUnitDatas();

        // init
        for (int i = 0; i < simpleInfoList.Count; i++)
            simpleInfoList[i].gameObject.SetActive(false);

        for (int i = 0; i < listToShow.Count; i++)
        {
            simpleInfoList[i].gameObject.SetActive(true);
            simpleInfoList[i].SetData(listToShow[i]);
            simpleInfoList[i].OnClickedContent = OnClickedSimpleInfo;
        }
    }

    List<UnitData> GetUnitDatas()
    {
        // 선택된 탭이 무엇인지 체크해야한다.
        // todo 갱신을 하는것이 유효한지 검사(MarketManager에서..)

        switch (selectedMarketContents)
        {
            case E_MarketContents.Purchase:
                break;

            case E_MarketContents.Sell:
                break;
        }

        return null;    // 임시
    }

    void OnClickedBtnBack()
    {
        selectedMarketContents = E_MarketContents.None;

        Close();
    }

    void OnClickedSimpleInfo(UnitData data, bool isSelected)
    {
        if (data == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        // 다른 선택된 리스트 선택 해제하기
        for (int i = 0; i < simpleInfoList.Count; i++)
        {
            if (data.Id == simpleInfoList[i].unitData.Id)
                continue;

            simpleInfoList[i].IsSelected = false;
        }

        if (!isSelected)
        {
            textInfo.text = string.Empty;
            selectedUnitInSimpleList = null;
            return;
        }

        selectedUnitInSimpleList = data;
        textInfo.text = GetDataInfo(selectedUnitInSimpleList);
    }

    string GetDataInfo(UnitData data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Lv. ");
        sb.Append(data.Level);
        sb.AppendLine();
        sb.Append("Atk: ");
        sb.Append(data.Atk);
        sb.AppendLine();
        sb.Append("Def: ");
        sb.Append(data.Def);
        sb.AppendLine();
        sb.Append("Cri: ");
        sb.Append(data.Cri);
        sb.AppendLine();
        sb.Append("Spd: ");
        sb.Append(data.Spd);
        sb.AppendLine();
        sb.Append("Price: ");
        sb.Append(data.Price);
        sb.AppendLine();
        sb.AppendLine();
        sb.Append(data.Description);

        return sb.ToString();
    }

    void OnChangedToggle(bool isOn)
    {
        if (!isOn)
            return;

        // selectedTabType = 

        // todo 새로운 리스트 갱신, 버튼 갱신

    }

    string ConectBtnWithMethod()
    {
        string resultMessage = string.Empty;
        if (sellToggle == null)
        {
            resultMessage = "sellToggle is null";
            return resultMessage;
        }

        if (purchaseToggle == null)
        {
            resultMessage = "purchaseToggle is null";
            return resultMessage;
        }

        // todo other btns
        sellToggle.onValueChanged.AddListener(OnChangedToggle);
        purchaseToggle.onValueChanged.AddListener(OnChangedToggle);

        return resultMessage;
    }
}
