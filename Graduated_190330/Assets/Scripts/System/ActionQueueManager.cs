using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueueManager
{
    Queue<Skill> actionQueue = new Queue<Skill>();
    public bool isActing = false;

    public bool IsEmpty
    {
        get
        {
            return (actionQueue.Count == 0);
        }
    }

    //용사 행동큐에 뭔가 존재하는가.
    //실행
    //비우기

    private void Awake()
    {
    }




    public void AddAction(Skill skill)
    {
        actionQueue.Enqueue(skill);
    }
    public Skill DequeueAction()
    {
        return actionQueue.Dequeue();
    }

    public void DoAction()
    {
        actionQueue.Dequeue();
        //스킬 행동
    }

    public void RemoveAll()
    {
        actionQueue.Clear();
    }
}
