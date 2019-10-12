using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_WorldEventType
{
    None,
    AllSale,    // 새로 나온 용병을 싸게 구매
    WarriorSale,
    MageSale,
    WarlockSale,
    RogueSale,

    AllPremium, // 현재 보유중인 용병을 비싸게 판매 
    WarriorPremium,
    MagePremium,
    WarlockPremium,
    RoguePremium,
}

public enum E_WorldEventNoticeIconType
{
    None,
    West,
    East,
    Mid,
    South,
}

public class WorldEventData
{
    public int Id { get; private set;}
    public E_WorldEventType WorldEventType {get; private set;}
    public string EventDescription { get; private set;}
    public int EventEffectValue {get; private set;}
    public E_WorldEventNoticeIconType IconType {get; private set;}

    public WorldEventData(int id, E_WorldEventType eventType, string eventDescription, int eventEffectValue)
    {
        Id = id;
        WorldEventType = eventType;
        EventDescription = eventDescription;
        EventEffectValue = eventEffectValue;
    }

    public void SetWorldEventNoticeIconType(E_WorldEventNoticeIconType type)
    {
        IconType = type;
    }
}
