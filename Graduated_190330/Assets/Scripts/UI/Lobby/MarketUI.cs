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
    public enum E_TabType
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
    [SerializeField] ToggleWrapper sellToggle;
    [SerializeField] ToggleWrapper purchaseToggle;
    List<ToggleWrapper> toggleList;
    UnitData selectedUnitInSimpleList;
    E_TabType selectedTabType = E_TabType.None;
    E_TabType SelectedTabType
    {
        get
        {
            if (toggleList == null)
                return E_TabType.None;

            selectedTabType = (E_TabType)toggleList.Find(x => (x.toggle.isOn == true)).ToggleType;

            return selectedTabType;
        }
    }

    protected override void Awake()
    {
        uiType = E_UIType.UnitMarket;
        base.Awake();

        simpleInfoList = GetComponentsInChildren<CharacterSimpleInfo>().ToList();
        selectedMarketContents = E_MarketContents.None;
        toggleList = new List<ToggleWrapper>();
        toggleList.Add(sellToggle);
        toggleList.Add(purchaseToggle);


        //todo 버튼 기능 연결
        string result = ConnectBtnWithMethod();
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

        if (sellToggle != null)
            sellToggle.toggle.isOn = true; // 콜백으로 등록된 함수를 호출하여 리스트 갱신
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
        List<UnitData> unitList = new List<UnitData>();
        switch (selectedMarketContents)
        {
            case E_MarketContents.Purchase:
                // 데이터를 기반으로 산출된 용병들(어떻게 산출할 것인가)
                break;

            case E_MarketContents.Sell:
                // 보유하고 있는 용병들
                unitList = UserManager.Instance.UserInfo.UnitDic.Values.ToList();
                break;
        }

        return unitList;    // 임시
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

    void OnClickedBtnSell()
    {
        // 선택된 탭이 판매인지
        if (SelectedTabType != E_TabType.Sell)
            return;

        // 선택된 용병이 있는지
        if (selectedUnitInSimpleList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "선택된 용병이 없습니다." });
        }
    }

    void OnClickBtnPurchase()
    {
        // 선택된 탭이 구매인지
        if (SelectedTabType != E_TabType.Purchase)
            return;

        // 선택된 용병이 있는지
        if (selectedUnitInSimpleList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "선택된 용병이 없습니다." });
        }
    }

    void OnClickedBtnShowNews()
    {
        // todo newsWindow popup
        if (newsWindow == null)
        {
            Debug.LogError("news window is null");
            return;
        }

        newsWindow.ShowWindow(true);
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

        switch (SelectedTabType)
        {
            case E_TabType.Purchase:
                break;

            case E_TabType.Sell:
                break;
        }

        // todo 새로운 리스트 갱신, 버튼 갱신

    }

    string ConnectBtnWithMethod()
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

        if (btnBack == null)
        {
            resultMessage = "btnBack is null";
            return resultMessage;
        }

        if (btnShowNews == null)
        {
            resultMessage = "btnShowNews is null";
            return resultMessage;
        }

        if (btnPurchase == null)
        {
            resultMessage = "btnPurchase is null";
            return resultMessage;
        }

        if (btnSell == null)
        {
            resultMessage = "btnSell is null";
            return resultMessage;
        }

        sellToggle.toggle.onValueChanged.AddListener(OnChangedToggle);
        purchaseToggle.toggle.onValueChanged.AddListener(OnChangedToggle);
        btnBack.onClick.AddListener(OnClickedBtnBack);
        btnShowNews.onClick.AddListener(OnClickedBtnShowNews);
        btnPurchase.onClick.AddListener(OnClickBtnPurchase);
        btnSell.onClick.AddListener(OnClickedBtnSell);

        return resultMessage;
    }
}
