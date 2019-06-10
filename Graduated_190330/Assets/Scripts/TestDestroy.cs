using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface iTest
{
    void Show();
    void Close();
}

public class Test0 : iTest
{
    public void Close()
    {
        
    }

    public void Show()
    {
        
    }
}

public class Test1 : iTest
{
    public void Close()
    {
        Debug.Log("test1 close");
    }

    public void Show()
    {
        Debug.Log("test1 show");
    }
}

public class Test2 : iTest
{
    public void Close()
    {
        Debug.Log("test2 close");
    }

    public void Show()
    {
        Debug.Log("test2 show");
    }
}

public class Test3 : iTest
{
    public void Close()
    {
        Debug.Log("test3 close");
    }

    public void Show()
    {
        Debug.Log("test3 show");
    }
}

public enum E_TestType
{
    First,
    Second,
    Third,
}

public class TestDestroy : MonoBehaviour
{
    [SerializeField] E_TestType testType;
    private void Start()
    {
        iTest test = new Test0();
        switch (testType)
        {
            case E_TestType.First:
                test = new Test1();
                break;

            case E_TestType.Second:
                test = new Test2();
                break;

            case E_TestType.Third:
                test = new Test3();
                break;
        }

        test.Show();
        test.Close();

        Test1 test1 = test as Test1;
        test1.Show();
        test1.Close();
    }
}
