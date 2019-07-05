using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_UIType
{
    Start,
    Lobby,
    Input,
    ShowMessage,
    BattleEnd,  // obsolete
    BattlePhaseInfo,    // obsolete
    Battle,
    CharacterProperty,
    DungeonSelect,
}

public interface IBaseUI
{
    void Initialize();
    void Show();
    void Close();
}

public class UIManager : Singletone<UIManager>
{
    Dictionary<E_UIType, IBaseUI> uiDic = new Dictionary<E_UIType, IBaseUI>();
    public Dictionary<E_UIType, IBaseUI> openedUiDic { get; private set; }
    public Action<E_UIType> OnClose { get; set; }
    public Action<E_UIType, IBaseUI> OnShow { get; set; }    // 모든 ui가 싱글톤이 아닐 수도..?

    void Awake()
    {
        openedUiDic = new Dictionary<E_UIType, IBaseUI>();
        OnClose = OnCloseUI;
        OnShow = OnShowUI;
    }

    public void InsertUI(E_UIType uiType, IBaseUI ui)
    {
        Debug.Log("Inserted " + uiType);
        if (!uiDic.ContainsKey(uiType))
        {
            uiDic.Add(uiType, ui);
        }
        else
        {
            // if(uiDic[uiType] == null)
                uiDic[uiType] = ui;
        }
    }

    public IBaseUI LoadUI(E_UIType uiType)
    {
        if (!uiDic.ContainsKey(uiType))
            return null;

        IBaseUI target = uiDic[uiType];

        return target;
    }

    void OnCloseUI(E_UIType uiType)
    {
        if (openedUiDic == null)
            return;

        openedUiDic.Remove(uiType);
        Debug.Log(uiType + "closed, openDic count: " + openedUiDic.Count);
    }

    void OnShowUI(E_UIType uiType, IBaseUI baseUI)
    {
        if (openedUiDic == null)
            return;

        openedUiDic.Add(uiType, baseUI);
        Debug.Log(uiType + "showed, openDic count: " + openedUiDic.Count);
    }
}
