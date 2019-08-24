using System.Collections;
using System.Collections.Generic;
using System.Text;
using Graduate.GameData.UnitData;
using UnityEngine;
using System;

public enum E_MarketEventType
{
    None,
    Discount,   // 특정 용병 할인
    UrgentWant, // 특정 용병 급구

}

public class MarketManager : Singletone<MarketManager>
{
    /* 마켓 이벤트
        특정 용병을 이벤트에 따라 다르게 구매 또는 판매 가능
        몇턴동안 지속
        턴은 전투를 시작할때 카운트

        뉴스 시스템
        - ex. 조만간 어떤 사건이 발생하여 어떤 용병이 많이 필요해질것이다.
        - ex. 

        - marketSpecialEventData: 구매 또는 판매 쪽에서 value값만큼(퍼센트) 보너스를 받을 수 있다.
        
    */

    public int RemainTurn { get; private set; }
    public List<UnitData> PurchaseableUnitList;

    void Start()
    {
        PurchaseableUnitList = new List<UnitData>();
    }

    // 이벤트의 남은 턴, 구매 가능한 용병 리스트
    public void SetMarketBasisData()
    {

    }

    public UnitData MakeEventUnitData(int userLevel)
    {
        UnitData resultUnitData = null;

        // <유닛 생성 작업>
        // 1. Select character type(random) -> job
        int characterTypeKey = RandomProcess((int)E_CharacterType.None + 1, (int)E_CharacterType.EnumDataCount);
        // E_CharacterType characterType = (E_CharacterType)characterTypeKey;
        // -> 유닛 기본값
        resultUnitData = GameDataManager.Instance.CharacterDataDic[characterTypeKey-1].ShallowCopy() as UnitData;

        // 2. Select unit level(from userLevel) -> level
        int offset = RandomProcess(1, 11);
        int level = RandomProcess(userLevel - offset, userLevel + offset + 1);
        resultUnitData.UpdateLevel(level);

        // 3. price: 기본가 + 레벨*10
        int price = GameDataManager.Instance.CharacterDataDic[characterTypeKey - 1].Price + level * 10;
        resultUnitData.UpdatePrice(price);

        // 4. Make Stat(from MarketSpecialData)-> ??
        // 4-1. 50% 확률로 보너스가 붙을지 안붙을지를 계산
        bool isBonusStat = RandomProcess(1,11) > 5 ? true :false;
        // 4-2. 붙으면 가중치를 계산하여 반영
        if(isBonusStat)
        {
            float bonusValue = RandomProcess(1,11);
            // todo
            
        }



        return resultUnitData;
    }

    public string MakeNewsData()
    {
        // todo 데이터에서 몇개 뽑아오기
        // 뉴스 2개
        StringBuilder sb = new StringBuilder();
        int key1 = RandomProcess(0, GameDataManager.Instance.MarketSpecialEventDataDic.Count);
        sb.Append(GameDataManager.Instance.MarketSpecialEventDataDic[key1].Description);
        sb.AppendLine();
        if (GameDataManager.Instance.MarketSpecialEventDataDic.Count > 1)
        {
            int key2 = RandomProcess(0, GameDataManager.Instance.MarketSpecialEventDataDic.Count);
            while (key1 == key2)    // 두개의 키가 같으면 다를때까지
                key2 = RandomProcess(0, GameDataManager.Instance.MarketSpecialEventDataDic.Count);

            sb.Append(GameDataManager.Instance.MarketSpecialEventDataDic[key2].Description);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void BuySpecialUnit(UnitData boughtUnit)
    {
        // todo 구매한 유닛은 유닛id를 갱신해줘야함
    }

    private int RandomProcess(int min, int max)
    {
        int retValue = -1;

        //  min~ max 사이의 값
        retValue = UnityEngine.Random.Range(min, max);
        return retValue;
    }
}
