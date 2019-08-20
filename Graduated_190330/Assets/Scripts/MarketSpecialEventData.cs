using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketSpecialEventData : MonoBehaviour
{
    public int Id {get; private set;}
    public E_MarketEventType MarketEventType {get; private set;}
    public int EventValue {get; private set;}
    public string Description {get; private set;}

    public MarketSpecialEventData(int id, E_MarketEventType eventType, int eventValue, string description)
    {
        Id = id;
        MarketEventType = eventType;
        EventValue = eventValue;
        Description = description;
    }
}
