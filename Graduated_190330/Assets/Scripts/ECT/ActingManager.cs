using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ActingManager : MonoBehaviour
{
    public List<ActInfo> actionList = new List<ActInfo>();
    Animator animator;

    [Serializable]
    public struct ActInfo
    {
        public string actionName;
        public float actionTime;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ActionLoop());
    }
    IEnumerator ActionLoop()
    {
        foreach(ActInfo actionInfo in actionList)
        {
            animator.Play(actionInfo.actionName);
            yield return new WaitForSeconds(actionInfo.actionTime);
        }
    }
}
