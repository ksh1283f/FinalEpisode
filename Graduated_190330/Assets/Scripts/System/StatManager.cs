using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager
{
    Dictionary<E_StatType, StatFloat> statDictionary;
    public StatManager()
    {
        statDictionary = new Dictionary<E_StatType, StatFloat>();
    }

    /// <summary>
    /// 해당 스테이터스 값이 존재하는지 확인한다.
    /// </summary>
    /// <param name="_statType"></param>
    /// <returns></returns>
    public bool Contains(E_StatType _statType)
    {
        return statDictionary.ContainsKey(_statType);
    }
    /// <summary>
    /// 해당 스테이터스 값을 획득한다.
    /// </summary>
    /// <param name="_statType"></param>
    /// <returns></returns>
    public StatFloat Get_Stat(E_StatType _statType)
    {
        if (Contains(_statType))
        {
            return statDictionary[_statType];
        }
        return null;
    }
    /// <summary>
    /// 해당 스테이터스 값을 신규 작성한다.
    /// </summary>
    /// <param name="_statType"></param>
    /// <returns></returns>
    StatFloat Create_Stat(E_StatType _statType)
    {
        StatFloat stat = new StatFloat(_statType, 0);
        statDictionary.Add(_statType, stat);
        return stat;
    }
    StatFloat Create_Stat(E_StatType _statType,float baseValue)
    {
        StatFloat stat = new StatFloat(_statType, baseValue);
        statDictionary.Add(_statType, stat);
        return stat;
    }
    /// <summary>
    /// 해당 스테이터스 값이 존재하면 리턴하고 없다면 신규 작성후 빈 스테이터스를 리턴한다.
    /// </summary>
    /// <param name="_statType"></param>
    /// <returns></returns>
    public StatFloat CreateOrGetStat(E_StatType _statType)
    {
        StatFloat stat = Get_Stat(_statType);
        if (stat == null)
        {
            stat = Create_Stat(_statType);
        }
        return stat;
    }
    /// <summary>
    /// 해당 스테이터스 값을 추가한다. 가급적 ConfigureStats를 통해 초기화 하는것을 권장함.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stat_Type"></param>
    /// <param name="stat"></param>
    public void AddStat(E_StatType stat_Type, StatFloat stat)
    {
        if (Contains(stat_Type))
        {
            Debug.Log(stat_Type + "은 이미 존재합니다.");
        }
        else
        {
            statDictionary.Add(stat_Type, stat);
        }
    }
}
