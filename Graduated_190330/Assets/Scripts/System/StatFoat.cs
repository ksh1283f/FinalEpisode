using UnityEngine.Events;
using UnityEngine;

public partial class StatFloat : Stat
{
    float baseValue;
    float modifiedValue;

    FloatEvent modificationEvent= new FloatEvent();
}

public partial class StatFloat : Stat
{


    public float BaseValue
    {
        get
        {
            return baseValue;
        }
        set
        {
            baseValue = value;
        }
    }
    public float ModifiedValue
    {
        get
        {
            // if(statType == E_StatType.MoveSpeed)
            //     Debug.Log("get speed: "+ modifiedValue);

            return modifiedValue;
        }
        set
        {
            modifiedValue = value;
            modificationEvent.Invoke(modifiedValue);
        }
    }

    public void AddEvent(UnityAction<float> statEvent)
    {
        modificationEvent.AddListener(statEvent);
    }
    /// <summary>
    /// 해당 이벤트를 구독해제한다.
    /// </summary>
    /// <param name="statEvent"></param>
    public void RemoveEvent(UnityAction<float> statEvent)
    {
        modificationEvent.RemoveListener(statEvent);
    }
    /// <summary>
    /// 모든 이벤트를 구독 해제한다.
    /// </summary>
    public void RemoveAllEvent()
    {
        modificationEvent.RemoveAllListeners();
    }


    public StatFloat()
    {
        statName = "needName";
        baseValue = 0;
        modifiedValue = baseValue;
    }
    public StatFloat(string _name, float _baseValue)
    {
        statName = _name;
        baseValue = _baseValue;
        modifiedValue = baseValue;
    }
    public StatFloat(E_StatType _statType)
    {
        statName = _statType.ToString();
        baseValue = 0;
        modifiedValue = baseValue;
    }
    public StatFloat(E_StatType _statType, string _name, float _baseValue)
    {
        statType = _statType;
        statName = _name;
        baseValue = _baseValue;
        modifiedValue = baseValue;
    }
    public StatFloat(E_StatType _statType, float _baseValue)
    {
        statType = _statType;
        statName = _statType.ToString();
        baseValue = _baseValue;
        modifiedValue = baseValue;
    }
    public StatFloat(E_StatType _statType, string _name, float _baseValue,float _modifiedValue)
    {
        statType = _statType;
        statName = _name;
        baseValue = _baseValue;
        modifiedValue = _modifiedValue;
    }
}
