using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_SimpleTutorialType {
    None = -1,
    HireUnit,
    BattleIntro,
    Battle,
    UnitManagement,
    BattleProperty,
    IntroduceMarket,

    E_SimpleTutorialTypeCount,
}

public class TutorialSimpleManager : Singletone<TutorialSimpleManager>
{
    public E_SimpleTutorialType SimpleType {get; private set;}
    
    void Start()
    {
        
    }
}
