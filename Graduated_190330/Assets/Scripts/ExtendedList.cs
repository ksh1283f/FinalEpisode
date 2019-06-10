using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedList<T> : List<T>
{
    public delegate void IntEvent(int num);
    public delegate void ItemEvent(T item);

    public event IntEvent CountListener;
    public event ItemEvent AddListener;
    public event ItemEvent RemoveListener;

    public new void Add(T item)
    {
        base.Add(item);
        if (CountListener != null)
        {
            CountListener(Count);
        }
        if (AddListener != null)
        {
            AddListener(item);
        }
    }
    public new void Remove(T item)
    {
        base.Remove(item);
        if (CountListener != null)
        {
            CountListener(Count);
        }
        if (RemoveListener != null)
        {
            RemoveListener(item);
        }
    }
}
