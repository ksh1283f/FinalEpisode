using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletone<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    T t = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }
}

public class uiSingletone<T> : MonoBehaviour, IBaseUI
    where T: MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    T t = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    protected E_UIType uiType;

    protected virtual void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        Debug.Log(gameObject.name+"initilized");
        UIManager.Instance.InsertUI(uiType, this);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        UIManager.Instance.OnShow.Execute(uiType, this);
    }

    public virtual void Show(string[] dataList)
    {
        if (dataList == null)
            return;

        gameObject.SetActive(true);
        UIManager.Instance.OnShow.Execute(uiType, this);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        UIManager.Instance.OnClose.Execute(uiType);
    }
}