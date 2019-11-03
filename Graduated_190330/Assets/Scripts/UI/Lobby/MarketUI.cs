using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Graduate.GameData.UnitData;
using System.Text;
using System;

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
    [SerializeField] Image unitImage;
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

        // OnShowThisWindow.Execute();

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
        // 판매 구매 분리 작업 필요
        List<UnitData> listToShow = GetUnitDatas();
        if(listToShow == null)
        {
            Debug.LogError("listToShow is null");
            return;
        }

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
                // 데이터를 기반으로 산출된 용병들
                unitList = WorldEventManager.Instance.eventUnitList;
                break;

            case E_MarketContents.Sell:
                // 보유하고 있는 용병들
                unitList = UserManager.Instance.UserInfo.UnitDic.Values.ToList();
                break;
        }

        return unitList;
    }

    void OnClickedBtnBack()
    {
        SoundManager.Instance.PlayButtonSound();
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
        SoundManager.Instance.PlayButtonSound();

        // 다른 선택된 리스트 선택 해제하기
        for (int i = 0; i < simpleInfoList.Count; i++)
        {
            if (simpleInfoList[i].unitData == null)
                continue;
            // todo 판매탭에서만 유효한 부분, 구매 탭에서의 별도 처리 필요
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
        SoundManager.Instance.PlayButtonSound();
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
        // 판매 프리미엄이 붙는 용병인 경우 가격 올려주기
        if (!WorldEventManager.Instance.IsSaleEvent(WorldEventManager.Instance.PresentWorldEventData.WorldEventType))
        {
            WorldEventData presentEventData = WorldEventManager.Instance.PresentWorldEventData;
            float effectValue = 0;
            switch (presentEventData.WorldEventType)
            {
                case E_WorldEventType.AllPremium:
                    effectValue = 1+((float)presentEventData.EventEffectValue/100);
                    price = Convert.ToInt32(price*effectValue);
                    break;

                case E_WorldEventType.WarriorPremium:
                    if(selectedUnitInSimpleList.CharacterType != E_CharacterType.Warrior)
                        break;

                    effectValue = 1+((float)presentEventData.EventEffectValue/100);
                    price = Convert.ToInt32(price*effectValue);
                    break;

                case E_WorldEventType.MagePremium:
                    if(selectedUnitInSimpleList.CharacterType != E_CharacterType.Mage)
                        break;

                    effectValue = 1+((float)presentEventData.EventEffectValue/100);
                    price = Convert.ToInt32(price*effectValue);
                    break;

                case E_WorldEventType.WarlockPremium:
                    if(selectedUnitInSimpleList.CharacterType != E_CharacterType.Warlock)
                        break;

                    effectValue = 1+((float)presentEventData.EventEffectValue/100);
                    price = Convert.ToInt32(price*effectValue);
                    break;

                case E_WorldEventType.RoguePremium:
                    if(selectedUnitInSimpleList.CharacterType != E_CharacterType.Rogue)
                        break;

                    effectValue = 1+((float)presentEventData.EventEffectValue/100);
                    price = Convert.ToInt32(price*effectValue);
                    break;
            }
        }

        int finalGoldValue = UserManager.Instance.UserInfo.Gold + price;
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
        SoundManager.Instance.PlayButtonSound();
        // 선택된 탭이 구매인지
        if (SelectedTabType != E_TabType.Purchase)
            return;

        // 선택된 용병이 있는지
        if (selectedUnitInSimpleList == null)
        {
            MessageUI messageUI = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            messageUI.Show(new string[] { "유저 메세지", "선택된 용병이 없습니다." });
            return;
        }

        // 돈은 제대로 있는지
        if (UserManager.Instance.UserInfo.Gold < selectedUnitInSimpleList.Price)
        {
            MessageUI messageUI = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            messageUI.Show(new string[] { "유저 메세지", "골드가 부족합니다." });
            Debug.LogError("Not enough your gold");
            return;
        }

        // 있다면 리스트가 가득 차진 않았는지
        if (UserManager.Instance.UserInfo.UnitDic.Count >= UserManager.MAX_CHARACTER_COUNT)
        {
            MessageUI messageUI = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            messageUI.Show(new string[] { "유저 메세지", "더이상 용병을 보유할 수 없습니다." });
            Debug.LogError("There is no capacity in UnitDic");
            return;
        }

        // 모든 조건이 충족되면, 새로운 사용가능한 id부여, 유저인포의 딕셔너리 갱신
        UnitData purchasedUnit = selectedUnitInSimpleList.ShallowCopy() as UnitData;
        int changedId = WorldEventManager.Instance.GetCreatedUnitID();
        if(changedId == -1)
            return;
        
        // 판매가격 원가로 변경 -> 되팔때 샀던 가격으로 팔리는 경우를 방지
        int originalPrice = GameDataManager.Instance.CharacterDataDicWithTypeKey[purchasedUnit.CharacterType].Price;
        purchasedUnit.UpdateUnitID(changedId);
        UserManager.Instance.SetMyUnitList(purchasedUnit);
        int gold = UserManager.Instance.UserInfo.Gold;
        gold -= purchasedUnit.Price;
        purchasedUnit.UpdatePrice(originalPrice);
        
        UserInfo userInfo = UserManager.Instance.UserInfo;

        UserManager.Instance.SetUserInfo(userInfo.UserName,userInfo.TeamLevel,userInfo.Exp,gold);
        // 기존 목록에서 제거
        WorldEventManager.Instance.RemoveEventUnit(selectedUnitInSimpleList);
        selectedUnitInSimpleList = null;
        SetSimpleInfoList();

        MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
        message.Show(new string[]{"유저 메세지", "고용완료"});
        return;
    }

    void OnClickedBtnShowNews()
    {
        SoundManager.Instance.PlayButtonSound();
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
        // 이벤트가 적용되는 용병인지 체크 후 가격 반영
        if(IsEventUnit(data))
        {
            if(SelectedTabType == E_TabType.Purchase)
                sb.Append("( - ");
            else if (SelectedTabType == E_TabType.Sell)
                sb.Append("( + ");

            sb.Append(WorldEventManager.Instance.PresentWorldEventData.EventEffectValue);
            sb.Append("%)");
        }

        sb.AppendLine();
        sb.AppendLine();
        sb.Append(data.Description);

        return sb.ToString();
    }

    void OnChangedToggle(bool isOn)
    {
        if (!isOn)
            return;

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

    bool IsEventUnit(UnitData data)
    {
        if(data == null)
        {
            Debug.LogError("unitData is null");
            return false;
        }

        WorldEventData presentEventData = WorldEventManager.Instance.PresentWorldEventData;
        switch (presentEventData.WorldEventType)
        {
            case E_WorldEventType.AllSale:
                if(selectedTabType != E_TabType.Purchase)
                    break;

                return true;

            case E_WorldEventType.WarriorSale:
                if(selectedTabType != E_TabType.Purchase)
                    break;

                if(data.CharacterType != E_CharacterType.Warrior)
                    break;

                return true;

            case E_WorldEventType.MageSale:
                if(selectedTabType != E_TabType.Purchase)
                    break;
                    
                if(data.CharacterType != E_CharacterType.Mage)
                    break;

                return true;

            case E_WorldEventType.WarlockSale:
                if(selectedTabType != E_TabType.Purchase)
                    break;
                    
                if(data.CharacterType != E_CharacterType.Warlock)
                    break;

                return true;

            case E_WorldEventType.RogueSale:
                if(selectedTabType != E_TabType.Purchase)
                    break;
                    
                if(data.CharacterType != E_CharacterType.Rogue)
                    break;

                return true;

            case E_WorldEventType.AllPremium:
                if(selectedTabType != E_TabType.Sell)
                    break;

                return true;

            case E_WorldEventType.WarriorPremium:
                if(selectedTabType != E_TabType.Sell)
                    break;

                if(data.CharacterType != E_CharacterType.Warrior)
                    break;

                return true;

            case E_WorldEventType.MagePremium:
                if(selectedTabType != E_TabType.Sell)
                    break;

                if(data.CharacterType != E_CharacterType.Mage)
                    break;

                return true;
            case E_WorldEventType.WarlockPremium:
                if(selectedTabType != E_TabType.Sell)
                    break;

                if(data.CharacterType != E_CharacterType.Warlock)
                    break;

                return true;

            case E_WorldEventType.RoguePremium:
                if(selectedTabType != E_TabType.Sell)
                    break;

                if(data.CharacterType != E_CharacterType.Rogue)
                    break;

                return true;
        }

        return false;
    }
}
