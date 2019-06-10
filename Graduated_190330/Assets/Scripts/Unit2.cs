using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit2 : MonoBehaviour
{
    public string id = string.Empty;
    public string unitName = string.Empty;
    public E_Class unitClass = E_Class.None;
    public int level = 0;
    public E_GroupTag groupTag;

    StatManager statManager = new StatManager();
    BuffManager buffManager = new BuffManager();
    public StatManager StatManager
    {
        get
        {
            return statManager;
        }
    }
    public BuffManager BuffManager
    {
        get
        {
            return buffManager;
        }
    }

    Vector3 direction;
    public Transform targetPosition;
    void MovePosition(Vector3 targetPosition)
    {
        if (direction.x >= 0)
        {
            if (targetPosition.x > transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime * 0.5f);
            }
        }
        else
        {
            if (targetPosition.x < transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue * Time.deltaTime * 0.5f);
            }
        }
    }
    IEnumerator AutoMoving()
    {
        while(true)
        {
            if(targetPosition)
            {
                MovePosition(targetPosition.position);
            }
            yield return null;
        }
    }
    private void Start()
    {
        statManager.CreateOrGetStat(E_StatType.MoveSpeed).ModifiedValue = 2;
        StartCoroutine(AutoMoving());
    }



}
