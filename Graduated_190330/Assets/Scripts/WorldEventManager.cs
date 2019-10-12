using System;
using System.Collections;
using System.Collections.Generic;
using Graduate.GameData.UnitData;
using UnityEngine;

public class WorldEventManager : Singletone<WorldEventManager> {
    /*
    마을에 입장할때마다 갱신(새로 접속을 하는경우 또는 전투를 마치고 오는 경우 등등)
    - 
     */
    private const int MIN_NOTICE_ICON_INDEX = 1;
    private const int MAX_NOTICE_ICON_INDEX = 4;

    private const int MIN_EVENT_UNIT_LIST_COUNT = 1;
    private const int MAX_EVENT_UNIT_LIST = 10;

    // 파싱한 월드 이벤트 데이터
    public Dictionary<E_WorldEventType, WorldEventData> WorldEventDataDic { get; private set; }

    // 랜덤하게 나온 유닛데이터리스트
    public List<UnitData> eventUnitList { get; private set; }

    // 현재 이벤트 데이터
    public WorldEventData PresentWorldEventData {get; private set;}

    void Awake () {
        WorldEventDataDic = new Dictionary<E_WorldEventType, WorldEventData> ();
        eventUnitList = new List<UnitData>();

    }

    void Start () {
        InitEventDic ();
    }

    // 앞에서 읽어온 데이터를 가지고 초기화해준다.
    public void InitEventDic () {
        foreach (var item in GameDataManager.Instance.WorldEventDataDic) {

            E_WorldEventNoticeIconType type = (E_WorldEventNoticeIconType) GetRandomizedValue (MIN_NOTICE_ICON_INDEX, MAX_NOTICE_ICON_INDEX);
            WorldEventData data = item.Value;
            data.SetWorldEventNoticeIconType (type);
            WorldEventDataDic.Add (data.WorldEventType, data);
        }

        // init 후 한번 update한다
        UpdateEvent ();
        UpdateEventUnitList();
    }

    // ui에서 보여줄 이벤트 데이터를 갱신한다.
    public void UpdateEvent () {
        // todo randomize, to set the notice icon type
        int randomizedIndex = GetRandomizedValue (0, WorldEventDataDic.Count - 1);
        if (randomizedIndex == -1) {
            Debug.LogError ("Invalid randomizedIndex!");
            return;
        }

        E_WorldEventType eventType = (E_WorldEventType) randomizedIndex;
        PresentWorldEventData = WorldEventDataDic[eventType];
    }

    // 현재 보여줄 이벤트데이터를 반환
    public WorldEventData GetPresentEventData () {
        return PresentWorldEventData;
    }

    int GetRandomizedValue (int min, int max) {
        int retVal = -1;
        retVal = UnityEngine.Random.Range (min, max + 1); // max는 exclusive이므로

        return retVal;
    }

