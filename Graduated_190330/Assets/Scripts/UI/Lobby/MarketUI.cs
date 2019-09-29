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
    public E_MarketContents selectedMarketContents
    {
        get
        {
            switch (SelectedTabType)
            {
                case E_TabType.Sell:
                    return E_MarketContents.Sell;

                case E_TabType.Purchase:
                    return E_MarketContents.Purchase;
            }

            return E_MarketContents.None;
        }
        set
        {
            switch (value)
            {

                case E_MarketContents.Purchase:
                    selectedTabType = E_TabType.Purchase;
                    break;

                case E_MarketContents.Sell:
                    selectedTabType = E_TabType.Sell;
                    break;
            }
        }
    }

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

    void Start()
    {
        Close();
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

        selectedMarketContents = E_MarketContents.Sell;
        newsWindow.ShowWindow(false);
        textInfo.text = string.Empty;

        if(!UserManager.Instance.UserInfo.TutorialClearList[(int)E_SimpleTutorialType.IntroduceMarket])
        {
            //show
            TutorialSimpleUI tutorialUI = UIManager.Instance.LoadUI(E_UIType.TutorialSimpleLobby) as TutorialSimpleUI;
            tutorialUI.Show(new string[]{"용병시장 소개"});
            tutorialUI.SetTutorialType(E_SimpleTutorialType.IntroduceMarket);
            // settype
        }
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
            if (simpleInfoList[i].unitData == null)
                continue;

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
            return;
        }

        // 출전 중인 용병인지
        if (UserManager.Instance.UserInfo.SelectedUnitDic.ContainsKey(selectedUnitInSimpleList.Id))
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "출전 중인 용병은 판매할 수 없습니다." });
            return;
        }

        // todo 판매 프로세스
        // 1. 유닛의 가격만큼 골드량 상승
        int price = selectedUnitInSimpleList.Price;
        int finalGoldValue = UserManager.Instance.UserInfo.Gold + price;
        // 1-1. todo 이벤트 유무 체크하여 상승 골드량 보정
        UserManager.Instance.SetUserGold(finalGoldValue);

        // 2. 리스트에서 유닛 제거
        UserManager.Instance.RemoveUnitInList(selectedUnitInSimpleList.Id);

        // 3. ui에 변경된 리스트 사항 갱신
        SetSimpleInfoList();
        selectedUnitInSimpleList = null;

        MessageUI messageUI = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
        messageUI.Show(new string[] { "유저 메세지", "판매 완료" });

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

        // switch (SelectedTabType)
        // {
        //     case E_TabType.Purchase:
        //         // todo 구매가능한 용병들을 산출하여 보여주기
        //         break;

        //     case E_TabType.Sell:
        //         SetSimpleInfoListWithMyUnits();
        //         break;
        // }
        SetSimpleInfoList();

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
