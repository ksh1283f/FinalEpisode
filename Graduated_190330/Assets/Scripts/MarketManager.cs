using System.Collections;
using System.Collections.Generic;
using Graduate.GameData.UnitData;
using UnityEngine;

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
        
        // todo 유닛 생성 작업
        // 1. Select character type(random)
        // 2. Select unit level(from userLevel)
        // 3. Make Stat(from MarketSpecialData)


        return resultUnitData;
    }

    public void BuySpecialUnit(UnitData boughtUnit)
    {
        // todo 구매한 유닛은 유닛id를 갱신해줘야함
    }

}
