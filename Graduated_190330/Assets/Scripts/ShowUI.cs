using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_TypeOfUI
{
    Singletone, // 고유한 기능을 하는 UI
    Input,  // 값을 입력하는 UI
    MessageBox, // 메세지 전달만 하는 UI (버튼 1개)
    YesOrNo,
}

public class ShowUI: uiSingletone<ShowUI>, IBaseUI
{
    protected override void Awake()
    {
        uiType = E_UIType.ShowMessage;
        base.Awake();
    }

    //public static void ShowInput(string value, Action okFunc)
    //{
    //    // todo 인스턴스 생성후 콜백을 어떻게 넘길지?
    //}

    //public static void MessageBox(Action okFunc)
    //{

    //}

    //public static void ShowYesOrNo(Action yesFunc, Action noFunc)
    //{

    //}

}