    void UpdateEventUnitList()
    {
        int unitListCount = GetRandomizedValue(MIN_EVENT_UNIT_LIST_COUNT, MAX_EVENT_UNIT_LIST);
        if(unitListCount == -1)
        {
            Debug.LogError("Failed to update eventUnitList!");
            return;
        }

        // 구매 이벤트에 따른 확정적 유닛 하나 생성
        UnitData essentialData = null;
        switch (PresentWorldEventData.WorldEventType)
        {
            case E_WorldEventType.WarriorSale:
                essentialData = GameDataManager.Instance.CharacterDataDic[0].ShallowCopy() as UnitData;
                break;

            case E_WorldEventType.MageSale:
                essentialData = GameDataManager.Instance.CharacterDataDic[1].ShallowCopy() as UnitData;
                break;

            case E_WorldEventType.WarlockSale:
                essentialData = GameDataManager.Instance.CharacterDataDic[2].ShallowCopy() as UnitData;
                break;

            case E_WorldEventType.RogueSale:
                essentialData = GameDataManager.Instance.CharacterDataDic[3].ShallowCopy() as UnitData;
                break;
        }

        if (essentialData != null)
        {
            float effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
            essentialData.UpdatePrice(Convert.ToInt32(essentialData.Price*effectValue));
            eventUnitList.Add(essentialData);
        }

        for (int i = 0; i < unitListCount; i++)
        {
            // 생성할 유닛?
            int getUnitId = GetRandomizedValue(0,3);
            if(getUnitId == -1)
            {
                Debug.LogError("Failed to get unitId");
                return;
            }

            UnitData eventUnit = GameDataManager.Instance.CharacterDataDic[getUnitId].ShallowCopy() as UnitData; 
            // 할인 또는 프리미엄 작업
            float effectValue;
            switch (PresentWorldEventData.WorldEventType)
            {
                case E_WorldEventType.AllSale:
                    effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
                    Debug.LogError("effectValue: "+ effectValue);
                    eventUnit.UpdatePrice(Convert.ToInt32(eventUnit.Price * effectValue));
                    break;

                case E_WorldEventType.WarriorSale:
                    if(eventUnit.CharacterType != E_CharacterType.Warrior)
                        break;
                        
                    effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
                    Debug.LogError("effectValue: "+ effectValue);
                    eventUnit.UpdatePrice(Convert.ToInt32(eventUnit.Price * effectValue));
                    break;

                case E_WorldEventType.MageSale:
                    if(eventUnit.CharacterType != E_CharacterType.Mage)
                        break;

                    effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
                    Debug.LogError("effectValue: "+ effectValue);
                    eventUnit.UpdatePrice(Convert.ToInt32(eventUnit.Price * effectValue));
                    break;

                case E_WorldEventType.WarlockSale:
                    if(eventUnit.CharacterType != E_CharacterType.Warlock)
                        break;
                        
                    effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
                    Debug.LogError("effectValue: "+ effectValue);
                    eventUnit.UpdatePrice(Convert.ToInt32(eventUnit.Price * effectValue));
                    break;

                case E_WorldEventType.RogueSale:
                    if(eventUnit.CharacterType != E_CharacterType.Rogue)
                        break;

                    effectValue = 1-((float)PresentWorldEventData.EventEffectValue/100);
                    Debug.LogError("effectValue: "+ effectValue);
                    eventUnit.UpdatePrice(Convert.ToInt32(eventUnit.Price * effectValue));
                    break;
            }

            eventUnitList.Add(eventUnit);
        }
    }

    public void BuyEventUnit(UnitData selectedData)
    {
        // 모든 조건이 충족되면, 새로운 사용가능한 id부여, 유저인포의 딕셔너리 갱신
        UnitData purchasedUnit = selectedData.ShallowCopy() as UnitData;
        int changedId = GetCreatedUnitID();
        if(changedId == -1)
            return;
        
        purchasedUnit.UpdateUnitID(changedId);
        UserManager.Instance.SetMyUnitList(purchasedUnit);
        int gold = UserManager.Instance.UserInfo.Gold;
        gold -= purchasedUnit.Price;
        UserInfo userInfo = UserManager.Instance.UserInfo;

        UserManager.Instance.SetUserInfo(userInfo.UserName,userInfo.TeamLevel,userInfo.Exp,gold);
        MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
        message.Show(new string[]{"유저 메세지", "고용완료"});
        return;
    }

    public int GetCreatedUnitID()
    {
        // 100번부터 시작
        int retVal = -1;
        int index = 0;
        do
        {
            retVal = UnityEngine.Random.Range(100, 200);
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

    public bool IsSaleEvent(E_WorldEventType type)
    {
        switch (type)
        {
            case E_WorldEventType.AllSale:
            case E_WorldEventType.WarriorSale:
            case E_WorldEventType.MageSale:
            case E_WorldEventType.WarlockSale:
            case E_WorldEventType.RogueSale:
                return true;

            case E_WorldEventType.AllPremium:
            case E_WorldEventType.WarriorPremium:
            case E_WorldEventType.MagePremium:
            case E_WorldEventType.RoguePremium:
                return false;
        }

        Debug.LogError("Invalid Type IsSaleEvent()");
        return false;
    }
}